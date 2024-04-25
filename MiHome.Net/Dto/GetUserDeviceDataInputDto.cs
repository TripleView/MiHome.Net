namespace MiHome.Net.Dto;

public class GetUserDeviceDataInputDto
{
    public string Did { get; set; }
    public string Key { get; set; }
    public string Type { get; set; }
    public long time_start { get; set; }

    public long time_end { get; set; }
    public int Limit { get; set; }

    public string Uid { get; set; }
}