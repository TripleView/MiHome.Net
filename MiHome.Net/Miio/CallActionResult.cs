using Newtonsoft.Json;

namespace MiHome.Net.Miio;

public class CallActionResult
{
    public int Id { get; set; }
    public CallActionResultItem result { get; set; }
    [JsonProperty("exe_time")]
    public int ExeTime { get; set; }
}

public class CallActionResultItem
{
    public int Code { get; set; }
    public List<object> Out { get; set; }
}
