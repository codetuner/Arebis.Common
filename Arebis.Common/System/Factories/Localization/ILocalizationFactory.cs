using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Factories.Localization
{
    public interface ILocalizationFactory
    {
        /// <summary>
        /// Clears all caches and re-initializes the LocalizationFactory.
        /// </summary>
        void Reinitialize();

        /// <summary>
        /// Returns a resource dictionary for the given context, current UI culture.
        /// </summary>
        IDictionary<string, string> GetResources(string context);

        /// <summary>
        /// Returns a resource dictionary for the given context and given culture.
        /// </summary>
        IDictionary<string, string> GetResources(string context, CultureInfo uiCulture);
    }
}
