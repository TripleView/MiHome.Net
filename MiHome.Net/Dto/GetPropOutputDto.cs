namespace MiHome.Net.Dto;

public class GetPropOutputDto
{
    /// <summary>
    /// 
    /// </summary>
    public long Code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Message { get; set; }

    public List<GetPropOutputItemDto> Result { get; set; }
}

public class GetPropOutputItemDto
{
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }
    /// <summary>
    /// 二级控制大类id
    /// </summary>
    public int Piid { get; set; }
    /// <summary>
    /// 一级控制大类id
    /// </summary>
    public int Siid { get; set; }
    /// <summary>
    /// 具体的值
    /// </summary>
    public object Value { get; set; }
    public string Code { get; set; }

    public long UpdateTime { get; set; }
    public long Exe_time { get; set; }
}