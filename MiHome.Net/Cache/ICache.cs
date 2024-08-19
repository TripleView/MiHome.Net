namespace MiHome.Net.Cache
{
    public interface ICache
    {
        #region sync
        /// <summary>
        /// 绝对时间缓存，固定时间后缓存值失效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        bool SetValueWithAbsolute<T>(string key, T value, TimeSpan absoluteExpiration);
        /// <summary>
        /// 滑动时间缓存，如果在时间内有命中，则继续延长时间，未命中则缓存值失效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="slidingExpiration"></param>
        /// <returns></returns>
        bool SetValueWithSliding<T>(string key, T value, TimeSpan slidingExpiration);
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        CacheEntity<T> GetValue<T>(string key);
        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove(string key);


        #endregion

        #region async
        /// <summary>
        /// 绝对时间缓存，固定时间后缓存值失效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns></returns>
        Task<bool> SetValueWithAbsoluteAsync<T>(string key, T value, TimeSpan absoluteExpiration);
        /// <summary>
        ///  滑动时间缓存，如果在时间内有命中，则继续延长时间，未命中则缓存值失效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="slidingExpiration"></param>
        /// <returns></returns>
        Task<bool> SetValueWithSlidingAsync<T>(string key, T value, TimeSpan slidingExpiration);
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<CacheEntity<T>> GetValueAsync<T>(string key);
        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);

        #endregion
    }
}