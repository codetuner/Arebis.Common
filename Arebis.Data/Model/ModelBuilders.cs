using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Data.Model
{
    public static class ModelBuilders
    {
        static ModelBuilders() 
        {
            ModelBuilderTypes = new Dictionary<string, Type>();
            ModelBuilderTypes["System.Data.SqlClient"] = typeof(SqlModelBuilder);
        }

        public static IDictionary<string, Type> ModelBuilderTypes { get; private set; }

        public static IModelBuilder GetModelBuilder(string providerName)
        {
            var type = (Type)null;
            if (ModelBuilderTypes.TryGetValue(providerName, out type))
            {
                return (IModelBuilder)Activator.CreateInstance(type);
            }
            else
            {
                throw new ArgumentException(String.Format("No ModelBuilder registered for the given provider name \"{0}\".", providerName));
            }
        }
    }
}
