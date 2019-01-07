using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Text
{
    /// <summary>
    /// Level of unidecoding.
    /// </summary>
    [Serializable]
    public enum UnidecoderLevel
    {
        /// <summary>
        /// Do not translate or 'unidecode' chars.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Do not translate chars of the default ANSI set.
        /// </summary>
        Ansi = 1,

        /// <summary>
        /// Do not translate chars of the default ANSI set as well as common accents.
        /// </summary>
        AnsiPlus = 2,

        /// <summary>
        /// Translate all chars.
        /// </summary>
        Ascii = 3,
    }
}
