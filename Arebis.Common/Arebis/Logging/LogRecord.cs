using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Arebis.Logging
{
    [Serializable]
    public sealed class LogRecord
    {
        public Guid Identifier { get; set; }

        public XDocument Document { get; private set; }

        public void Set(string collectionName, string valueName, object value)
        {
            throw new NotImplementedException();
        }

        public void Add(string collectionName, object value)
        {
            throw new NotImplementedException();
        }

        public object Get(string collectionName, string valueName, object valueIfMissing = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> Enumerate(string collectionName)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public LogRecord Extend()
        {
            var extension = new LogRecord();
            extension.Identifier = this.Identifier;
            return extension;
        }
    }
}
