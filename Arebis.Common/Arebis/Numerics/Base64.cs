using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system with 64 case-sensitive symbols (0-9, A-Z, a-z, + and /).
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base64 : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base64 Instance = new Base64();

        /// <summary>
        /// Creates a numeral system with 64 case-sensitive symbols (0-9, A-Z, a-z, + and /).
        /// </summary>
        public Base64()
            : base(64)
        { }
    }
}