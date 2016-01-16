using System;
using Microsoft.Xna.Framework;
using OTA.Misc;

namespace OTA.Data
{
    /// <summary>
    /// Generic encoding for some OTA functions
    /// </summary>
    public static class DataEncoding
    {
        public const Int32 BitsPerByte = 8;

        /// <summary>
        /// Encodes a color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="color">Color.</param>
        public static uint EncodeColor(Color color)
        {
            return color.PackedValue;
        }

        /// <summary>
        /// Encodes a color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        public static uint EncodeColor(int r, int g, int b, int a = 255)
        {
            return ColorHelper.PackHelper(r, g, b, a);
        }

        /// <summary>
        /// Decodes a color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="color">Color.</param>
        public static Color DecodeColor(uint color)
        {
            var col = default(Color);
            col.PackedValue = color;
            return col;
        }

        /// <summary>
        /// Decodes bits into an array.
        /// </summary>
        /// <returns>The bits.</returns>
        /// <param name="data">Data.</param>
        public static bool[] DecodeBits(byte data)
        {
            var bits = new bool[BitsPerByte];
            for (int i = 0; i < bits.Length; i++)
                bits[i] = (data & 1 << i) != 0;

            return bits;
        }

        /// <summary>
        /// Decodes bits into an array.
        /// </summary>
        /// <returns>The bits.</returns>
        /// <param name="data">Data.</param>
        public static bool[] DecodeBits(short data)
        {
            var bits = new bool[2 * BitsPerByte];
            for (int i = 0; i < bits.Length; i++)
                bits[i] = (data & 1 << i) != 0;

            return bits;
        }

        /// <summary>
        /// Decodes bits into an array.
        /// </summary>
        /// <returns>The bits.</returns>
        /// <param name="data">Data.</param>
        public static bool[] DecodeBits(int data)
        {
            var bits = new bool[4 * BitsPerByte];
            for (int i = 0; i < bits.Length; i++)
                bits[i] = (data & 1 << i) != 0;

            return bits;
        }

        /// <summary>
        /// Encodes to a byte.
        /// </summary>
        /// <returns>The byte.</returns>
        /// <param name="bits">Bits.</param>
        public static byte EncodeByte(bool[] bits)
        {
            if (bits == null) return 0;
            if (bits.Length >= 0 && bits.Length <= 8)
            {
                byte value = 0;
                for (int i = 0; i < bits.Length; i++)
                {
                    if (bits[i]) value |= (byte)(1 << i);
                }

                return value;
            }
            else throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Encodes to a short.
        /// </summary>
        /// <returns>The short.</returns>
        /// <param name="bits">Bits.</param>
        public static short EncodeShort(bool[] bits)
        {
            if (bits == null) return 0;
            if (bits.Length >= 0 && bits.Length <= 16)
            {
                short value = 0;
                for (int i = 0; i < bits.Length; i++)
                {
                    if (bits[i]) value |= (short)(1 << i);
                }

                return value;
            }
            else throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Encodes to a integer.
        /// </summary>
        /// <returns>The integer.</returns>
        /// <param name="bits">Bits.</param>
        public static int EncodeInteger(bool[] bits)
        {
            if (bits == null) return 0;
            if (bits.Length >= 0 && bits.Length <= 32)
            {
                int value = 0;
                for (int i = 0; i < bits.Length; i++)
                {
                    if (bits[i]) value |= (1 << i);
                }

                return value;
            }
            else throw new ArgumentOutOfRangeException();
        }
    }
}