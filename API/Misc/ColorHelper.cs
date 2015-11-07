using System;

namespace OTA.Misc
{
    public static class ColorHelper
    {
        public static uint PackHelper(float vectorX, float vectorY, float vectorZ, float vectorW)
        {
            uint num = PackUNorm(255f, vectorX);
            uint num2 = PackUNorm(255f, vectorY) << 8;
            uint num3 = PackUNorm(255f, vectorZ) << 16;
            uint num4 = PackUNorm(255f, vectorW) << 24;
            return num | num2 | num3 | num4;
        }

        public static uint PackUNorm(float bitmask, float value)
        {
            value *= bitmask;
            return (uint)ClampAndRound(value, 0f, bitmask);
        }

        private static double ClampAndRound(float value, float min, float max)
        {
            if (float.IsNaN(value))
            {
                return 0.0;
            }
            if (float.IsInfinity(value))
            {
                return (double)(float.IsNegativeInfinity(value) ? min : max);
            }
            if (value < min)
            {
                return (double)min;
            }
            if (value > max)
            {
                return (double)max;
            }
            return Math.Round((double)value);
        }
    }
}

