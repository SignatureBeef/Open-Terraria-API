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
    internal static class HalfUtils
    {
        private const int cFracBits = 10;

        private const int cExpBits = 5;

        private const int cSignBit = 15;

        private const uint cSignMask = 32768u;

        private const uint cFracMask = 1023u;

        private const int cExpBias = 15;

        private const uint cRoundBit = 4096u;

        private const uint eMax = 16u;

        private const int eMin = -14;

        private const uint wMaxNormal = 1207955455u;

        private const uint wMinNormal = 947912704u;

        private const uint BiasDiffo = 3355443200u;

        private const int cFracBitsDiff = 13;

        public unsafe static ushort Pack(float value)
        {
            uint num = *(uint*)(&value);
            uint num2 = (num & 2147483648u) >> 16;
            uint num3 = num & 2147483647u;
            ushort result;
            if (num3 > 1207955455u)
            {
                result = (ushort)(num2 | 32767u);
            }
            else if (num3 < 947912704u)
            {
                uint num4 = (num3 & 8388607u) | 8388608u;
                int num5 = (int)(113u - (num3 >> 23));
                num3 = ((num5 > 31) ? 0u : (num4 >> num5));
                result = (ushort)(num2 | num3 + 4095u + (num3 >> 13 & 1u) >> 13);
            }
            else
            {
                result = (ushort)(num2 | num3 + 3355443200u + 4095u + (num3 >> 13 & 1u) >> 13);
            }
            return result;
        }

        public unsafe static float Unpack(ushort value)
        {
            uint num3;
            if (((int)value & -33792) == 0)
            {
                if ((value & 1023) != 0)
                {
                    uint num = 4294967282u;
                    uint num2 = (uint)(value & 1023);
                    while ((num2 & 1024u) == 0u)
                    {
                        num -= 1u;
                        num2 <<= 1;
                    }
                    num2 &= 4294966271u;
                    num3 = (uint)((int)(value & 32768) << 16 | (int)((int)(num + 127u) << 23) | (int)((int)num2 << 13));
                }
                else
                {
                    num3 = (uint)((uint)(value & 32768) << 16);
                }
            }
            else
            {
                num3 = (uint)((int)(value & 32768) << 16 | (value >> 10 & 31) - 15 + 127 << 23 | (int)(value & 1023) << 13);
            }
            return *(float*)(&num3);
        }
    }
}
