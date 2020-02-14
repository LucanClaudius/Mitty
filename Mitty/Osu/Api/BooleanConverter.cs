using Newtonsoft.Json;
using System;

namespace Mitty.Osu.Api
{
    class BooleanConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;

            if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
                return false;

            if (string.Equals(value, "1"))
                return true;

            return false;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(String) || typeToConvert == typeof(Boolean))
                return true;

            return false;
        }
    }
}
