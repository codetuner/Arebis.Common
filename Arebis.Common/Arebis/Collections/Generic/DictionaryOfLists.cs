using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Collections.Generic
{
    /// <summary>
    /// A dictionary of lists. See also DictionaryBag.
    /// </summary>
    [Serializable]
    public class DictionaryOfLists<TKey, TListItem> : Dictionary<TKey, IList<TListItem>>
    {
        /// <summary>
        /// Adds a value to a list of a dictionary key. If the dictionary key is not already set,
        /// creates a list and stores the value.
        /// </summary>
        public void AddValue(TKey key, TListItem value)
        {
            IList<TListItem> list;
            if (!this.TryGetValue(key, out list))
            {
                this[key] = list = this.CreateNewList();
            }

            list.Add(value);
        }

        /// <summary>
        /// Returns the values associated with the dictionary key. If the dictionary key is not set,
        /// returns no values (and no exception).
        /// </summary>
        public IEnumerable<TListItem> GetValues(TKey key)
        {
            IList<TListItem> list;
            if (this.TryGetValue(key, out list))
            {
                foreach (var item in list)
                    yield return item;
            }
            else
            {
                yield break;
            }
        }

        /// <summary>
        /// By default, creates instances of System.Collections.Generic.List&lt;T&gt;.
        /// Override this method to create instances of another IList type.
        /// </summary>
        protected virtual IList<TListItem> CreateNewList()
        {
            return new List<TListItem>();
        }
    }
}
