using System;
using System.Globalization;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Solid;


namespace utils
{
    public class ModelUtils
    {
		private readonly Model _Model = new Model();
		private static GraphicsDrawer GraphicsDrawer = new GraphicsDrawer();
		private readonly static Color TextColor = new Color(1, 0, 1);
		public static void CheckOneBoltGroup()
        {
			

            Picker picker = new Picker();            
            BoltGroup myBoltGroup = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_BOLTGROUP, "pick BOLT GROUP") as BoltGroup;
			double boltLength = 0,
			       boltDiameter = 0;
			int boltNParts = 1;
			myBoltGroup.GetReportProperty("BOLT_NPARTS", ref boltNParts);
			List<Part> boltedParts = new List<Part>();

			boltedParts.Add(myBoltGroup.PartToBoltTo);
			if (boltNParts > 2)
			{
				foreach (Part part in myBoltGroup.OtherPartsToBolt)
				{
					boltedParts.Add(part);
				}
			}
			if (boltNParts > 1)
			{
				boltedParts.Add(myBoltGroup.PartToBeBolted);
			}

			myBoltGroup.GetReportProperty("LENGTH",ref boltLength);
			myBoltGroup.GetReportProperty("DIAMETER", ref boltDiameter);

			double boltExtraLenght = myBoltGroup.ExtraLength;
			double boltCutLength = myBoltGroup.CutLength;

			ArrayList boltLines = new ArrayList();

			CoordinateSystem boltCS = new CoordinateSystem(myBoltGroup.FirstPosition, myBoltGroup.GetCoordinateSystem().AxisX, myBoltGroup.GetCoordinateSystem().AxisY);
			Matrix transformationMatrix = MatrixFactory.ToCoordinateSystem(boltCS);

			//Console.WriteLine(boltCS.Origin);
			//Console.WriteLine(myBoltGroup.SecondPosition);
			//Console.WriteLine(transformationMatrix.Transform(myBoltGroup.SecondPosition));
			foreach (Point p in myBoltGroup.BoltPositions)
			{
				Point myPoint = new Point(transformationMatrix.Transform(p));
				Point pStart = new Point(myPoint.X, myPoint.Y, myPoint.Z - boltCutLength);
				Point pEnd = new Point(myPoint.X, myPoint.Y, myPoint.Z + boltCutLength);
				LineSegment ls = new LineSegment(MatrixFactory.FromCoordinateSystem(boltCS).Transform(pStart), MatrixFactory.FromCoordinateSystem(boltCS).Transform(pEnd));
				GraphicsDrawer.DrawText(ls.Point1, "start", TextColor);
				GraphicsDrawer.DrawText(ls.Point2, "end", TextColor);
				boltLines.Add(ls);
			}


			foreach (LineSegment ls in boltLines)
			{
				foreach (Part part in boltedParts)
				{
					ArrayList intesections = new ArrayList(part.GetSolid().Intersect(ls));
					foreach (Point myPoint in intesections)
					{
						Console.WriteLine(myPoint.ToString());
					}
				}
			}
        }
    }
}
