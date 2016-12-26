using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Extensions
{
    public static class CollectionExtension
    {
        public static void AddRange<TValue>(this ICollection<TValue> coll, IEnumerable<TValue> range)
        {
            foreach (var item in range)
                coll.Add(item);
        }

        /// <summary>
        /// Removes several items from a collection.
        /// </summary>
        /// <returns>Values removed.</returns>
        public static IEnumerable<TValue> RemoveMany<TValue>(this ICollection<TValue> collection, IEnumerable<TValue> valuesToRemove)
        {
            var values = valuesToRemove.ToArray();
            foreach(var value in values)
            {
                collection.Remove(value);
            }

            return values;
        }

        /// <summary>
        /// Removes all items from a collection that match a given criteria.
        /// </summary>
        /// <returns>Values removed.</returns>
        public static IEnumerable<TValue> RemoveWhere<TValue>(this ICollection<TValue> collection, Func<TValue, bool> predicate)
        {
            return RemoveMany(collection, collection.Where(predicate));
        }

        /// <summary>
        /// Removes all occurences of the given item.
        /// </summary>
        /// <returns>The number of occurences removed.</returns>
        public static int RemoveAllOccurences<TValue>(this ICollection<TValue> collection, TValue item)
        {
            int count = 0;
            while (collection.Remove(item))
            {
                count++;
            }

            return count;
        }

        /// <summary>
        /// Adds the given item to the collection and returns the collection for fluent syntax.
        /// </summary>
        public static ICollection<T> With<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return collection;
        }

        /// <summary>
        /// Adds the given items to the collection and returns the collection for fluent syntax.
        /// </summary>
        public static ICollection<T> WithAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            collection.AddRange(items);
            return collection;
        }

        /// <summary>
        /// Removes the first instance of the given item from the collection and returns the collection for fluent syntax.
        /// </summary>
        public static ICollection<T> Without<T>(this ICollection<T> collection, T item)
        {
            collection.Remove(item);
            return collection;
        }

        /// <summary>
        /// Removes all instances of the given item from the collection and returns the collection for fluent syntax.
        /// </summary>
        public static ICollection<T> WithoutAny<T>(this ICollection<T> collection, T item)
        {
            collection.RemoveAllOccurences(item);
            return collection;
        }
    }
}
