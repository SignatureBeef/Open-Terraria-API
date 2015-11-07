using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace OTA.Data.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Page the specified query
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="page">Page.</param>
        /// <param name="size">Size.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <remarks>To prevent the query being pulled to memory</remarks>
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int size)
        {
            Contract.Requires(page >= 0);
            Contract.Requires(size > 0);

            return query.Skip(page * size).Take(size);
        }

        /// <summary>
        /// Page the specified data
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="page">Page.</param>
        /// <param name="size">Size.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> data, int page, int size)
        {
            Contract.Requires(page >= 0);
            Contract.Requires(size > 0);

            return data.Skip(page * size).Take(size);
        }
    }
}

