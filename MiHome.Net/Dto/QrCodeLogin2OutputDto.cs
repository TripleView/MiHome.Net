namespace MiHome.Net.Dto;

public class QrCodeLogin2OutputDto
{
    public string psecurity { get; set; }
    public long nonce { get; set; }
    public string ssecurity { get; set; }
    public string passToken { get; set; }
    public string userId { get; set; }
    public string cUserId { get; set; }
    public int securityStatus { get; set; }
    public string notificationUrl { get; set; }
    public int pwd { get; set; }
    public int child { get; set; }
    public int code { get; set; }
    public string result { get; set; }
    public string desc { get; set; }
    public string description { get; set; }
    public string location { get; set; }
    public object captchaUrl { get; set; }
}
