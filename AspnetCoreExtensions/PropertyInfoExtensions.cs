using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AspnetCoreExtensions
{
    public static class PropertyInfoExtensions
    {
        private static Dictionary<string, PropertyInfo> cacheOfProperties = new Dictionary<string, PropertyInfo>();

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
