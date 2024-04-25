namespace MiHome.Net.Dto;

public class ServiceLoginAuth2OutputDto
{
    public string ssecurity { get; set; }
    public string userId { get; set; }

    public string cUserId { get; set; }
    public string passToken { get; set; }

    public string location { get; set; }

    public string code { get; set; }
    public string nonce { get; set; }
}