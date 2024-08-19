using Newtonsoft.Json;

namespace MiHome.Net.Utils;

public static class StringUtils
{
    /// <summary>
    /// 判断字符串是否有值
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool HasText(this string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    /// <summary>
    /// 判断字符串是否为空
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string ToJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}