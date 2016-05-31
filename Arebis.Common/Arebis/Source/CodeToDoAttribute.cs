using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Source
{
    /// <summary>
    /// Decorates code that needs completion or refactoring.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CodeToDoAttribute : Attribute
    {
        /// <summary>
        /// Decorates code that needs completion or refactoring.
        /// </summary>
        public CodeToDoAttribute(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Message or information about the needed code completion or refactoring.
        /// </summary>
        public string Message { get; set; }
    }
}
