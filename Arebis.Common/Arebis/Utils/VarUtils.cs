using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Utils
{
    /// <summary>
    /// Utilities for working with variables.
    /// </summary>
    public static class VarUtils
    {
        /// <summary>
        /// Swaps the values of the two given variables.
        /// </summary>
        public static void Swap<T>(ref T a, ref T b)
        {
            T x = a;
            a = b;
            b = x;
        }
    }
}
