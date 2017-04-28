using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system with 2 symbols (0 and 1).
    /// </summary>
    [DataContract]
    [Serializable]
    public class Binary : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Binary Instance = new Binary();

        /// <summary>
        /// Creates a numeral system with 2 symbols (0 and 1).
        /// </summary>
        public Binary()
            : base(2)
        { }
    }
}
