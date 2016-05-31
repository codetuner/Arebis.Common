using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.ImportExport
{
    public enum DbExportMode
    {
        /// <summary>
        /// None, do not export related rows.
        /// </summary>
        None = 0,

        /// <summary>
        /// Children, only export parent with it's children.
        /// </summary>
        Children = 1,

        /// <summary>
        /// Full, export parent with it's children and children with it's parents.
        /// </summary>
        Full = 2,

    }
}
