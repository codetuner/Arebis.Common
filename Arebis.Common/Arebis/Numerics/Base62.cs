using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Numerics
{
    /// <summary>
    /// A numeral system with 62 case-sensitive symbols (0-9, A-Z and a-z).
    /// </summary>
    [DataContract]
    [Serializable]
    public class Base62 : SymbolicNumeralSystem
    {
        /// <summary>
        /// Default instance.
        /// </summary>
        public static Base62 Instance = new Base62();

        /// <summary>
        /// Creates a numeral system with 62 case-sensitive symbols (0-9, A-Z and a-z).
        /// </summary>
        public Base62()
            : base(62)
        { }
    }
}
