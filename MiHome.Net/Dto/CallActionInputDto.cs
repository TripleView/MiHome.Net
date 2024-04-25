namespace MiHome.Net.Dto;

public class CallActionInputDto
{
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }

    public int Aiid { get; set; }
    public int Siid { get; set; }

    public List<string> In { get; set; }
}


