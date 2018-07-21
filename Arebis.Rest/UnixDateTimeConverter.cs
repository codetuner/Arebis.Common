using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineDogShows.Payments.MangoPay.ApiContract
{
    public class UnixDateTimeConverter : Newtonsoft.Json.Converters.DateTimeConverterBase
    {
        private static readonly DateTime EpochDt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTimeOffset EpochDto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromHours(0));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return ConvertFromUnixFormat((long)reader.Value);
            }
            else if (reader.TokenType == JsonToken.Float)
            {
                return ConvertFromUnixFormat(Int64.Parse(reader.Value.ToString()));
            }
            else
            {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                writer.WriteValue(ConvertToUnixFormat((DateTime)value));
            }
            else if (value is DateTimeOffset)
            {
                writer.WriteValue(ConvertToUnixFormat((DateTimeOffset)value));
            }
            else
            {
                writer.WriteValue((Int64?)null);
            }
        }

        public static DateTime ConvertFromUnixFormat(long unixtimestamp)
        {
            return EpochDt.AddSeconds(unixtimestamp);
        }

        public static long ConvertToUnixFormat(DateTime value)
        {
            return (long)(value - EpochDt).TotalSeconds;
        }

        public static long ConvertToUnixFormat(DateTimeOffset value)
        {
            return (long)(value - EpochDto).TotalSeconds;
        }
    }
}
