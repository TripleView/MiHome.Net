namespace MiHome.Net.Dto;

public class SetPropertyDto
{
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }

    public int Piid { get; set; }
    public int Siid { get; set; }
    public object Value { get; set; }
}