using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreExtensions.QueryBasies
{
    /// <summary>
    /// Search result implementation
    /// </summary>
    /// <typeparam name="T">Model type (i.e. denormalized row)</typeparam>
    public class QueryResult<T> : IQueryResult<T> where T : class
    {
        /// <summary>
        /// Gets all matching items
        /// </summary>
        public IEnumerable<T> Items { get; }

        /// <summary>
        /// Gets total number of items (useful when paging is used, otherwise 0)
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="totalCount">The total count (if paging is used, otherwise <c>0</c>).</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public QueryResult(IEnumerable<T> items, int totalCount)
        {
            if (totalCount < 0)
                throw new ArgumentOutOfRangeException(nameof(totalCount), totalCount, "Incorrect value.");

            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
        }

        public static IQueryResult<T> Empty()
        {
            return new QueryResult<T>(Array.Empty<T>(), 0);
        }

        public static IQueryResult<T> Single(T entity)
        {
            return new QueryResult<T>(new T[] { entity }, 1);
        }
    }
}
