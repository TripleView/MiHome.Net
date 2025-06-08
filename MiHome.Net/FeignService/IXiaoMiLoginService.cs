using MiHome.Net.Dto;
using SummerBoot.Feign;
using SummerBoot.Feign.Attributes;

namespace MiHome.Net.FeignService;

//,
//"Cookie:sdkVersion=3.9;deviceId={{deviceId}}")
[FeignClient(Url = "https://account.xiaomi.com/")]
[Headers("User-Agent:APP/com.xiaomi.mihome APPV/6.0.103 iosPassportSDK/3.9.0 iOS/14.4 miHSTS")]
public interface IXiaoMiLoginService
{

    [GetMapping("/pass/serviceLogin?sid=xiaomiio&_json=true")]
    Task<string> ServiceLogin(string deviceId);

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

    [PostMapping("/miotspec/prop/set")]
    Task<string> PropSet([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    [PostMapping("/miotspec/prop/get")]
    Task<string> PropGet([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    [PostMapping("/miotspec/action")]
    Task<string> ActionCall([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
    [PostMapping("user/get_user_device_data")]
    Task<string> GetUserDeviceData([Body(BodySerializationKind.Form)] Dictionary<string, string> dto);
}
