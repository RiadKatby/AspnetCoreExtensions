using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroORM
{
    public static class SqlDataReaderExtensions
    {
        /// <summary>
        /// Map first row of <see cref="SqlDataReader"/> into new instance of <typeparamref name="T"/> if <see cref="SqlDataReader.HasRows"/>, Null otherwise.
        /// </summary>
        /// <param name="reader">Instance of SqlDataReader.</param>
        /// <param name="entityType">Business Entity type.</param>
        /// <returns>New instance of T with properties are filled up.</returns>
        public static T ToEntity<T>(this SqlDataReader reader)
            where T : new()
        {
            return ToEntity<T>(reader, null);
        }

        /// <summary>
        /// Map first row of <see cref="SqlDataReader"/> into new instance of <typeparamref name="T"/> if <see cref="SqlDataReader.HasRows"/>, Null otherwise.
        /// </summary>
        /// <typeparam name="T">Business Entity Type.</typeparam>
        /// <param name="reader">Instance of SqlDataReader.</param>
        /// <returns>New instance of T with properties are filled up.</returns>
        public static T ToEntity<T>(this SqlDataReader reader, Action<T, SqlDataReader> postSet)
            where T : new()
        {
            var entityType = typeof(T);

            if (reader.Read())
            {
                var entity = (T)Activator.CreateInstance(entityType);
                return reader.SetEntity<T>(entity, postSet);
            }

            return default;
        }

        /// <summary>
        /// Asynchronously map first row of SqlDataReader into new Instance of <typeparamref name="T"/> if <see cref="SqlDataReader.HasRows"/>, Null otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> ToEntityAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken)
            where T : new()
        {
            return await reader.ToEntityAsync<T>(null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously map first row of SqlDataReader into new Instance of <typeparamref name="T"/> if <see cref="SqlDataReader.HasRows"/>, Null otherwise.
        /// </summary>
        public static async Task<T> ToEntityAsync<T>(this SqlDataReader reader, Action<T, SqlDataReader> postSet, CancellationToken cancellationToken)
            where T : new()
        {
            var entityType = typeof(T);

            if (await reader.ReadAsync(cancellationToken))
            {
                var entity = (T)Activator.CreateInstance(entityType);
                return reader.SetEntity(entity, postSet);
            }

            return default;
        }

        /// <summary>
        /// Map all rows of <see cref="SqlDataReader"/> into list of <typeparamref name="T"/>, Empty list otherwise.
        /// </summary>
        public static List<T> ToList<T>(this SqlDataReader reader)
            where T : new()
        {
            List<T> items = new List<T>();
            while (reader.Read())
            {
                var entity = Activator.CreateInstance<T>();
                SetEntity(reader, entity, null);
                items.Add(entity);
            }

            return items;
        }

        /// <summary>
        /// Map all rows of <see cref="SqlDataReader"/> into list of <typeparamref name="T"/>, Empty list otherwise.
        /// </summary>
        public static List<T> ToList<T>(this SqlDataReader reader, Action<T, SqlDataReader> postSet)
            where T : new()
        {
            List<T> items = new List<T>();
            while (reader.Read())
            {
                var entity = Activator.CreateInstance<T>();
                SetEntity(reader, entity, postSet);
                items.Add(entity);
            }

            return items;
        }

        /// <summary>
        /// Asynchronously map all rows of <see cref="SqlDataReader"/> into list of <typeparamref name="T"/>, Empty list otherwise.
        /// </summary>
        public static async Task<List<T>> ToListAsync<T>(this SqlDataReader reader, Action<T, SqlDataReader> postSet, CancellationToken cancellationToken)
            where T : new()
        {
            List<T> items = new List<T>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var item = Activator.CreateInstance<T>();
                SetEntity(reader, item, postSet);
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Asynchronously map all rows of <see cref="SqlDataReader"/> into list of <typeparamref name="T"/>, Empty list otherwise.
        /// </summary>
        public static async Task<List<T>> ToListAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken)
            where T : new()
        {
            return await ToListAsync<T>(reader, null, cancellationToken);
        }

        /// <summary>
        /// Fill up specified entities list will all record in specified instance of SqlDataReader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">SqlDataReader instance to be mapped into List of T</param>
        /// <param name="entities">List of entities that will populated with all entities</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task SetListAsync<T>(this SqlDataReader reader, ICollection<T> entities, CancellationToken cancellationToken)
            where T : new()
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var entity = Activator.CreateInstance<T>();
                reader.SetEntity(entity, null);
                entities.Add(entity);
            }
        }

        public static async Task SetListAsync<T>(this SqlDataReader reader, ICollection<T> entities, Action<T, SqlDataReader> postSet, CancellationToken cancellationToken)
            where T : new()
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var entity = Activator.CreateInstance<T>();
                reader.SetEntity(entity, postSet);
                entities.Add(entity);
            }
        }

        /// <summary>
        /// Set values of specified entity's properties from specified <see cref="SqlDataReader"/> instance.
        /// </summary>
        /// <typeparam name="T">Business Entity Type.</typeparam>
        /// <param name="reader">Instance of SqlDataReader.</param>
        /// <param name="entity">instance of business entity.</param>
        /// <returns>Same instance of business entity with properties are filled up.</returns>
        public static T SetEntity<T>(this SqlDataReader reader, T entity, Action<T, SqlDataReader> postSet = null)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), "reader must not be null.");

            if (reader.IsClosed | !reader.HasRows)
                throw new InvalidOperationException("reader either Closed or Has no rows.");

            if (entity is null)
                throw new ArgumentNullException(nameof(entity), "entity must not be null.");

            var entityType = entity.GetType();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbValue = reader.IsDBNull(i) ? null : reader.GetValue(i);

                var propertyInfo = PropertyInfoExtensions.GetProperty(entityType, columnName);
                if (propertyInfo != null)
                {
                    // https://stackoverflow.com/a/8605677/1726318
                    var nullableType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    if (dbValue != null && nullableType != null && nullableType.IsEnum)
                        dbValue = Enum.ToObject(nullableType, dbValue);

                    propertyInfo.SetValue(entity, dbValue);
                }
                else
                    Debug.WriteLine("DbColumn[{0} = {1}] has no mapping in Entity[{2}]", columnName, dbValue, entityType.Name);
            }

            postSet?.Invoke(entity, reader);

            return entity;
        }

        public static void ReadEach<T>(this SqlDataReader reader, Action<T> action, Action<T, SqlDataReader> postSet)
        {
            while (reader.Read())
            {
                var entity = Activator.CreateInstance<T>();
                SetEntity(reader, entity, postSet);
                action(entity);
            }
        }

        public static void ReadEach<T>(this SqlDataReader reader, Action<T> action)
        {
            while (reader.Read())
            {
                var entity = Activator.CreateInstance<T>();
                SetEntity(reader, entity, null);
                action(entity);
            }
        }

        public static void SetValue<T, U>(this T item, Expression<Func<T, U>> expression, string dbColumnName, SqlDataReader reader)
        {
            var member = expression.Body as MemberExpression;

            string columnName = member.Member.Name;
            Type itemType = member.Expression.Type;

            var dbValue = reader[dbColumnName];
            if (dbValue != DBNull.Value)
            {
                PropertyInfo property = PropertyInfoExtensions.GetProperty(itemType, columnName);
                if (property != null)
                {
                    property.SetValue(item, dbValue);
                }
                else
                    Debug.WriteLine("DbColumn[{0} = {1}] has no mapping in Entity[{2}]", columnName, dbValue, itemType.Name);
            }
        }

        public static T SetValue<T, U>(this T item, Expression<Func<T, U>> expression, object value)
        {
            var member = expression.Body as MemberExpression;

            string columnName = member.Member.Name;
            Type itemType = member.Expression.Type;

            if (value != DBNull.Value)
            {
                PropertyInfo property = PropertyInfoExtensions.GetProperty(itemType, columnName);
                if (property != null)
                {
                    property.SetValue(item, value);
                }
                else
                    Debug.WriteLine("DbColumn[{0} = {1}] has no mapping in Entity[{2}]", columnName, value, itemType.Name);
            }

            return item;
        }

        public static T SetValue<T, U>(this T item, Expression<Func<T, U>> expression, SqlDataReader reader)
        {
            var member = expression.Body as MemberExpression;

            string columnName = member.Member.Name;
            Type itemType = member.Expression.Type;

            var dbValue = reader[columnName];
            if (dbValue != DBNull.Value)
            {
                PropertyInfo property = PropertyInfoExtensions.GetProperty(itemType, columnName);
                if (property != null)
                {
                    property.SetValue(item, dbValue);
                }
                else
                    Debug.WriteLine("DbColumn[{0} = {1}] has no mapping in Entity[{2}]", columnName, dbValue, itemType.Name);
            }

            return item;
        }
    }
}
