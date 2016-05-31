using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data
{
    public static class DbConnectionManager
    {
        public static DbConnection GetConnection(String connectionName)
        {
            // Get the connection string info from web.config:
            var cs = ConfigurationManager.ConnectionStrings[connectionName];

            // Documented to return null if it couldn't be found:
            if (cs == null)
                throw new ConfigurationErrorsException("Invalid connection name \"" + connectionName + "\"");

            // Return connection:
            return GetConnection(cs.ConnectionString, cs.ProviderName);
        }

        public static DbConnection GetConnection(String connectionString, String providerName)
        {
            // Get the factory for the given provider (e.g. "System.Data.SqlClient"):
            var factory = DbProviderFactories.GetFactory(providerName);

            // Undefined behaviour if GetFactory couldn't find a provider.
            // Defensive test for null factory anyway
            if (factory == null)
                throw new Exception("Could not obtain factory for provider \"" + providerName + "\"");

            // Have the factory give us the right connection object:
            var conn = factory.CreateConnection();

            // Undefined behaviour if CreateConnection failed
            // Defensive test for null connection anyway
            if (conn == null)
                throw new Exception("Could not obtain connection from factory");

            // Knowing the connection string, open the connection
            conn.ConnectionString = connectionString;
            conn.Open();

            return conn;
        }
    }
}
