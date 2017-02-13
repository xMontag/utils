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
			string[] boltPartGUID = new string[boltNParts];
			myBoltGroup.GetReportProperty("MAIN_PART.GUID", ref boltPartGUID[0]);
			if (boltNParts > 1)
			{
				for (int i = 1; i < boltNParts; i++)
				{
					myBoltGroup.GetReportProperty("SECONDARY_" + i + ".GUID", ref boltPartGUID[i]);
				}
			}
			myBoltGroup.GetReportProperty("LENGTH",ref boltLength);
			myBoltGroup.GetReportProperty("DIAMETER", ref boltDiameter);



			Console.WriteLine(boltNParts + "\n" +
							  boltDiameter + "\n" +
							  boltLength);
			foreach (String GUID in boltPartGUID)
			{
				Console.WriteLine(GUID);
			}
			Console.WriteLine(myBoltGroup.BoltStandard);
			
        }


    }
}
