using Newtonsoft.Json;

namespace MiHome.Net.Dto;

public class GetHomeOutputResultDto
{
    public int Code { get; set; }
    public string Message { get; set; }
    public GetHomeOutputDto Result { get; set; }
}

public class GetHomeOutputDto
{
    /// <summary>
    /// 家庭列表
    /// </summary>
    public List<HomeDto> HomeList { get; set; }
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }
    [JsonProperty("max_id")]
    public string MaxId { get; set; }
}

/// <summary>
/// 家庭
/// </summary>
public class HomeDto
{
    public string Id { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    public string Bssid { get; set; }
    public List<string> Dids { get; set; }
    [JsonProperty("temp_dids")]
    public object TempDids { get; set; }
    /// <summary>
    /// 家庭图标
    /// </summary>
    public string Icon { get; set; }
    public int ShareFlag { get; set; }
    [JsonProperty("permit_level")]
    public int PermitLevel { get; set; }
    public int Status { get; set; }
    /// <summary>
    /// 米家里家庭的背景
    /// </summary>
    public string Background { get; set; }
    [JsonProperty("smart_room_background")]
    public string SmartRoomBackground { get; set; }
    /// <summary>
    /// 家庭地理位置，经度
    /// </summary>
    public float Longitude { get; set; }
    /// <summary>
    /// 家庭地理位置，纬度
    /// </summary>
    public float Latitude { get; set; }
    /// <summary>
    /// 所在城市id
    /// </summary>
    [JsonProperty("city_id")]
    public int CityId { get; set; }
    /// <summary>
    /// 家庭地址
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    [JsonProperty("create_time")]
    public int CreateTime { get; set; }

    /// <summary>
    /// 房间列表
    /// </summary>
    public List<RoomDto> RoomList { get; set; }
    /// <summary>
    /// uid
    /// </summary>
    public long Uid { get; set; }
    [JsonProperty("appear_home_list")]
    public object AppearHomeList { get; set; }
    [JsonProperty("popup_flag")]
    public int PopupFlag { get; set; }

    [JsonProperty("popup_time_stamp")]
    public int PopupTimeStamp { get; set; }

    [JsonProperty("car_did")]
    public string CarDid { get; set; }
}

/// <summary>
/// 房间
/// </summary>
public class RoomDto
{
    public string Id { get; set; }
    /// <summary>
    /// 房间名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Bssid { get; set; }
    /// <summary>
    /// 上级id，一般是家庭的id
    /// </summary>
    public string Parentid { get; set; }
    /// <summary>
    /// 房间里设备的did列表
    /// </summary>
    public List<string> Dids { get; set; }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }
    /// <summary>
    /// 房间背景
    /// </summary>
    public string Background { get; set; }
    public int Shareflag { get; set; }
    /// <summary>
    /// 创建时间
    /// </summary>
    [JsonProperty("create_time")]
    public int CreateTime { get; set; }
}
