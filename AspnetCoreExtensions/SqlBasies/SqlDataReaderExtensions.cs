using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace AspnetCoreExtensions.SqlBasies
{
    public static class SqlDataReaderExtensions
    {
        /// <summary>
        /// Create new instance of entityType and fill up all properties from specified <see cref="SqlDataReader"/> instance.
        /// </summary>
        /// <param name="reader">Instance of SqlDataReader.</param>
        /// <param name="entityType">Business Entity type.</param>
        /// <returns>New instance of T with properties are filled up.</returns>
        public static object ToEntity(this SqlDataReader reader, Type entityType)
        {
            var entity = Activator.CreateInstance(entityType);
            return reader.SetValues(entity, null);
        }

        /// <summary>
        /// Create new instance of <typeparamref name="T"/> and fill up all properties from specified <see cref="SqlDataReader"/>.
        /// </summary>
        /// <typeparam name="T">Business Entity Type.</typeparam>
        /// <param name="reader">Instance of SqlDataReader.</param>
        /// <returns>New instance of T with properties are filled up.</returns>
        public static T ToEntity<T>(this SqlDataReader reader, Action<T> postSet = null)
            where T : new()
        {
            var entityType = typeof(T);
            var entity = (T)Activator.CreateInstance(entityType);

            return reader.SetValues<T>(entity, postSet);
        }

        public static List<T> ToList<T>(this SqlDataReader reader, Action<T> postSet = null)
            where T : new()
        {
            List<T> items = new List<T>();
            while (reader.Read())
                items.Add(reader.ToEntity<T>(postSet));

            return items;
        }

        /// <summary>
        /// Set values of specified entity's properties from specified <see cref="SqlDataReader"/> instance.
        /// </summary>
        /// <typeparam name="T">Business Entity Type.</typeparam>
        /// <param name="reader">Instance of SqlDataReader.</param>
        /// <param name="entity">instance of business entity.</param>
        /// <returns>Same instance of business entity with properties are filled up.</returns>
        public static T SetValues<T>(this SqlDataReader reader, T entity, Action<T> postSet = null)
        {
            var entityType = entity.GetType();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var dbValue = reader.IsDBNull(i) ? null : reader.GetValue(i);

                var propertyInfo = PropertyInfoExtensions.GetProperty(entityType, columnName);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(entity, dbValue);
                }
                else
                    Debug.WriteLine("DbColumn[{0} = {1}] has no mapping in Entity[{2}]", columnName, dbValue, entityType.Name);
            }

            postSet?.Invoke(entity);

            return entity;
        }
    }
}
