using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Json
{
    public static class Extensions
    {
        public static string ToJsonString(this IJsonSerializable obj, bool indent = false)
        {
            return JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
        }
    }
}
