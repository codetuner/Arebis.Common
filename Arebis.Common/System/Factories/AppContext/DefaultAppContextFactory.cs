using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Factories.AppContext
{
    public class DefaultAppContextFactory : IAppContextFactory
    {
        public virtual string MapPath(params string[] paths)
        {
            var result = AppDomain.CurrentDomain.BaseDirectory;

            if (paths != null && paths.Length > 0)
            {
                if (paths[0] != null)
                    result = Path.Combine(result, paths[0].Replace("~/", "").Replace("/","\\"));
            }

            for (int i = 1; i < paths.Length; i++)
            {
                if (paths[i] != null)
                    result = Path.Combine(result, paths[i].Replace("/","\\"));
            }

            return result;
        }
    }
}
