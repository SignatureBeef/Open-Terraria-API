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
using System;

namespace Microsoft.Xna.Framework.Graphics.PackedVector
{
    public struct HalfVector2 : IPackedVector<uint>, IPackedVector, IEquatable<HalfVector2>
    {
        private uint packedValue;

        [CLSCompliant(false)]
        public uint PackedValue
        {
            get
            {
                return this.packedValue;
            }
            set
            {
                this.packedValue = value;
            }
        }

        public HalfVector2(float x, float y)
        {
            this.packedValue = HalfVector2.PackHelper(x, y);
        }

        public HalfVector2(Vector2 vector)
        {
            this.packedValue = HalfVector2.PackHelper(vector.X, vector.Y);
        }

        public void PackFromVector4(Vector4 vector)
        {
            this.packedValue = HalfVector2.PackHelper(vector.X, vector.Y);
        }

        private static uint PackHelper(float vectorX, float vectorY)
        {
            uint num = (uint)HalfUtils.Pack(vectorX);
            uint num2 = (uint)((uint)HalfUtils.Pack(vectorY) << 16);
            return num | num2;
        }

        public Vector2 ToVector2()
        {
            Vector2 result;
            result.X = HalfUtils.Unpack((ushort)this.packedValue);
            result.Y = HalfUtils.Unpack((ushort)(this.packedValue >> 16));
            return result;
        }

        public Vector4 ToVector4()
        {
            Vector2 vector = this.ToVector2();
            return new Vector4(vector.X, vector.Y, 0f, 1f);
        }

        public override string ToString()
        {
            return this.ToVector2().ToString();
        }

        public override int GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is HalfVector2 && this.Equals((HalfVector2)obj);
        }

        public bool Equals(HalfVector2 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        public static bool operator ==(HalfVector2 a, HalfVector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(HalfVector2 a, HalfVector2 b)
        {
            return !a.Equals(b);
        }
    }
}
