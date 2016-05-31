using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Contract
{
    /// <summary>
    /// Generic paged results
    /// </summary>
    [DataContract, Serializable]
    public class PagedResponse<T>
        where T : class
    {
        public PagedResponse()
        { }

        /// <summary>
        /// Total number of items existing on backstore.
        /// </summary>
        [DataMember()]
        public int TotalCount { get; set; }

        /// <summary>
        /// Number of items existing on backstore that match the filter.
        /// </summary>
        [DataMember()]
        public int TotalFilteredCount { get; set; }

        /// <summary>
        /// Some mysterious value echoed from the request.
        /// Matches the 'draw' parameter from DataTables.
        /// </summary>
        [DataMember()]
        public int Echo { get; set; }

        /// <summary>
        /// The resulting objects.
        /// </summary>
        [DataMember()]
        public T[] Results { get; set; }

        /// <summary>
        /// Returns AutocompleteItems based on the given mapping.
        /// </summary>
        /// <param name="mapping">Mapping expression defining value, label and eventually other properties no Autocomplete items.</param>
        public List<Object> ToAutocompleteItem(Func<T, object> mapping) {
            List<object> ret = new List<object>();

            foreach (T item in this.Results)
            {
                object o = mapping.Invoke(item);
                ret.Add(o);
            }

            if (TotalFilteredCount > 0)
            {
                ret.Add(new { TotalFilteredCount = this.Results.Length, TotalCount = this.TotalFilteredCount });
            }

            return ret; 
        }
    }
}
