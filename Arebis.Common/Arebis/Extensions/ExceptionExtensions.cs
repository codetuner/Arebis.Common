using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Adds data to the given Exception object.
        /// </summary>
        public static Exception WithData(this Exception ex, object dataKey, object dataValue)
        {
            if (ex.Data != null) ex.Data[dataKey] = dataValue;
            return ex;
        }
    }
}
