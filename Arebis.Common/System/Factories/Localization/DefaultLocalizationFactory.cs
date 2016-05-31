using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace System.Factories.Localization
{
    public class DefaultLocalizationFactory : BaseLocalizationFactory
    {
        private IDictionary<Tuple<string, string>, ResourceSet> cache = new ConcurrentDictionary<Tuple<string, string>, ResourceSet>();

        public override void Reinitialize()
        {
            cache.Clear();
        }

        public override IDictionary<string, string> GetResources(string context, CultureInfo uiCulture)
        {
            var key = new Tuple<string,string>(context, uiCulture.Name);
            var resourceSet = (ResourceSet)null;
            if (!cache.TryGetValue(key, out resourceSet))
            {
                cache[key] = resourceSet = new ResourceManager(Type.GetType(context)).GetResourceSet(uiCulture, true, true);
            }

            throw new NotImplementedException();
        }
    }
}
