using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Json
{
    /// <summary>
    /// A JsonConverter that converts ICustomJsonSerializable types.
    /// </summary>
    public class CustomJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ICustomJsonSerializable).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ((ICustomJsonSerializable)Activator.CreateInstance(objectType)).ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ((ICustomJsonSerializable)value).WriteJson(writer, serializer);
        }
    }
}
