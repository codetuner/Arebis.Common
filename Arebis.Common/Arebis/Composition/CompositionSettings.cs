using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;

namespace Arebis.Composition
{
    public static class CompositionSettings
    {
        private static object _syncRoot = new Object();
        private static AggregateCatalog defaultCatalog;

        public static AggregateCatalog DefaultCatalog
        {
            get
            {
                lock (_syncRoot)
                {
                    if (defaultCatalog == null)
                    {
                        defaultCatalog = new AggregateCatalog();
                        defaultCatalog.Catalogs.Add(new DirectoryCatalog(ConfigurationManager.AppSettings["DefaultCompositionCatalogPath"] ?? @".\bin"));
                    }

                    return defaultCatalog;
                }
            }
            set 
            {
                lock (_syncRoot)
                {
                    defaultCatalog = value;
                }
            }
        }
    }
}
