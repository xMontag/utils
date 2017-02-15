using System;
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



			Console.WriteLine(boltNParts + "\n" +
							  boltDiameter + "\n" +
							  boltLength);
			
			Console.WriteLine(myBoltGroup.BoltStandard);
			Console.WriteLine(myBoltGroup.CutLength);

			foreach (Part part in boltedParts)
			{
				Console.WriteLine(part.Profile.ProfileString);
				ArrayList intesections = new ArrayList(part.GetSolid().Intersect(new Point(12387.87, 212.13, 104.0),new Point(12158.07, 220.97, -131.4)));
				foreach (Point myPoint in intesections)
				{
					Console.WriteLine(myPoint.ToString());
				}
			}
			foreach (Point cord in myBoltGroup.BoltPositions)
			{
				Console.WriteLine(cord.ToString());
			}
			
        }


    }
}
