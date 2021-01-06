using AspnetCoreExtensions.CacheBasies.Hash;
using AspnetCoreExtensions.QueryBasies;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspnetCoreExtensions.CacheBasies
{
    public static class MemoryCacheExtensions
    {
        public static string CacheKey<TBusinessEntity>(this IMemoryCache cache, object id)
        {
            return string.Concat(typeof(TBusinessEntity).Name, "/", cache.GetGeneration<TBusinessEntity>(), "/", id);
        }

        public static string CacheKey<TBusinessEntity>(this IMemoryCache cache, string name, object value)
        {
            int generation = cache.GetGeneration<TBusinessEntity>();

            if (generation == 1)
                generation = cache.NextGeneration<TBusinessEntity>();

            return string.Concat(typeof(TBusinessEntity).Name, "/", generation, "/", name, "=", value);
        }

        public static string CacheKey<TBusinessEntity>(this IMemoryCache cache, IQueryConstraints<TBusinessEntity> constraints)
            where TBusinessEntity : class
        {
            Expression predicate = constraints.Predicate;

            string pagePart = $"Page[{constraints.PageNumber}, {constraints.PageSize}]";

            string includePart = constraints.IncludePaths.Any()
                ? $"Include[{constraints.IncludePaths.Aggregate((c, n) => c + ", " + n)}]"
                : "";

            string sortPart = constraints.SortOrder.Any()
                ? $"Sort[{constraints.SortOrder.Select(x => x.SortPropertyName + " " + x.SortOrder.ToString()).Aggregate((c, n) => c + ", " + n)}]"
                : "";

            StringBuilder key = new StringBuilder();
            key.Append(pagePart);

            if (string.IsNullOrEmpty(includePart) == false)
                key.Append(", " + includePart);

            if (string.IsNullOrEmpty(sortPart) == false)
                key.Append(", " + sortPart);

            if (constraints.Predicate != null)
                key.Append(", " + $"Where[{GetHash(predicate)}]");

            return string.Concat(typeof(TBusinessEntity).Name, "/", cache.GetGeneration<TBusinessEntity>(), "/", key);
        }

        private static int GetGeneration<TBusinessEntity>(this IMemoryCache cache)
        {
            int generation;

            if (!cache.TryGetValue(GetGenerationKey<TBusinessEntity>(), out generation))
            {
                generation = 1;
                cache.Set(GetGenerationKey<TBusinessEntity>(), generation);
            }

            return generation;
        }

        private static string GetGenerationKey<TBusinessEntity>()
        {
            return $"{typeof(TBusinessEntity).Name}/Generation";
        }

        public static int NextGeneration<TBusinessEntity>(this IMemoryCache cache)
        {
            var key = GetGenerationKey<TBusinessEntity>();

            if (cache.TryGetValue<int>(key, out int value))
            {
                value++;
                cache.Set(key, value);
            }
            else
                cache.Set(key, 1);

            return cache.Get<int>(key);
        }

        // special thanks to Pete Montgomery's post here: http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/
        private static string GetHash(Expression expression)
        {
            if (expression == null)
                return null;

            // locally evaluate as much of the query as possible
            expression = Evaluator.PartialEval(expression);

            // support local collections
            expression = LocalCollectionExpander.Rewrite(expression);

            // use the string representation of the expression for the cache key
            return expression.ToString();
        }
    }
}
