using Newtonsoft.Json;

namespace MiHome.Net.Miio;

public class GetDeviceInfoResult
{
    public int id { get; set; }
    public GetDeviceInfoResultItem Result { get; set; }
    [JsonProperty("exe_time")]
    public int ExeTime { get; set; }
}

public class GetDeviceInfoResultItem
{
    public int Life { get; set; }
    public long Uid { get; set; }
    public string Model { get; set; }
    public string Token { get; set; }
    public int Ipflag { get; set; }

    [JsonProperty("fw_ver")]
    public string FwVer { get; set; }
    [JsonProperty("miio_ver")]
    public string MiioVer { get; set; }
    [JsonProperty("hw_ver")]
    public string HwVer { get; set; }
    public int Mmfree { get; set; }
    public string Mac { get; set; }

    [JsonProperty("wifi_fw_ver")]
    public string WifiFwVer { get; set; }
    public Ap Ap { get; set; }
    public Netif Netif { get; set; }
}

public class Ap
{
    public string Ssid { get; set; }
    public string Bssid { get; set; }
    public int Rssi { get; set; }
    public int Primary { get; set; }
}

public class Netif
{
    public string LocalIp { get; set; }
    public string Mask { get; set; }
    public string Gw { get; set; }
}
