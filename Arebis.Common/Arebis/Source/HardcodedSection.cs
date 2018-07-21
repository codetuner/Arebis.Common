using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Source
{
    /// <summary>
    /// This class is to be instantiated with a using block to surround code that contains hardcoded portions and may need refactoring.
    /// This code itself does nothing, it is just a marker class.
    /// </summary>
    public class HardcodedSection : IDisposable
    {
        public HardcodedSection()
        { }

        public HardcodedSection(string filterExpression)
        { }

        public HardcodedSection(string filterExpression, bool toRefactor = false)
        { }

        public HardcodedSection(string filterExpression, string documentation, bool toRefactor = false)
        { }

        public void Dispose()
        { }
    }
}
