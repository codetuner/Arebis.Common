using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Factories.ExceptionHandler
{
    public interface IExceptionHandlerFactory
    {
        void Ignore(Exception ex, string reason = null);

        void Log(Exception ex, string contextInformation = null);
    }
}
