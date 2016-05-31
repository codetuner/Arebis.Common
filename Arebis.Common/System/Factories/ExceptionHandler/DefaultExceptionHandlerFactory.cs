using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Factories.ExceptionHandler
{
    public class DefaultExceptionHandlerFactory : IExceptionHandlerFactory
    {
        public virtual void Ignore(Exception ex, string reason = null)
        {
        }

        public virtual void Log(Exception ex, string contextInformation = null)
        {
            if (ex == null) return;
            System.Diagnostics.Debug.WriteLine("__ {0:yyyy/MM/dd HH:mm:ss} _ Exception log ___", System.DateTime.Now);
            System.Diagnostics.Debug.WriteLine(ex);
            System.Diagnostics.Debug.WriteLine(contextInformation ?? "(No contextual information provided.)");
            System.Diagnostics.Debug.WriteLine("_______________________________", System.DateTime.Now);
        }
    }
}
