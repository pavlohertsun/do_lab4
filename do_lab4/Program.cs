using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace do_lab4
{
    public class MyProgram
    {

        static int rowsAmount, columnsAmount;
        static TableElement[,] Table;
        static double[] Supplies;
        static double[] Demands;
        static double[] alphaArray;
        static double[] betaArray;
        static List<TableElement> Cycle;
        static Direction[] Directions;
        static Direction DirectionStart;
        static int Width = 5;
        static bool IsDirectionFound = false;

        public static void Main()
        {
            Console.Write("Enter amount of rows: ");
            rowsAmount = int.Parse(Console.ReadLine());
            Console.Write("Enter amount of columns: ");
            columnsAmount = int.Parse(Console.ReadLine());
            Table = new TableElement[rowsAmount, columnsAmount];
            Supplies = new double[rowsAmount];
            Demands = new double[columnsAmount];
            Console.WriteLine();
            int index = 1;
            string[] rowString = new string[rowsAmount];
            for (int i = 0; i < rowsAmount; i++)
            {
                for (int j = 0; j < columnsAmount; j++)
                {
                    Console.Write("Enter costs for row #{0} element #{1}: ", index, j + 1);
                    rowString[j] = Console.ReadLine();
                }
                index++;
                for (int j = 0; j < columnsAmount; j++)
                {
                    TableElement temp = new TableElement();
                    temp.Cost = double.Parse(rowString[j]);
                    Table[i, j] = temp;
                }
            }
            Console.WriteLine();
            Console.Write("Enter supplies: \n");
            string[] supplyString = new string[rowsAmount];
            for (int i = 0; i < rowsAmount; i++)
            {
                Console.Write("Enter suply #{0} : ", i + 1);
                supplyString[i] = Console.ReadLine();
                Supplies[i] = double.Parse(supplyString[i]);
            }
            Console.Write("Enter demands: \n");
            string[] demandString = new string[rowsAmount];
            for (int i = 0; i < columnsAmount; i++)
            {
                Console.Write("Enter demand #{0} : ", i + 1);
                demandString[i] = Console.ReadLine();
                Demands[i] = double.Parse(demandString[i]);
            }

            alphaArray = new double[rowsAmount];
            betaArray = new double[columnsAmount];
            Cycle = new List<TableElement>();
            Directions = new Direction[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

            PotentialsMethod();

            return;
        }
        static void PotentialsMethod()
        {

            int index = 0;
            NorthWestCorner();

            while (true)
            {

                GetPotentials();
                Console.WriteLine("\nIteration #{0}", index + 1);
                PrintTable();

                if (IsDegenerate())
                {
                    Console.WriteLine("This problem is degenerate.\nCannot perform solving with Potentials method.\n");
                    return;
                }

                if (IsOptimal())
                {
                    Console.WriteLine("\n\nOptimal solution found.\nZ = {0}", GetSum());
                    return;
                }
                else
                {
                    StartCycle();
                    GetCycle();
                    Cycle.Clear();
                    ClearDirection();
                    index++;
                }
            }
        }
        static void PrintTable()
        {

            Console.WriteLine();
            for (int i = 0; i < columnsAmount * 2; i++)
            {
                Console.Write("-----------");
            }
            Console.WriteLine();
            Console.Write("A / B\t");
            for (int i = 0; i < columnsAmount; i++)
            {

                Console.Write("B".PadLeft(Width * 2 - 1) + (i + 1).ToString() + " = " + Math.Round(betaArray[i], 2).ToString("F1"));
            }
            Console.WriteLine("\tSupplies");
            for (int i = 0; i < columnsAmount * 2; i++)
            {

                Console.Write("-----------");
            }
            for (int i = 0; i < rowsAmount; i++)
            {

                Console.WriteLine();
                Console.WriteLine("A" + (i + 1).ToString() + " = " + Math.Round(alphaArray[i], 2).ToString("F1") + "\t");

                for (int j = 0; j < columnsAmount; j++)
                {

                    Console.Write("\t".PadRight(Width * 2 - 1) + Math.Round(Table[i, j].Cost, 2).ToString("F1"));
                }
                Console.WriteLine("\t".PadRight(Width * 2 + 1) + Supplies[i].ToString("F1"));

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (Table[i, j].IsMarked)
                    {

                        Console.Write("\t");
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("".PadRight(Width * 2 - 1) + Math.Round(Table[i, j].Value, 2).ToString("F1"));
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    else
                    {

                        Console.Write("\t");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("".PadRight(Width * 2 - 1) + Math.Round(Table[i, j].Value, 2).ToString("F1"));
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();

                for (int k = 0; k < columnsAmount * 2; k++)
                {

                    Console.Write("-----------");
                }
            }
            Console.Write("\nDemands\t");
            for (int i = 0; i < columnsAmount; i++)
            {

                Console.Write("\t" + Math.Round(Demands[i], 2).ToString("F1") + "\t");
            }
            Console.WriteLine();
            for (int k = 0; k < columnsAmount * 2; k++)
            {

                Console.Write("-----------");
            }
        }
        static void NorthWestCorner()
        {

            double[] TempSupplies = new double[rowsAmount];
            Array.Copy(Supplies, TempSupplies, rowsAmount);
            double[] TempDemands = new double[columnsAmount];
            Array.Copy(Demands, TempDemands, columnsAmount);

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (TempSupplies[i] > 0 && TempDemands[j] > 0)
                    {

                        double quantity = Math.Min(TempSupplies[i], TempDemands[j]);

                        Table[i, j].Value = quantity;
                        Table[i, j].IsMarked = true;
                        TempSupplies[i] -= quantity;
                        TempDemands[j] -= quantity;
                    }
                }
            }
        }
        static Direction FindMaxValueToStartCycle()
        {

            double Max = Double.MinValue;
            Direction StartDirection = new Direction(0, 0);

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (!Table[i, j].IsMarked && Table[i, j].Value > Max)
                    {

                        Max = Table[i, j].Value;
                        StartDirection.X = j;
                        StartDirection.Y = i;
                    }
                }
            }
            return StartDirection;
        }
        static void StartCycle()
        {

            IsDirectionFound = false;

            Cycle.Clear();
            DirectionStart = FindMaxValueToStartCycle();

            int x = DirectionStart.X;
            int y = DirectionStart.Y;

            Table[y, x].IsStart = true;

            Cycle.Add(Table[y, x]);

            for (int q = 0; q < Directions.Length; q++)
            {
                if (!IsDirectionFound)
                {
                    GetNextStep(y, x, Directions[q], Direction.None);
                }
            }
        }
        static void GetNextStep(int i, int j, Direction current, Direction previous)
        {

            while ((i + current.Y >= 0 && i + current.Y <= rowsAmount - 1) && // перевіряємо на вихід за межі таблиці
                (j + current.X >= 0 && j + current.X <= columnsAmount - 1))
            {

                if (IsDirectionFound)
                {

                    return;
                }

                Table[i, j].CycleDirection.Y = previous.Y;
                Table[i, j].CycleDirection.X = previous.X;

                i += current.Y;
                j += current.X;

                if (DirectionStart.X == j && DirectionStart.Y == i)
                {

                    IsDirectionFound = true;
                    return;
                }

                if (Table[i, j].IsMarked && !Cycle.Contains(Table[i, j]))
                { // перевіряємо чи заповнена нова клітинка

                    Cycle.Add(Table[i, j]);

                    for (int k = 0; k < Directions.Length; k++)
                    {

                        if (CheckDirection(current, Directions[k]) && !IsDirectionFound)
                        {

                            GetNextStep(i, j, Directions[k], current);
                        }
                    }

                    if (!IsDirectionFound)
                    {

                        Cycle.Remove(Cycle.Last());
                    }
                }
            }

            return;
        }
        static void GetCycle()
        {

            Console.WriteLine();

            for (int i = 1; i < Cycle.Count - 1; i++)
            { // видалення зайвих клітинок у циклі

                if (Cycle[i].CycleDirection.Equals(Cycle[i + 1].CycleDirection))
                {

                    Cycle.Remove(Cycle[i]);
                    i = 0;
                }
            }

            Direction Start = FindMaxValueToStartCycle();
            int Count = Cycle.Count;
            int i1 = 0, j1 = 0, i2 = 0, j2 = 0;

            for (int k = 0; k < rowsAmount; k++)
            {

                for (int l = 0; l < columnsAmount; l++)
                {

                    if (Cycle[Count - 1].Cost == Table[k, l].Cost
                        && Cycle[Count - 1].Value == Table[k, l].Value)
                    {

                        i1 = k;
                        j1 = l;

                        break;
                    }
                }
            }

            for (int k = 0; k < rowsAmount; k++)
            {

                for (int l = 0; l < columnsAmount; l++)
                {

                    if (Cycle[Count - 2].Cost == Table[k, l].Cost
                        && Cycle[Count - 2].Value == Table[k, l].Value)
                    {

                        i2 = k;
                        j2 = l;

                        break;
                    }
                }
            }

            if ((Start.Y == i1 && i1 == i2) || (Start.X == j1 && j1 == j2))
            {

                Cycle.RemoveAt(Count - 1);
            }

            Console.WriteLine("Cycle: ");
            for (int i = 0; i < Cycle.Count; i++)
            {

                if (i % 2 == 0)
                {

                    Console.Write(Cycle[i].Value.ToString() + " (+) --> ");
                }
                else
                {

                    Console.Write(Cycle[i].Value.ToString() + " (-) --> ");
                }

            }
            Console.Write(Cycle[0].Value.ToString());
            Console.WriteLine();

            double Min = double.MaxValue;
            TableElement MinCell = new TableElement();


            for (int i = 0; i < Cycle.Count; i++)
            {

                if (i % 2 != 0 && Cycle[i].Value < Min)
                {

                    Min = Cycle[i].Value;
                    MinCell = Cycle[i];
                }
            }

            Cycle[0].Value = Min;

            for (int i = 1; i < Cycle.Count; i++)
            {

                if (i % 2 == 0)
                {

                    Cycle[i].Value += Min;

                }
                else
                {

                    Cycle[i].Value -= Min;
                }
            }

            Table[DirectionStart.Y, DirectionStart.X].IsMarked = true;
            Cycle[Cycle.IndexOf(MinCell)].IsMarked = false;
        }
        static void ClearDirection()
        {

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    Table[i, j].CycleDirection.Clear();
                }
            }
        }
        static bool CheckDirection(Direction obj1, Direction obj2)
        {

            if (obj1.X != -obj2.X || obj1.Y != -obj2.Y)
            {

                return true;
            }

            return false;
        }
        static void GetPotentials()
        {

            alphaArray[0] = 0;
            for (int i = 1; i < rowsAmount; i++)
            {

                alphaArray[i] = double.NaN;
            }

            betaArray[0] = double.NaN;
            for (int j = 1; j < columnsAmount; j++)
            {

                betaArray[j] = double.NaN;
            }

            bool isAlphaSet, isBetaSet;

            do
            {

                isAlphaSet = false;
                isBetaSet = false;

                for (int i = 0; i < rowsAmount; i++)
                {

                    for (int j = 0; j < columnsAmount; j++)
                    {

                        if (Table[i, j].IsMarked)
                        {

                            if (!double.IsNaN(alphaArray[i]) && double.IsNaN(betaArray[j]))
                            {

                                betaArray[j] = Table[i, j].Cost - alphaArray[i];
                                isBetaSet = true;

                            }
                            else if (double.IsNaN(alphaArray[i]) && !double.IsNaN(betaArray[j]))
                            {

                                alphaArray[i] = Table[i, j].Cost - betaArray[j];
                                isAlphaSet = true;
                            }
                        }
                    }
                }

            } while (isAlphaSet || isBetaSet);

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (!Table[i, j].IsMarked)
                    {

                        Table[i, j].Value = (alphaArray[i] + betaArray[j]) - Table[i, j].Cost;
                    }
                }
            }
        }
        static double GetSum()
        {

            double sum = 0;

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (Table[i, j].IsMarked)
                    {

                        sum += Table[i, j].Cost * Table[i, j].Value;
                    }
                }
            }
            return sum;
        }
        static bool IsDegenerate()
        {

            int Counter = 0;

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (Table[i, j].IsMarked)
                        Counter++;
                }
            }

            if (Counter == rowsAmount + columnsAmount - 1)
            {

                return true;
            }
            else
            {

                return false;
            }
        }
        static bool IsOptimal()
        {

            for (int i = 0; i < rowsAmount; i++)
            {

                for (int j = 0; j < columnsAmount; j++)
                {

                    if (!Table[i, j].IsMarked && Table[i, j].Value > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
