using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineDogShows.Payments.PayPal
{
    // https://stackoverflow.com/a/24052157
    // http://www.rubencanton.com/blog/2017/05/implementing-a-basic-newtonsofts-jsonconverter.html

    public class CustomDecimalConverter : JsonConverter
    {
        public CustomDecimalConverter()
            : this(4)
        { }

        public CustomDecimalConverter(int roundingDecimals)
            : this(roundingDecimals, (roundingDecimals == 0) ? "0" : ("0." + new string('0', roundingDecimals)))
        { }

        public CustomDecimalConverter(int roundingDecimals, string format)
            : this(roundingDecimals, format, CultureInfo.InvariantCulture)
        { }

        public CustomDecimalConverter(int roundingDecimals, string format, string cultureName)
        {
            this.RoundingDecimals = roundingDecimals;
            this.Format = format;
            this.Culture = String.IsNullOrEmpty(cultureName) ? CultureInfo.InvariantCulture : new CultureInfo(cultureName);
        }

        public CustomDecimalConverter(int roundingDecimals, string format, CultureInfo culture)
        {
            this.RoundingDecimals = roundingDecimals;
            this.Format = format;
            this.Culture = culture ?? CultureInfo.InvariantCulture;
        }

        public int RoundingDecimals { get; set; }

        public string Format { get; set; }

        public CultureInfo Culture { get; set; }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                return token.ToObject<decimal>();
            }
            if (token.Type == JTokenType.String)
            {
                return Decimal.Parse(token.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            }
            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }
            throw new JsonSerializationException("Unexpected token type: " + token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var d = (value as decimal?);
            if (d.HasValue)
            {
                if (this.RoundingDecimals < 4)
                    d = Math.Round(d.Value, this.RoundingDecimals, MidpointRounding.AwayFromZero);
                writer.WriteRawValue(d.Value.ToString(this.Format, this.Culture));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
