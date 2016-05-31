using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Factories.Localization
{
    public abstract class BaseLocalizationFactory : ILocalizationFactory
    {
        public abstract void Reinitialize();

        public IDictionary<string, string> GetResources(string context)
        {
            return this.GetResources(context, Thread.CurrentThread.CurrentUICulture);
        }

        public abstract IDictionary<string, string> GetResources(string context, Globalization.CultureInfo uiCulture);
    }
}
