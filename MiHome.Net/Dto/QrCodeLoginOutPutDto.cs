namespace MiHome.Net.Dto;

public class QrCodeLoginOutPutDto
{
    public string loginUrl { get; set; }
    public string qr { get; set; }
    public string qrTips { get; set; }
    public string lp { get; set; }
    public string sl { get; set; }
    public int timeout { get; set; }
    public int timeInterval { get; set; }
    public int code { get; set; }
    public string result { get; set; }
    public string desc { get; set; }
    public string description { get; set; }
}
