using Newtonsoft.Json;

namespace MiHome.Net.Dto;

public class GetSceneOutputResultDto
{
    public int Code { get; set; }
    public string Message { get; set; }
    public Result Result { get; set; }
}

public class Result
{
    /// <summary>
    /// 额外json
    /// </summary>
    [JsonProperty("scene_info_list")]
    public List<SceneDto> SceneList { get; set; }
    
    public int distance_ceiling { get; set; }
}

/// <summary>
/// 场景dto
/// </summary>
public class SceneDto
{
    /// <summary>
    /// 场景id
    /// </summary>
    [JsonProperty("scene_id")]
    public string SceneId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Uid { get; set; }
    /// <summary>
    /// 家庭id
    /// </summary>
    [JsonProperty("home_id")]
    public string HomeId { get; set; }
    /// <summary>
    /// 场景名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 模板id
    /// </summary>
    [JsonProperty("template_id")]
    public string TemplateId { get; set; }
    /// <summary>
    /// 场景类型
    /// </summary>
    public int Type { get; set; }
    [JsonProperty("local_dev")]
    public string LocalDev { get; set; }
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enable { get; set; }
    [JsonProperty("enable_push")]
    public bool EnablePush { get; set; }
    [JsonProperty("common_use")]
    public bool CommonUse { get; set; }
    public object Timespan { get; set; }
    /// <summary>
    /// 场景绑定的所有触发器
    /// </summary>
    [JsonProperty("scene_trigger")]
    public SceneTriggerBinding SceneTrigger { get; set; }
    /// <summary>
    ///  场景绑定的所有条件
    /// </summary>
    [JsonProperty("scene_condition")]
    public SceneConditionBinding SceneCondition { get; set; }
    /// <summary>
    ///  场景绑定的所有动作
    /// </summary>
    [JsonProperty("scene_action")]
    public SceneActionBinding SceneAction { get; set; }
    /// <summary>
    /// 值格式化
    /// </summary>
    [JsonProperty("value_format")]
    public int ValueFormat { get; set; }
    /// <summary>
    /// 时间窗口
    /// </summary>
    public TimeWindow TimeWindow { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    [JsonProperty("create_time")]
    public string CreateTime { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    [JsonProperty("update_time")]
    public string UpdateTime { get; set; }
    /// <summary>
    /// 子id
    /// </summary>
    [JsonProperty("sub_usIds")]
    public List<string> SubUsIds { get; set; }
    /// <summary>
    /// 是否不报告日志
    /// </summary>
    [JsonProperty("no_record_log")]
    public bool NoRecordLog { get; set; }
    /// <summary>
    /// 父id
    /// </summary>
    [JsonProperty("parent_usId")]
    public string ParentUsId { get; set; }
    [JsonProperty("scene_id_v1")]
    public string SceneIdV1 { get; set; }
    /// <summary>
    /// 房间id
    /// </summary>
    [JsonProperty("room_id")]
    public string RoomId { get; set; }
    /// <summary>
    /// 通用使用过的房间id列表
    /// </summary>
    [JsonProperty("common_used_roomIds")]
    public List<string> CommonUsedRoomIds { get; set; }
    /// <summary>
    /// 标签
    /// </summary>
    public Tags Tags { get; set; }
    /// <summary>
    /// 附加信息
    /// </summary>
    public string Extra { get; set; }
    /// <summary>
    /// 图标链接
    /// </summary>
    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }
    [JsonProperty("enable_consist")]
    public bool EnableConsist { get; set; }
    /// <summary>
    /// 场景版本
    /// </summary>
    [JsonProperty("scene_version")]
    public int SceneVersion { get; set; }
    /// <summary>
    /// 是否为临时场景
    /// </summary>
    public bool Temporary { get; set; }
    /// <summary>
    /// 数据类型
    /// </summary>
    [JsonProperty("data_type")]
    public int DataType { get; set; }
    /// <summary>
    /// 鸿蒙额外信息
    /// </summary>
    [JsonProperty("hm_extra_info")]
    public string HmExtraInfo { get; set; }
}

/// <summary>
/// 场景绑定的所有触发器
/// </summary>
public class SceneTriggerBinding
{
    public int Express { get; set; }
    /// <summary>
    /// 触发器
    /// </summary>
    public List<Trigger> Triggers { get; set; }
}

/// <summary>
/// 触发器
/// </summary>
public class Trigger
{
    public int Id { get; set; }
    /// <summary>
    /// 排序
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// 来源
    /// </summary>
    public string Src { get; set; }
    /// <summary>
    /// 主键
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// 附加信息
    /// </summary>
    public string Extra { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// 值类型
    /// </summary>
    [JsonProperty("value_type")]
    public int ValueType { get; set; }
    /// <summary>
    /// 额外json
    /// </summary>
    [JsonProperty("extra_json")]
    public ExtraJson ExtraJson { get; set; }
    /// <summary>
    /// 值json
    /// </summary>
    [JsonProperty("value_json")]
    public object ValueJson { get; set; }
    /// <summary>
    /// 协议类型
    /// </summary>
    [JsonProperty("protocol_type")]
    public int ProtocolType { get; set; }
    [JsonProperty("sc_id")]
    public int ScId { get; set; }
    /// <summary>
    /// 来源
    /// </summary>
    public int From { get; set; }
    /// <summary>
    /// 值操作
    /// </summary>
    [JsonProperty("value_operation")]
    public int ValueOperation { get; set; }
    /// <summary>
    /// 标准scid
    /// </summary>
    [JsonProperty("std_sc_id")]
    public string StdScId { get; set; }
    public int Duration { get; set; }
}

/// <summary>
/// 场景额外信息
/// </summary>
public class ExtraJson
{
    /// <summary>
    /// 设备名称
    /// </summary>
    [JsonProperty("device_name")]
    public string DeviceName { get; set; }
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }
    /// <summary>
    /// 设备型号
    /// </summary>
    public string Model { get; set; }
    /// <summary>
    /// 时区
    /// </summary>
    [JsonProperty("time_zone")]
    public string TimeZone { get; set; }
    /// <summary>
    /// 语音别名
    /// </summary>
    [JsonProperty("voice_alias")]
    public VoiceAlias VoiceAlias { get; set; }
}

/// <summary>
/// 语音别名
/// </summary>
public class VoiceAlias
{
    /// <summary>
    /// 别名列表
    /// </summary>
    public List<string> Alias { get; set; }
    /// <summary>
    /// 使用名称
    /// </summary>
    [JsonProperty("use_title")]
    public int UseTitle { get; set; }
}

/// <summary>
///  场景绑定的所有条件
/// </summary>
public class SceneConditionBinding
{
    public int Express { get; set; }
    public object[] Conditions { get; set; }
}

/// <summary>
/// 场景绑定的所有动作
/// </summary>
public class SceneActionBinding
{
    /// <summary>
    /// 模式
    /// </summary>
    public int Mode { get; set; }
    /// <summary>
    /// 动作列表
    /// </summary>
    public List<SceneAction> Actions { get; set; }
}

/// <summary>
/// 实际执行的动作
/// </summary>
public class SceneAction
{
    /// <summary>
    /// 分组id
    /// </summary>
    [JsonProperty("group_id")]
    public int GroupId { get; set; }
    /// <summary>
    /// 动作id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 动作排序
    /// </summary>
    public int Order { get; set; }
    /// <summary>
    /// 动作类型
    /// </summary>
    public int Type { get; set; }
    /// <summary>
    /// 动作名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 载荷
    /// </summary>
    public string Payload { get; set; }
    /// <summary>
    /// 载荷json
    /// </summary>
    [JsonProperty("payload_json")]
    public PayloadJson PayloadJson { get; set; }
    /// <summary>
    /// 协议类型
    /// </summary>
    [JsonProperty("protocol_type")]
    public int ProtocolType { get; set; }
    /// <summary>
    /// SaId
    /// </summary>
    [JsonProperty("sa_id")]
    public int SaId { get; set; }
    /// <summary>
    /// 来源
    /// </summary>
    public int From { get; set; }
    /// <summary>
    /// 设备分组id
    /// </summary>
    [JsonProperty("device_group_id")]
    public int DeviceGroupId { get; set; }
    /// <summary>
    /// 嵌入场景的信息
    /// </summary>
    [JsonProperty("nested_scene_info")]
    public object NestedSceneInfo { get; set; }
    /// <summary>
    /// 标准SaId
    /// </summary>
    [JsonProperty("std_sa_id")]
    public string StdSaId { get; set; }
}

/// <summary>
/// 载荷json
/// </summary>
public class PayloadJson
{
    /// <summary>
    /// 命令
    /// </summary>
    public string Command { get; set; }
    /// <summary>
    /// 延迟时间
    /// </summary>
    [JsonProperty("delay_time")]
    public int DelayTime { get; set; }
    /// <summary>
    /// 设备名称
    /// </summary>
    [JsonProperty("device_name")]
    public string DeviceName { get; set; }
    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }
    /// <summary>
    /// 设备型号
    /// </summary>
    public string Model { get; set; }
    /// <summary>
    /// 真正执行的命令的参数
    /// </summary>
    public object Value { get; set; }
}

public class TimeWindow
{
    /// <summary>
    /// 起始时间
    /// </summary>
    public string From { get; set; }
    /// <summary>
    /// 结束时间
    /// </summary>
    public string To { get; set; }
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string Filter { get; set; }
    /// <summary>
    /// 附加信息
    /// </summary>
    public string Extra { get; set; }
    /// <summary>
    /// 时区
    /// </summary>
    [JsonProperty("time_zone")]
    public string TimeZone { get; set; }
}

public class Tags
{
}
