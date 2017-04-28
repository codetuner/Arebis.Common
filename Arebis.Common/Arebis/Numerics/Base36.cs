using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system with 36 case-unsensitive symbols (0-9 and A-Z).
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base36 : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base36 Instance = new Base36();

        /// <summary>
        /// Creates a numeral system with 36 case-unsensitive symbols (0-9 and A-Z).
        /// </summary>
        public Base36()
            : base(36)
        { }

        public override string PrepareForParse(string s)
        {
            return s.ToUpperInvariant();
        }
    }
}
