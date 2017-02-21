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

			bool[] boltChecks = new bool[5];


			

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

			ArrayList myBoltArrayLS = new ArrayList();

			foreach (LineSegment ls in boltLines)
			{
				ArrayList myBoltSubArrayLS = new ArrayList();
				myBoltSubArrayLS.Add(ls);
				foreach (Part part in boltedParts)
				{
					
					ArrayList intersections = new ArrayList(part.GetSolid().Intersect(ls));
					if (intersections.Count == 0 || intersections.Count % 2 != 0)
					{
						Console.WriteLine("check 1 NO!");
						boltChecks[0] = false;
						break;
					}
					else
					{
						LineSegment myLineSegment = new LineSegment(intersections[0] as Point, intersections[intersections.Count - 1] as Point);
						myBoltSubArrayLS.Add(myLineSegment);
					} 	
				}
				myBoltArrayLS.Add(myBoltSubArrayLS);

			}
			int numOfBolt = 0;

			foreach (ArrayList a in myBoltArrayLS)
			{
				Console.WriteLine(numOfBolt++);
				int numOfLS = 0;
				foreach (LineSegment ls in a)
				{
					Console.WriteLine(numOfLS++);
					Console.WriteLine(ls.Point1);
					Console.WriteLine(ls.Point2);
				}
			}


        }
    }
}
