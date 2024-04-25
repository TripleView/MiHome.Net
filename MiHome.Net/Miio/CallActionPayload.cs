namespace MiHome.Net.Miio;

public class CallActionPayload
{
    public string Did => $"call-{Siid}-{Aiid}";
    /// <summary>
    /// service id 服务id
    /// </summary>
    public int Siid { get; set; }
    /// <summary>
    /// action id 方法id
    /// </summary>
    public int Aiid { get; set; }
    /// <summary>
    /// 入参
    /// </summary>
    public List<string> In { get; set; }
}