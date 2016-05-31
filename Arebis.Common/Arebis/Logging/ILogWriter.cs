using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Logging
{
    public interface ILogWriter
    {
        void Write(LogRecord record);

        void Flush();
    }
}
