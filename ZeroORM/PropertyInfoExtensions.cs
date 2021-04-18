using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ZeroORM
{
    internal static class PropertyInfoExtensions
    {
        private static ConcurrentDictionary<string, PropertyInfo> cacheOfProperties = new ConcurrentDictionary<string, PropertyInfo>();

        public static PropertyInfo GetProperty<T>(string propertyName)
        {
            Type entityType = typeof(T);
            return GetProperty(entityType, propertyName);

        }

        public static PropertyInfo GetProperty(Type entityType, string propertyName)
        {
            string cacheKey = entityType.FullName + "." + propertyName;
            if (!cacheOfProperties.TryGetValue(cacheKey, out PropertyInfo property))
            {
                property = entityType.GetProperty(propertyName);
                if (property == null)
                    return null;

                cacheOfProperties[cacheKey] = property;
            }

            return property;
        }
    }
}
