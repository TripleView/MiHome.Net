namespace MiHome.Net.Dto;

public class RunSceneInputDto
{
    /// <summary>
    /// 场景id
    /// </summary>
    public string scene_id { get; set; }
    /// <summary>
    /// 触发方式
    /// </summary>
    public string trigger_key { get; set; }
}