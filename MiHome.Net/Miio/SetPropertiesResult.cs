using Newtonsoft.Json;

namespace MiHome.Net.Miio;

public class SetPropertiesResult
{
    public int Id { get; set; }
    public List<SetPropertiesResultItem> Result { get; set; }

    [JsonProperty("exe_time")]
    public int ExeTime { get; set; }
}

public class SetPropertiesResultItem
{
    public string Did { get; set; }
    public int Siid { get; set; }
    public int Piid { get; set; }
    public int Code { get; set; }
}
