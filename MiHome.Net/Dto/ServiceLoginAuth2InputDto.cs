namespace MiHome.Net.Dto;

public class ServiceLoginAuth2InputDto
{
    //public string _json { get; set; } = "true";
    public string user { get; set; }

    public string hash { get; set; }
    public string callback { get; set; }

    public string sid { get; set; }

    public string qs { get; set; }

    public string _sign { get; set; }
}