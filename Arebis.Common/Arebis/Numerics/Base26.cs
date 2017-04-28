using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system using the 26 letters of the Roman alphabet (A-Z), case-unsensitive.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base26 : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base26 Instance = new Base26();

        /// <summary>
        /// Creates a numeral system using the 26 letters of the Roman alphabet (A-Z), case-unsensitive.
        /// </summary>
        public Base26()
            : base("ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        { }

        public override string PrepareForParse(string s)
        {
            return s.ToUpperInvariant();
        }
    }
}
