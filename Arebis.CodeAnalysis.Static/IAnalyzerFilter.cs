using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Arebis.CodeAnalysis.Static
{
    public interface IAnalyzerFilter
    {
        bool ProcessAssembly(Assembly assembly);

        bool ProcessType(Type type);

        bool ProcessMethod(MethodBase method);
    }
}
