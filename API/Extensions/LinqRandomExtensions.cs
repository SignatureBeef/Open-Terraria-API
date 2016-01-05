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
        private static readonly Random _rand = new Random();

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

        /// <summary>
        /// Finds a random element using a chance value as weighting
        /// </summary>
        /// <returns>The random element.</returns>
        /// <param name="list">List.</param>
        /// <param name="fncChance">Fnc chance.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T WeightedRandom<T>(this System.Collections.Generic.IEnumerable<T> list, Func<T, double> fncChance)
        {
            var totalWeight = 0.0;
            T selection = default(T);

            foreach (var item in list)
            {
                //Calculate how many chances this item has
                var chance = fncChance(item);
                //If there are zero chances then move on to the next item
                if (chance <= 0) continue;

                /*
                     * Calculate the random item weight. If the random item weight is in the chance range then use it.
                     * e.g.
                     *  [totalWeight]   + [chance]  = [current weight range | chance range]
                     *  123             + 4         = 127
                     *  if [randWeight] >= 123 && [randWeight] <= 127 then the chance suceeded
                     *  else do nothing and continue to the next item (can be an unordered list)
                     */
                var randWeight = _rand.Next((int)((totalWeight + chance) * 100.0)) / 100.0;
                if (randWeight >= totalWeight) selection = item;

                totalWeight += chance;
            }

            //If there are multiple items for the same chance then we must find a random element
            //otherwise it's possible that only one of these items will ever be used.
            if (!selection.Equals(default(T)))
            {
                var matches = list.Where(x => fncChance(x) == fncChance(selection));
                if (matches.Count() > 1)
                {
                    var index = _rand.Next(0, matches.Count());
                    selection = matches.ElementAt(index);
                }
            }

            return selection;
        }
    }
}

