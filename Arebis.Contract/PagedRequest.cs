using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Contract
{
    /// <summary>
    /// Page filter
    /// </summary>
    [DataContract, Serializable]    
    public class PagedRequest
    {
        /// <summary>
        /// Constructs a default PagedRequest.
        /// </summary>
        public PagedRequest()
        {
            // Default values:
            this.Ascending = true;
            this.Length = 10;
        }

        /// <summary>
        /// Name or name/value combination of filtering to be done before returning whole count of items.
        /// </summary>
        [DataMember()]
        public string PreFilter { get; set; }

        /// <summary>
        /// A global search value.
        /// </summary>
        [DataMember()]
        public string GlobalSearchValue { get; set; }

        /// <summary>
        /// Name of a named filter.
        /// Named filters are predefined filters identified by name.
        /// </summary>
        [DataMember()]
        public string NamedFilter { get; set; }

        /// <summary>
        /// Index of the first row to retrieve.
        /// </summary>
        [DataMember()]
        public int Start { get; set; }

        /// <summary>
        /// Number of rows to retrieve.
        /// </summary>
        [DataMember()]
        public int Length { get; set; }

        /// <summary>
        /// Comma-separated list of fields to sort on.
        /// </summary>
        [DataMember()]
        public string OrderByFields { get; set; }

        /// <summary>
        /// Whether ordering should be ascending (default).
        /// </summary>
        [DataMember()]
        public bool Ascending { get; set; }

        /// <summary>
        /// Some mysterious value that will be returned with the PagedResponse.
        /// </summary>
        [DataMember()]
        public int Echo { get; set; }
    }
}
