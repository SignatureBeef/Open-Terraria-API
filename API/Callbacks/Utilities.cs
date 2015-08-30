using System;

namespace OTA.Callbacks
{
    /// <summary>
    /// Callbacks from vanilla code for Terraria.Utilities
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Vanilla replacement to correctly handle cross platform of removing files
        /// </summary>
        /// <returns><c>true</c>, if file was removed, <c>false</c> otherwise.</returns>
        /// <param name="path">Path.</param>
        public static bool RemoveFile(string path)
        {
            if (Tools.RuntimePlatform == RuntimePlatform.Mono)
            {
                try
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            #if Full_API
            return Terraria.Utilities.FileOperationAPIWrapper.MoveToRecycleBin(path);
            #else
            return false;
            #endif
        }
    }

    /// <summary>
    /// A checked random instance
    /// </summary>
    /// <remarks>This is mainly used to fix a bug in Terraria.Player.Ghost</remarks>
    // [TODO, enumer-ize this]
    public class Rand : Random
    {
        /// <summary>
        /// Ensures there is no exception, if maxValue is lower then zero the random number will return as 0
        /// </summary>
        /// <param name="maxValue">Max value.</param>
        public override int Next(int maxValue)
        {
            if (maxValue <= 0)
                return 0;
            return base.Next(maxValue);
        }

        /// <summary>
        /// Generates a random number between min/maxValue
        /// It ensures the minValue is higher than 0 and maxValue is larger than minValue
        /// </summary>
        /// <param name="minValue">Minimum value.</param>
        /// <param name="maxValue">Max value.</param>
        public override int Next(int minValue, int maxValue)
        {
            if (minValue > 0 && minValue < maxValue)
                return base.Next(minValue, maxValue);
            return 0;
        }
    }
}

