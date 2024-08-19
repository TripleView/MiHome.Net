namespace MiHome.Net.Cache
{
    public class CacheOption
    {
        public ICacheDeserializer CacheDeserializer { get; set; }
        public ICacheSerializer CacheSerializer { get; set; }
        public bool IsUseMemory { get; private set; }

        /// <summary>
        /// 使用内存缓存
        /// </summary>
        public void UseMemory()
        {
            this.IsUseMemory = true;
        }

        public void UseRedis(string redisConnectionString)
        {
            this.IsUseRedis = true;
            this.RedisConnectionString = redisConnectionString;
        }
        public bool IsUseRedis { get; private set; }
        public string RedisConnectionString { get; private set; }
    }
}