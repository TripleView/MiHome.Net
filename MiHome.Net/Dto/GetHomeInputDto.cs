namespace MiHome.Net.Dto;

/// <summary>
/// 获取家庭列表dto
/// </summary>
public class GetHomeInputDto
{
    public bool fg { get; set; }
    public bool fetch_share { get; set; }
    public bool fetch_share_dev { get; set; }

    public int limit { get; set; }
    public int app_ver { get; set; }
}