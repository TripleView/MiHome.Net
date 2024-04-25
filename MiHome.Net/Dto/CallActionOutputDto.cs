namespace MiHome.Net.Dto;

public class CallActionOutputDto
{
    /// <summary>
    /// 
    /// </summary>
    public long Code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Message { get; set; }

    public CallActionOutputItemDto Result { get; set; }
}

public class CallActionOutputItemDto
{
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }
    /// <summary>
    /// 模块id
    /// </summary>
    public int Miid { get; set; }
    /// <summary>
    /// 方法id
    /// </summary>
    public int Aiid { get; set; }
    /// <summary>
    /// 一级控制大类id
    /// </summary>
    public int Siid { get; set; }
   
    public string Code { get; set; }

    public long WithLatency { get; set; }
    public long Exe_time { get; set; }
}