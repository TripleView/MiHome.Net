namespace MiHome.Net.Dto;

public class GetPropertyDto
{
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }
    /// <summary>
    /// 属性id propertyId
    /// </summary>
    public int Piid { get; set; }
    /// <summary>
    /// 服务id serviceId
    /// </summary>
    public int Siid { get; set; }
}