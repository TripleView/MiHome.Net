namespace MiHome.Net.Dto;

public class GetAllInstanceResult
{
    public List<GetAllInstanceItem> Instances { get; set; }
}

public class GetAllInstanceItem
{
    public string Model { get; set; }
    public string Version { get; set; }

    public string Type { get; set; }
    public long Ts { get; set; }
}