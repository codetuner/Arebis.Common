using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Factories.AppContext
{
    public interface IAppContextFactory
    {
        /// <summary>
        /// Maps a path relative to the application installation or root folder.
        /// Multiple paths can be given and will be combined.
        /// </summary>
        string MapPath(params string[] paths);
    }
}
