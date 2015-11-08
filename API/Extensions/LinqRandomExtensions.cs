using System;
using System.Collections.Generic;
using System.Linq;

namespace OTA.Extensions
{
    /// <summary>
    /// Contains some Linq extensions that use the <see cref="System.Random"/> class
    /// </summary>
    public static class LinqExtensions
    {
        static readonly Random _rand = new Random();

        /// <summary>
        /// Selects a random item from the list
        /// </summary>
        /// <param name="enumerable">Enumerable.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            var count = list.Count;
            if (count == 0)
                return default(T);
            return list.ElementAt(_rand.Next(0, count));
        }

        /// <summary>
        /// Shuffle the specified data.
        /// </summary>
        /// <remarks>Based on https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle</remarks>
        /// <param name="data">Data.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Shuffle<T>(this T[] data)
        {
            var n = data.Length;  
            while (n > 1)
            {  
                n--;  
                var j = _rand.Next(n + 1);
                T value = data[j];  
                data[j] = data[n];  
                data[n] = value;  
            }  

            return data;
        }
    }
}

