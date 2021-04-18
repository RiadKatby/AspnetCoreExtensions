using AspnetCoreExtensions.EntityBasies;
using AspnetCoreExtensions.QueryBasies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Solitaires.IdentityApp.Cores
{
    /// <summary>
    /// Abstract class for all kind of SearchCriteria classes, it is inhertis from <see cref="ValidatableEntity"/> as well.
    /// </summary>
    public abstract class SearchCriteria : ValidatableEntity
    {
        /// <summary>
        /// The size of page that is intended to be retrieved.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The number of page to be retrieved.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The property name to sort with
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// The sort direction
        /// </summary>
        public SortOrderEnum SortDirection { get; set; }

        public virtual StringBuilder CacheKey()
        {
            string pagePart = $"Page[{PageNumber}, {PageSize}]";

            string sortPart = !string.IsNullOrWhiteSpace(Sort)
              ? $"Sort[{Sort} {SortDirection}]"
              : "";

            StringBuilder key = new StringBuilder();
            key.Append(pagePart);

            if (string.IsNullOrEmpty(sortPart) == false)
                key.Append(", " + sortPart);

            List<string> whereBuilder = new List<string>();
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.DeclaringType != typeof(SearchCriteria) && x.DeclaringType != typeof(ValidatableEntity))
                .ToList();

            foreach (var item in properties)
                whereBuilder.Add(string.Format("({0} = '{1}')", item.Name, item.GetValue(this)));

            key.AppendFormat(", Where[{0}]", string.Join(", ", whereBuilder));

            return key;
        }

        protected SearchCriteria()
        {
            PageSize = 10;
            PageNumber = 1;
        }

    }
}
