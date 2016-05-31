using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// RouteValueDictionary extension methods.
    /// </summary>
    public static class RouteValueDictionaryExtensions
    {
        /// <summary>
        /// A RouteValueDictionary copy, extended with the given key/value pair.
        /// </summary>
        public static RouteValueDictionary With(this RouteValueDictionary subject, string key, object value)
        {
            subject = new RouteValueDictionary(subject);
            subject[key] = value;
            return subject;
        }
    }
}
