using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ZeroORM
{
    public static class SqlParameterExtensions
    {
        /// <summary>
        /// Create <see cref="SqlParameter"/> for the specified property defined by <paramref name="expression"/>
        /// </summary>
        /// <param name="expression">specific property used to create <see cref="SqlParameter"/> instance.</param>
        /// <returns>New <see cref="SqlParameter"/> instance with name of PropertyName and value of the Property itself.</returns>
        public static SqlParameter ToParameter<T, U>(this T item, Expression<Func<T, U>> expression, string dbColumnName)
        {
            var member = expression.Body as MemberExpression;

            var columnName = member.Member.Name;
            var itemType = member.Expression.Type;

            PropertyInfo property = PropertyInfoExtensions.GetProperty(itemType, columnName);

            if (property != null)
            {
                var dbValue = property.GetValue(item).ToDbValue();
                return new SqlParameter("@" + (dbColumnName ?? columnName), dbValue);
            }

            return null;
        }

        /// <summary>
        /// Create <see cref="SqlParameter"/> for the specified property defined by <paramref name="expression"/>
        /// </summary>
        /// <param name="expression">specific property used to create <see cref="SqlParameter"/> instance.</param>
        /// <returns>New <see cref="SqlParameter"/> instance with name of PropertyName and value of the Property itself.</returns>
        public static SqlParameter ToParameter<T, U>(this T item, Expression<Func<T, U>> expression)
        {
            return ToParameter(item, expression, null);
        }

        /// <summary>
        /// Create <see cref="SqlParameter"/> for the specified property defined by <paramref name="expression"/>
        /// </summary>
        /// <typeparam name="T">Business entity type parameter</typeparam>
        /// <typeparam name="U">Property value type parameter</typeparam>
        /// <param name="item">business entity instance</param>
        /// <param name="expression">specific property used to create <see cref="SqlParameter"/> instance.</param>
        /// <returns>New <see cref="SqlParameter"/> instance with name of PropertyName and value of the Property itself.</returns>
        public static SqlParameter ToLikeParameter<T, U>(this T item, Expression<Func<T, U>> expression)
        {
            var member = expression.Body as MemberExpression;

            var columnName = member.Member.Name;
            var itemType = member.Expression.Type;

            PropertyInfo property = PropertyInfoExtensions.GetProperty(itemType, columnName);

            if (property != null)
            {
                var dbValue = property.GetValue(item).ToDbValue();
                return new SqlParameter("@" + columnName, "%" + dbValue + "%");
            }

            return null;
        }

        public static T SetParameter<T, U>(this T item, Expression<Func<T, U>> expression, SqlCommand cmd)
        {
            var member = expression.Body as MemberExpression;

            var columnName = member.Member.Name;
            var itemType = member.Expression.Type;

            PropertyInfo property = PropertyInfoExtensions.GetProperty(itemType, columnName);

            if (property != null)
            {
                var dbValue = property.GetValue(item).ToDbValue();
                cmd.Parameters.AddWithValue(columnName, dbValue);
            }

            return item;
        }

        /// <summary>
        /// Return value if not null, DbNull.Value otherwise.
        /// </summary>
        /// <param name="value">value to be passed into Database.</param>
        /// <returns>Same value if is not null, DbNull otherwise.</returns>
        public static object ToDbValue(this object value)
        {
            if (value == null)
                return DBNull.Value;
            else
                return value;
        }
    }
}
