using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace do_lab4
{
    public class Direction
    {

        public int X { get; set; }
        public int Y { get; set; }

        public static Direction None = new Direction(0, 0);
        public static Direction Up = new Direction(0, -1);
        public static Direction Down = new Direction(0, 1);
        public static Direction Left = new Direction(-1, 0);
        public static Direction Right = new Direction(1, 0);

        public Direction(int x, int y)
        {

            this.X = x;
            this.Y = y;
        }
        public void Clear()
        {

            this.X = 0;
            this.Y = 0;
        }
        public override bool Equals(object obj)
        {

            return Equals(obj as Direction);
        }
        public bool Equals(Direction other)
        {

            return other != null && other.X == X && other.Y == Y;
        }
        public override int GetHashCode()
        {

            return HashCode.Combine(X, Y);
        }

    }
    public class TableElement
    {

        public double Cost;
        public double Value;
        public bool IsMarked;
        public bool IsStart;
        public Direction CycleDirection = new Direction(0, 0);
    }
}
