using MiHome.Net.Dto;
using SummerBoot.Feign;
using SummerBoot.Feign.Attributes;

namespace MiHome.Net.FeignService;

//,
//"Cookie:sdkVersion=3.9;deviceId={{deviceId}}")
[FeignClient(Url = "https://account.xiaomi.com/",Timeout = 60)]
[Headers("User-Agent:APP/com.xiaomi.mihome APPV/6.0.103 iosPassportSDK/3.9.0 iOS/14.4 miHSTS")]
public interface IXiaoMiLoginService
{

    [GetMapping("/pass/serviceLogin?sid=xiaomiio&_json=true")]
    Task<string> ServiceLogin(string deviceId);

    [GetMapping("/longPolling/loginUrl")]
    Task<string> LoginUrl([Query] QrCodeLoginInputDto dto);

    [Headers("Connection:keep-alive")]
    [GetMapping("{{url}}", UsePathAsUrl = true)]
    Task<string> QrCodeLogin(string url);

    [PostMapping("/pass/serviceLoginAuth2?_json=true")]
    Task<string> ServiceLoginAuth2([Body(SerializationKind = BodySerializationKind.Form)] ServiceLoginAuth2InputDto dto);

    [PostMapping("{{url}}?_dc={{dc}}")]
    Task<HttpResponseMessage> VerifyTicket(string url,[Body(SerializationKind = BodySerializationKind.Form)] LoginVerifyTicketInputDto dto,string dc);
    
    [GetMapping("{{url}}", UsePathAsUrl = true)]
    Task<HttpResponseMessage> CheckIdentityList(string url);

    [GetMapping("{{url}}", UsePathAsUrl = true)]
    Task<HttpResponseMessage> Login(string url);

}

//
[FeignClient(Url = "https://api.io.mi.com/app/")]
[Headers("User-Agent:APP/com.xiaomi.mihome APPV/6.0.103 iosPassportSDK/3.9.0 iOS/14.4 miHSTS",
    "X-XIAOMI-PROTOCAL-FLAG-CLI:PROTOCAL-HTTP2",
    "Accept-Encoding:identity", "Accept:*/*",
    "MIOT-ENCRYPT-ALGORITHM:ENCRYPT-RC4", "Connection:keep-alive")]
public interface IXiaoMiControlDevicesService
{
    [PostMapping("home/device_list")]
    Task<string> GetDeviceList([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    /// <summary>
    /// 获取家庭列表
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [PostMapping("/v2/homeroom/gethome")]
    Task<string> GetHomeList([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);

    [PostMapping("/miotspec/prop/set")]
    Task<string> PropSet([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    [PostMapping("/miotspec/prop/get")]
    Task<string> PropGet([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    [PostMapping("/miotspec/action")]
    Task<string> ActionCall([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    [PostMapping("user/get_user_device_data")]
    Task<string> GetUserDeviceData([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
}
