namespace MiHome.Net.Dto;

/// <summary>
/// 二维码登录参数
/// </summary>
public class QrCodeLoginInputDto
{
    public string _qrsize { get; set; }
    public string qs { get; set; }

    public string bizDeviceType { get; set; } = "";
    public string callback { get; set; }

    public string _json { get; set; } = "true";
    public string theme { get; set; } = "";

    public string sid { get; set; } = "xiaomiio";
    public string needTheme { get; set; } = "false";

    public string showActiveX { get; set; } = "false";
    public string _local { get; set; } = "zh_CN";

    public string _sign { get; set; } 
    public string _dc { get; set; }

    public string serviceParam { get; set; }
}