using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system with 51 case-sensitive symbols (0-9, A-Z and a-z) avoiding ambiguous characters.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base51 : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base51 Instance = new Base51();

        /// <summary>
        /// Creates a numeral system with 51 case-sensitive symbols (0-9, A-Z and a-z) avoiding ambiguous characters.
        /// </summary>
        public Base51()
            : base("0123456789ABCDEFGHJKLMNPQRSTUVWXYZabdefghijkmnopqrt")
        { }
    }
}
