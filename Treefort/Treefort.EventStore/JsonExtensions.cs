using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Treefort.EventStore
{
    public static class JsonExtensions
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
            };


        public static byte[] ToJsonBytes(this object source)
        {
            var instring = JsonConvert.SerializeObject(source, Formatting.Indented, JsonSettings);
            return Encoding.UTF8.GetBytes(instring);
        }

        public static string ToJson(this object source)
        {
            var instring = JsonConvert.SerializeObject(source, Formatting.Indented, JsonSettings);
            return instring;
        }

        public static T ParseJson<T>(this string json)
        {
            var result = JsonConvert.DeserializeObject<T>(json, JsonSettings);
            return result;
        }

        public static T ParseJson<T>(this byte[] json)
        {
            var result = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json), JsonSettings);
            return result;
        }
    }
}