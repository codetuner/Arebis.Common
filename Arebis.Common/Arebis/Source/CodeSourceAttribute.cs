using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Source
{
    /// <summary>
    /// Decorates the code with information about it's external origin.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CodeSourceAttribute : Attribute
    {
        /// <summary>
        /// Decorates the code with information about it's external origin.
        /// </summary>
        /// <param name="sourceUrl">URL to the original source of the code.</param>
        /// <param name="authorName">Name of the code author.</param>
        /// <param name="copyright">Copyright of the code.</param>
        public CodeSourceAttribute(string sourceUrl, string authorName = null, string copyright = null)
        {
            this.SourceUrl = sourceUrl;
            this.AuthorName = authorName;
            this.Copyright = copyright;
        }

        /// <summary>
        /// URL to the original source of the code.
        /// </summary>
        public string SourceUrl { get; private set; }

        /// <summary>
        /// Name of the code author.
        /// </summary>
        public string AuthorName { get; private set; }

        /// <summary>
        /// Copyright of the code.
        /// </summary>
        public string Copyright { get; set; }
    }
}
