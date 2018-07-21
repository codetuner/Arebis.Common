using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Json
{
    /// <summary>
    /// Indicates this object type uses custom Json serialization.
    /// </summary>
    public interface ICustomJsonSerializable
    {
        /// <summary>
        /// Performs a custom read from json.
        /// </summary>
        object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer);

        /// <summary>
        /// Performs a custom write to json.
        /// </summary>
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
    }
}
