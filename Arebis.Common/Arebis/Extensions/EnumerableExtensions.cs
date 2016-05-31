using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the given enumerable values grouped in sets of groupSize.
        /// The last set can be smaller.
        /// </summary>
        /// <typeparam name="T">Type of values in enumeration.</typeparam>
        /// <param name="enumerable">An enumerable to group.</param>
        /// <param name="groupSize">Maximum group size. The last group can be smaller.</param>
        public static IEnumerable<T[]> GroupByCountOf<T>(this IEnumerable<T> enumerable, int groupSize)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                var group = new T[groupSize];
                var groupIndex = 0;
                while (enumerator.MoveNext())
                {
                    group[groupIndex++] = enumerator.Current;
                    if (groupIndex == groupSize)
                    {
                        yield return group;
                        group = new T[groupSize];
                        groupIndex = 0;
                    }
                }

                if (groupIndex != 0)
                {
                    var remainder = new T[groupIndex];
                    Array.Copy(group, remainder, groupIndex);
                    yield return remainder;
                }
            }
        }

        /// <summary>
        /// Return the first index for which the given predicate matches. Returns -1 if no match was found.
        /// </summary>
        public static int IndexWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item)) return index;
                index++;
            }

            return -1;
        }

        /// <summary>
        /// Return the indexes for which the given predicate match.
        /// </summary>
        public static IEnumerable<int> IndexesWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item)) yield return index;
                index++;
            }
        }

        /// <summary>
        /// Transforms an enumeration of input items into an enumeration of master/detail items.
        /// </summary>
        /// <example>
        /// This example shows how to transform a joined query result into master/details order with orderlines.
        /// The used QueryMapper executes the SQL query and returns an ExpandoObject per row.
        /// <code>
        /// class Program
        /// {
        ///     static void Main(string[] args)
        ///     {
        ///         // Get a DbConnection instance:
        ///         var connection = Arebis.Data.DbConnectionManager.GetConnection("DefaultConnection");
        ///
        ///         using (var q = new Arebis.Data.QueryMapper(connection, "SELECT o.Id AS OrderId, o.Date AS OrderDate, c.Id AS CustomerId, c.Name AS CustomerName, ol.Id AS OrderLineId, ol.ArticleCode AS ArticleCode, ol.Quantity AS Quantity, ol.UnitPrice AS UnitPrice FROM [Order] AS o INNER JOIN [Customer] AS c ON (o.CustomerId = c.Id) LEFT OUTER JOIN [OrderLine] AS ol ON (o.Id = ol.OrderId)"))
        ///         {
        ///             // Transform query results to master/detail objects:
        ///             var orders = q.TakeAll().ToMasterDetail&lt;dynamic, Order, OrderLine&gt;(
        ///                 // The column(s) that identity a master object:
        ///                 r => r.OrderId,
        ///                 // Transforms a row into a master object:
        ///                 r => new Order() { Id = (int)r.OrderId, Date = (DateTime)r.OrderDate, Customer = new Customer() { Id = (int)r.CustomerId, Name = (string)r.CustomerName }, Lines = new List&lt;OrderLine&gt;() },
        ///                 // Returns the master's details collection:
        ///                 o => o.Lines,
        ///                 // Transform a row into a detail object (with null check if outer join is used):
        ///                 r => (r.OrderLineId == null) ? null : new OrderLine() { Id = (int)r.OrderLineId, ArticleCode = (string)r.ArticleCode, Quantity = (int)r.Quantity, UnitPrice = (decimal)r.UnitPrice },
        ///                 // (Optional) link back detail to master:
        ///                 (ol, o) => ol.Order = o
        ///             );
        ///        
        ///             // Iterate over all master objects, showing total price:
        ///             Console.WriteLine("{0,8}  {1,-10}  {2,-40}  {3,10}", "OrderId", "Date", "Customer", "TotalPrice");
        ///             foreach (var order in orders)
        ///             {
        ///                 Console.WriteLine("{0,8}  {1:yyyy-MM-dd}  {2,-40}  {3,10:#,##0.00}", order.Id, order.Date, order.Customer.Name, order.Lines.Sum(ol => ol.Quantity * ol.UnitPrice));
        ///             }
        ///         }
        ///     }
        /// }
        /// 
        /// public class Customer
        /// {
        ///     public int Id { get; set; }
        /// 
        ///     public string Name { get; set; }
        /// }
        /// 
        /// public class Order
        /// {
        ///     public int Id { get; set; }
        /// 
        ///     public DateTime Date { get; set; }
        /// 
        ///     public Customer Customer { get; set; }
        /// 
        ///     public List&lt;OrderLine&gt; Lines { get; set; }
        /// }
        /// 
        /// public class OrderLine
        /// {
        ///     public int Id { get; set; }
        /// 
        ///     public string ArticleCode { get; set; }
        /// 
        ///     public int Quantity { get; set; }
        /// 
        ///     public decimal UnitPrice { get; set; }
        /// 
        ///     public Order Order { get; set; }
        /// }
        /// </code>
        /// </example>
        /// <typeparam name="TInput">Type of the input objects, the 'row' items.</typeparam>
        /// <typeparam name="TMaster">Type of master objects.</typeparam>
        /// <typeparam name="TDetail">Type of detail objects.</typeparam>
        /// <param name="input">Enumeration of input objects.</param>
        /// <param name="masterIdentifier">Function returning the identifier (single property or tuple) for master objects.</param>
        /// <param name="inputToMasterTransform">Function transforming input objects into master objects.</param>
        /// <param name="masterDetailsCollection">Function returning the details collection of a master object.</param>
        /// <param name="inputToDetailTransform">Function transforming input obects into detail objects.</param>
        /// <param name="detailToMasterReferenceSetter">(Optional) Action to set master object on detail object.</param>
        /// <returns>An enumeration of master objects.</returns>
        public static IEnumerable<TMaster> ToMasterDetail<TInput, TMaster, TDetail>(this IEnumerable<TInput> input, Func<TInput, object> masterIdentifier, Func<TInput, TMaster> inputToMasterTransform, Func<TMaster, ICollection<TDetail>> masterDetailsCollection, Func<TInput, TDetail> inputToDetailTransform, Action<TDetail, TMaster> detailToMasterReferenceSetter = null)
            where TMaster : class
            where TDetail : class
        {
            var masters = new Dictionary<object, TMaster>();

            foreach (var item in input)
            {
                var key = masterIdentifier(item);
                TMaster master;
                if (!masters.TryGetValue(key, out master))
                {
                    masters[key] = master = inputToMasterTransform(item);
                }

                var collection = masterDetailsCollection(master);
                var detail = inputToDetailTransform(item);
                if (detail != null)
                {
                    if (detailToMasterReferenceSetter != null) detailToMasterReferenceSetter(detail, master);
                    collection.Add(detail);
                }
            }

            return masters.Values;
        }
    }
}
