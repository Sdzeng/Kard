using Newtonsoft.Json;

namespace Kard.Json
{
    public class Serialize
    {
        public static T FromJson<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
        public static string ToJson<T>(T value)
        {
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(value, Formatting.None, jSetting);
        }
    }
}
