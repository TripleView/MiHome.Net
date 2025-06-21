namespace MiHome.Net.Dto;

public class Extra
{
    /// <summary>
    /// 
    /// </summary>
    public long IsSetPincode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long PinCodeType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Fw_Version { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long NeedVerifyCode { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long IsPasswordEncrypt { get; set; }
}

public class XiaoMiDeviceInfo
{

    /// <summary>
    /// 设备id
    /// </summary>
    public string Did { get; set; }
    /// <summary>
    /// 设备token值
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Longitude { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Latitude { get; set; }
    /// <summary>
    /// 设备名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Pid { get; set; }
    /// <summary>
    /// 局域网ip
    /// </summary>
    public string LocalIp { get; set; }
    /// <summary>
    /// 设备mac地址
    /// </summary>
    public string Mac { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Ssid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Bssid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Parent_Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Parent_Model { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long Show_Mode { get; set; }
    /// <summary>
    /// Model型号
    /// </summary>
    public string Model { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long AdminFlag { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long ShareFlag { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long PermitLevel { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool IsOnline { get; set; }
    /// <summary>
    /// 设备在线 
    /// </summary>
    public string Desc { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Extra Extra { get; set; }
    /// <summary>
    /// 家庭id
    /// </summary>
    public long Uid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long Pd_id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string P2p_id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long Rssi { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long Family_Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long Reset_Flag { get; set; }
}

public class VirtualModelsItem
{

    /// <summary>
    /// 
    /// </summary>
    public string Model { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long State { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Url { get; set; }
}

public class GetDeviceListOutputResultItem
{

    /// <summary>
    /// 
    /// </summary>
    public List<XiaoMiDeviceInfo> List { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<VirtualModelsItem> VirtualModels { get; set; }
}

public class GetDeviceListOutputResult
{

    /// <summary>
    /// 
    /// </summary>
    public long Code { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public GetDeviceListOutputResultItem Result { get; set; }
}

