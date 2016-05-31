using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class TypeExtension
    {
        /// <summary>
        /// Returns an array of PropertyInfo objects that translates a property path as "Customer.Address.Town"
        /// into an array with properties Customer, Address and Town.
        /// </summary>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyPath(this Type type, string propertyPath)
        {
            var propertyNames = propertyPath.Split('.');
            var properties = new PropertyInfo[propertyNames.Length];

            for (int i = 0; i < propertyNames.Length; i++)
            {
                properties[i] = type.GetProperty(propertyNames[i]);
                type = properties[i].PropertyType;
            }

            return properties;
        }

    }
}
