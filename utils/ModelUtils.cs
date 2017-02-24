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
			// число деталей в пакете
			int boltNParts = 1;
			myBoltGroup.GetReportProperty("BOLT_NPARTS", ref boltNParts);

			// спислк деталей пакета

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

			// другие свойства болта
			myBoltGroup.GetReportProperty("LENGTH",ref boltLength);
			myBoltGroup.GetReportProperty("DIAMETER", ref boltDiameter);

			double boltExtraLenght = myBoltGroup.ExtraLength;
			double boltCutLength = myBoltGroup.CutLength;

			// список отрезков по каждому болту

			ArrayList boltLines = new ArrayList();
			// система координат болта
			CoordinateSystem boltCS = new CoordinateSystem(myBoltGroup.FirstPosition, myBoltGroup.GetCoordinateSystem().AxisX, myBoltGroup.GetCoordinateSystem().AxisY);
			Matrix transformationMatrix = MatrixFactory.ToCoordinateSystem(boltCS);

			//Console.WriteLine(boltCS.Origin);
			//Console.WriteLine(myBoltGroup.SecondPosition);
			//Console.WriteLine(transformationMatrix.Transform(myBoltGroup.SecondPosition));
			foreach (Point p in myBoltGroup.BoltPositions)
			{
				Point myPoint = new Point(transformationMatrix.Transform(p));
				Point pStart = new Point(myPoint.X, myPoint.Y, myPoint.Z - boltCutLength / 2);
				Point pEnd = new Point(myPoint.X, myPoint.Y, myPoint.Z + boltCutLength / 2);
				LineSegment ls = new LineSegment(MatrixFactory.FromCoordinateSystem(boltCS).Transform(pStart), MatrixFactory.FromCoordinateSystem(boltCS).Transform(pEnd));
				//GraphicsDrawer.DrawText(ls.Point1, "start", TextColor);
				//GraphicsDrawer.DrawText(ls.Point2, "end", TextColor);
				boltLines.Add(ls);
			}

			// список отрезков пересечения каждого болта с каждой деталью пакета + проверка на правильность в модели CHECK - 0

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
						//Console.WriteLine("check 1 NO!");
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


			// проверка болтового поля на правильность в модели CHECK - 0

			foreach (ArrayList a in myBoltArrayLS)
			{
				if (!(compareArrayLS(myBoltArrayLS[0] as ArrayList, a)))
				{
					boltChecks[0] = false;
					break;
				}
				else
				{
					boltChecks[0] = true;
				}
				//Console.WriteLine(numOfBolt++);

				//int numOfLS = 0;
				//foreach (LineSegment ls in a)
				//{
					//Console.WriteLine(numOfLS++);
					//Console.WriteLine(ls.Length());
					//Console.WriteLine(ls.Point1);
					//Console.WriteLine(ls.Point2);
				//}
			}

			// составление списка отрезков пересечений одного болта где под индексом 0 полный отрезок болта

			ArrayList myBoltArrayOneLS = new ArrayList();
			foreach (LineSegment ls in myBoltArrayLS[0] as ArrayList)
			{
				myBoltArrayOneLS.Add(TransformLineSegmentToCS(ls, boltCS));

			}


			// вывод точек в консоль
			foreach (LineSegment ls in myBoltArrayOneLS)
			{
				Console.WriteLine(ls.Point1 + " " + ls.Point2);
			}





			Console.WriteLine(boltChecks[0]);
		}

		// проверка отрезков пересечения с деталью на одинаковость во всех болтах болтового поля

		public static bool compareArrayLS(ArrayList Arr1, ArrayList Arr2)
		{
			//Console.WriteLine(Arr1.Count + " " + Arr2.Count);
			if (Arr1.Count != Arr2.Count)
			{
				return false;
			}
			else
			{
				LineSegment l1 = Arr1[0] as LineSegment;;
				LineSegment l2 = Arr2[0] as LineSegment;;
				LineSegment ls1;
				LineSegment ls2;

				for (int i = 1; i < Arr1.Count; i++)
				{
					ls1 = Arr1[i] as LineSegment;
					ls2 = Arr2[i] as LineSegment;

					LineSegment segm1Start = new LineSegment(l1.Point1, ls1.Point1);
					LineSegment segm1End = new LineSegment(l1.Point1, ls1.Point2);
					LineSegment segm2Start = new LineSegment(l2.Point1, ls2.Point1);
					LineSegment segm2End = new LineSegment(l2.Point1, ls2.Point2);

					//Console.WriteLine(segm1Start.Length() + " " + segm2Start.Length());
					//Console.WriteLine(segm2End.Length() + " " + segm2End.Length());
					if ((Math.Abs(segm1Start.Length() - segm2Start.Length()) > 0.01) && (Math.Abs(segm1End.Length() - segm2End.Length()) > 0.01))
						{
							return false;
						}
				}
			}
			return true;
		}

		// перевод отрезка из одной системы координат в другую

		public static LineSegment TransformLineSegmentToCS(LineSegment ls, CoordinateSystem cs)
		{
			Matrix transformationMatrix = MatrixFactory.ToCoordinateSystem(cs);

			LineSegment newLS = new LineSegment(new Point(transformationMatrix.Transform(ls.Point1)), new Point(transformationMatrix.Transform(ls.Point2)));
			return newLS;
		}

		// соритировка отрезков от максимально приближенного к точке до удаленного

		public static ArrayList SortArrayLS(ArrayList arr, Point p)
		{
			ArrayList newArr = new ArrayList(arr);
			LineSegment minLS = new LineSegment((arr[0] as LineSegment).Point1, (arr[0] as LineSegment).Point2);
			int i = 0;
			foreach (LineSegment ls in arr)
			{
				Point p1_r = new Point(ls.Point1);
				Point p2_r = new Point(ls.Point1);
				Point 
				if (true)
				{
				
				}
			}
			return newArr;
		}
    }
}
