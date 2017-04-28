using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A 'regular' decimal numeral system with 10 symbols (0-9).
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base10 : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base10 Instance = new Base10();

        /// <summary>
        /// Creates a 'regular' decimal numeral system with 10 symbols (0-9).
        /// </summary>
        public Base10()
            : base(10)
        { }


    }
}
