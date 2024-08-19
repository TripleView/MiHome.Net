namespace MiHome.Net.Utils;

public static class DateTimeUtils
{

    /// <summary>
    /// 1970的起始时间
    /// </summary>
    private static readonly DateTime Jan1st1970 = new DateTime
        (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static DateTime UnixTimeStampToUtcDateTime(this double unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }

    public static double UtcDateTimeToUnixTimeStamp(this DateTime utcDateTime)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var result = (utcDateTime - dateTime).TotalSeconds;
        return result;
    }

    /// <summary>
    /// 返回自1970年以来以毫秒为单位的UTC时间
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static long CurrentSeconds()
    {
        return (long)(DateTime.UtcNow - Jan1st1970).TotalSeconds;
    }
}