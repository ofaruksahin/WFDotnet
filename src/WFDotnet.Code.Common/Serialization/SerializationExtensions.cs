using Newtonsoft.Json;

namespace WFDotnet.Code.Common.Serialization
{
    public static class SerializationExtensions
    {

        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string ToSerialize(this object @this)
        {
            return JsonConvert.SerializeObject(@this, _jsonSerializerSettings);
        }

        public static T ToDeserialize<T>(this string @this)
        {
            return JsonConvert.DeserializeObject<T>(@this, _jsonSerializerSettings);
        }
    }
}
