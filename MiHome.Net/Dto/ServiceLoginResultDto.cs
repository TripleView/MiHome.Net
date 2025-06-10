namespace MiHome.Net.Dto;

public class ServiceLoginResultDto
{
    public string qs { get; set; }
    public string sid { get; set; }
    public string callback { get; set; }
    public string _sign { get; set; }

    public string location { get; set; }
}