using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Types
{
    public static class AutoInitializeExtensions
    {
        /// <summary>
        /// Auto initializes this instance. To be called from within constructor methods of to auto initialize classes.
        /// </summary>
        public static void AutoInitialize(this object obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                var pattr = (AutoInitializeAttribute)Attribute.GetCustomAttribute(prop, typeof(AutoInitializeAttribute));
                if (pattr != null)
                {
                    prop.SetValue(obj, pattr.CreateValue(prop.PropertyType, obj));
                    continue;
                }
                foreach (AutoInitializeAttribute cattr in prop.PropertyType.GetCustomAttributes(typeof(AutoInitializeAttribute), false))
                {
                    prop.SetValue(obj, cattr.CreateValue(prop.PropertyType, obj));
                    break;
                }
            }
        }
    }
}
