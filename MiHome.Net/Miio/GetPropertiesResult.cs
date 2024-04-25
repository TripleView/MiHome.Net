using Newtonsoft.Json;

namespace MiHome.Net.Miio;

public class GetPropertiesResult
{
    public int Id { get; set; }
    public List<GetPropertiesResultItem> Result { get; set; }
    [JsonProperty("exe_time")]
    public int ExeTime { get; set; }
}

public class GetPropertiesResultItem
{
    public string Did { get; set; }
    public int Siid { get; set; }
    public int Piid { get; set; }
    public int Code { get; set; }
    public object Value { get; set; }
}
