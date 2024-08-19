using MiHome.Net.Utils;
using Newtonsoft.Json;

namespace MiHome.Net.Cache
{
    public interface ICacheDeserializer
    {
        T DeserializeObject<T>(byte[] obj);
    }

    public class JsonCacheDeserializer : ICacheDeserializer
    {
        public T DeserializeObject<T>(byte[] obj)
        {
            var result= JsonConvert.DeserializeObject<T>(obj.GetString());
           return result;
        }
    }
}