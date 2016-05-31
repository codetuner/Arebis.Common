using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Arebis.Web
{
    /// <summary>
    /// Represents a shopping cart of T items stored in the session.
    /// </summary>
    public static class Cart<T>
    {
        public static void Clear()
        {
            HttpContext.Current.Session[typeof(Cart<T>).FullName] = null;
        }

        public static void Add(T item)
        {
            GetCartList().Add(item);
        }

        public static void RemoveAt(int index)
        {
            GetCartList().RemoveAt(index);
        }

        public static IList<T> Content
        {
            get
            {
                return GetCartList();
            }
        }

        private static List<T> GetCartList()
        {
            var cart = (List<T>)HttpContext.Current.Session[typeof(Cart<T>).FullName];
            if (cart == null)
            {
                HttpContext.Current.Session[typeof(Cart<T>).FullName] = cart = new List<T>();
            }

            return cart;
        }
    }
}
