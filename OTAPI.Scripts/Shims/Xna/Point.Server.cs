/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
namespace Microsoft.Xna.Framework
{
    public struct Point
    {
        public static Point[] Array;

        public int X;
        public int Y;

        private static readonly Point _zero = new Point();
        public static Point Zero
        {
            get
            {
                return Point._zero;
            }
        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        public override bool Equals(object obj)
        {
            //bool result = false;
            //if (obj is Point)
            //{
            //    result = this.Equals((Point)obj);
            //}
            //return result;

            if (obj is Point target)
            {
                return target.X == this.X && target.Y == this.Y;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }
    }
}