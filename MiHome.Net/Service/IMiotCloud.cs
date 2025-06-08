using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using MiHome.Net.Dto;
using MiHome.Net.FeignService;
using MiHome.Net.Utils;
using Newtonsoft.Json;
using SummerBoot.Cache;
using SummerBoot.Core;
using SummerBoot.Feign;

namespace MiHome.Net.Service;
/// <summary>
/// 米家云端接口
/// </summary>
public interface IMiotCloud
{
    /// <summary>
    /// Get device specification information获取设备规格信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<MiotSpec> GetDeviceSpec(string model);

    /// <summary>
    /// Get a list of home devices获取家庭设备列表
    /// </summary>
    /// <returns></returns>
    Task<List<XiaoMiDeviceInfo>> GetDeviceListAsync();
    /// <summary>
    /// Get properties in batches批量获取属性
    /// </summary>
    /// <param Name="param"></param>
    /// <returns></returns>
    Task<List<GetPropOutputItemDto>> GetPropertiesAsync(List<GetPropertyDto> properties);
    /// <summary>
    /// get property获取属性 
    /// </summary>
    /// <param Name="param"></param>
    /// <returns></returns>
    Task<GetPropOutputItemDto> GetPropertyAsync(GetPropertyDto property);

    /// <summary>
    /// set property设置属性
    /// </summary>
    /// <param Name="param"></param>
    /// <returns></returns>
    Task<SetPropOutputItemDto> SetPropertyAsync(SetPropertyDto property);
    /// <summary>
    /// Set properties in batches 批量设置属性
    /// </summary>
    /// <param Name="param"></param>
    /// <returns></returns>
    Task<List<SetPropOutputItemDto>> SetPropertiesAsync(List<SetPropertyDto> properties);

    /// <summary>
    /// Call device method调用设备方法
    /// </summary>
    /// <param Name="callActionParam"></param>
    /// <returns></returns>
    Task<string> CallActionAsync(CallActionInputDto callActionParam);
}

[AutoRegister(typeof(IMiotCloud))]
public class MIotCloud : IMiotCloud
{
    private readonly IMiotCloudService miotCloudService;
    private readonly IXiaoMiLoginService xiaoMiLoginService;
    private readonly IXiaoMiControlDevicesService xiaoMiControlDevicesService;
    private readonly IFeignUnitOfWork fegiFeignUnitOfWork;
    private readonly ICache cache;
    private readonly ILogger<MiHomeDriver> logger;
    private readonly MiHomeAccountOption option;

    public MIotCloud(IMiotCloudService miotCloudService, IXiaoMiLoginService xiaoMiLoginService, IXiaoMiControlDevicesService xiaoMiControlDevicesService, IFeignUnitOfWork fegiFeignUnitOfWork, ICache cache, ILogger<MiHomeDriver> logger, MiHomeAccountOption option)
    {
        this.miotCloudService = miotCloudService;
        this.xiaoMiLoginService = xiaoMiLoginService;
        this.xiaoMiControlDevicesService = xiaoMiControlDevicesService;
        this.fegiFeignUnitOfWork = fegiFeignUnitOfWork;
        this.cache = cache;
        this.logger = logger;
        this.option = option;
    }
    public async Task<MiotSpec> GetDeviceSpec(string model)
    {
        var modelSchema = await GetModelSchema(model);
        return modelSchema;
    }

    private async Task<MiotSpec> GetModelSchema(string model)
    {
        var allInstances = await GetAllInstancesAsync();
        var modelInfo = allInstances.Instances.Where(it => it.Model == model).MaxBy(it => it.Version);
        if (modelInfo == null)
        {
            throw new Exception($"Device(model:{model} ) information not found");
        }

        var miotSpec = await GetSpecByDeviceType(modelInfo.Type, model);
        return miotSpec;
    }

    private async Task<GetAllInstanceResult> GetAllInstancesAsync()
    {
        var cacheFilePath = Path.Combine(AppContext.BaseDirectory, "allInstance.json");
        var fi = new FileInfo(cacheFilePath);
        if (fi.Exists && (DateTime.Now - fi.CreationTime).TotalHours < 24)
        {
            using var sw = new StreamReader(fi.OpenRead());
            var allInstancesTxt = await sw.ReadToEndAsync();
            var result = JsonConvert.DeserializeObject<GetAllInstanceResult>(allInstancesTxt);
            return result;
        }
        else
        {
            if (fi.Exists)
            {
                fi.Delete();
                await Task.Delay(50);
            }

            var instances = await miotCloudService.GetAllInstancesAsync();
            using var sw = new StreamWriter(fi.OpenWrite());
            await sw.WriteAsync(instances.ToJson());
            return instances;
        }
    }

    private async Task<MiotSpec> GetSpecByDeviceType(string deviceType, string model)
    {
        var cacheFilePath = Path.Combine(AppContext.BaseDirectory, model + ".json");
        var fi = new FileInfo(cacheFilePath);
        if (fi.Exists && (DateTime.Now - fi.CreationTime).TotalHours < 24)
        {
            using var sw = new StreamReader(fi.OpenRead());
            var miotSpecTxt = await sw.ReadToEndAsync();
            var result = JsonConvert.DeserializeObject<MiotSpec>(miotSpecTxt);
            return result;
        }
        else
        {
            if (fi.Exists)
            {
                fi.Delete();
                await Task.Delay(50);
            }

            var miotSpec = await miotCloudService.GetSpecByDeviceType(deviceType);
            using var sw = new StreamWriter(fi.OpenWrite());
            await sw.WriteAsync(miotSpec.ToJson());
            return miotSpec;
        }
    }

    /// <summary>
    ///关闭控制设备的cookie
    /// </summary>
    /// <returns></returns>
    private async Task StopControlDeviceCookieAsync()
    {
        fegiFeignUnitOfWork.StopCookie();
    }

    /// <summary>
    /// 开启控制设备的cookie
    /// </summary>
    /// <returns></returns>
    private async Task BeginControlDeviceCookieAsync()
    {
        var loginInfoDto = await GetLoginInfoAsync();

        fegiFeignUnitOfWork.BeginCookie();
        var apiUrl = "https://api.io.mi.com/app/";
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("userId", loginInfoDto.UserId));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("serviceToken", loginInfoDto.ServiceToken));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("yetAnotherServiceToken", loginInfoDto.ServiceToken));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("is_daylight", "0"));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("channel", "MI_APP_STORE"));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("dst_offset", "0"));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("locale", "zh_CN"));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("timezone", "GMT+08:00"));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("sdkVersion", "3.9"));
        fegiFeignUnitOfWork.AddCookie(apiUrl, new Cookie("deviceId", loginInfoDto.DeviceId));
    }

    /// <summary>
    /// 获取登录信息，从缓存或者真正登录获取
    /// </summary>
    /// <returns></returns>
    private async Task<LoginInfoDto> GetLoginInfoAsync()
    {
        LoginInfoDto loginInfoDto;

        var loginInfoResult = await cache.GetValueAsync<LoginInfoDto>("loginInfo");
        if (loginInfoResult.HasValue)
        {
            loginInfoDto = loginInfoResult.Data;
        }
        else
        {
            loginInfoDto = await GetInternalLoginInfoAsync();
            await cache.SetValueWithAbsoluteAsync("loginInfo", loginInfoDto, TimeSpan.FromHours(6));
        }

        return loginInfoDto;
    }


    /// <summary>
    /// 真正登录的代码
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<LoginInfoDto> GetInternalLoginInfoAsync()
    {
        if (option.UserName.IsNullOrWhiteSpace() || option.Password.IsNullOrWhiteSpace())
        {
            logger?.LogError("UserName or Password is empty!");
            throw new Exception("UserName or Password is empty!");
        }

        fegiFeignUnitOfWork.BeginCookie();

        var clientId = GetClientId();
        var url = "https://account.xiaomi.com/";
        fegiFeignUnitOfWork.AddCookie(url, new Cookie("sdkVersion", "3.9"));
        fegiFeignUnitOfWork.AddCookie(url, new Cookie("deviceId", clientId));

        var result = await xiaoMiLoginService.ServiceLogin(clientId);
        var startValue = "&&&START&&&";

        if (result.HasText() && result.StartsWith(startValue))
        {
            result = result.Replace(startValue, "");

            var resultObj = JsonConvert.DeserializeObject<ServiceLoginResultDto>(result);
            var param = new ServiceLoginAuth2InputDto()
            {
                _sign = resultObj._sign,
                callback = resultObj.callback,
                qs = resultObj.qs,
                sid = resultObj.sid,
                user = option.UserName,
                hash = ConvertStringtoMD5(option.Password).ToUpper()
            };
            var result2 = await xiaoMiLoginService.ServiceLoginAuth2(param);
            if (result2.HasText() && result2.StartsWith(startValue))
            {
                result2 = result2.Replace(startValue, "");

                var result2Obj = JsonConvert.DeserializeObject<ServiceLoginAuth2OutputDto>(result2);
                if (result2Obj == null)
                {
                    throw new Exception("获取登陆信息失败");
                }

                if (result2Obj.notificationUrl.HasText() && result2Obj.notificationUrl.Contains("verifyPhone"))
                {
                    throw new Exception("请先登陆米家app校验手机");
                }
                if (result2Obj.notificationUrl.HasText() && result2Obj.notificationUrl.Contains("identity/authStart"))
                {
                    var checkIdentityListUrl =
                        "https://account.xiaomi.com" + result2Obj.notificationUrl.Replace("identity/authStart", "identity/list");
                    var checkIdentityLisRes = await xiaoMiLoginService.CheckIdentityList(checkIdentityListUrl);
                    checkIdentityLisRes.EnsureSuccessStatusCode();
                    var checkIdentityListCookies = checkIdentityLisRes.Headers.Where(it => it.Key == "Set-Cookie").SelectMany(it => it.Value).ToList()
                        .Select(it => it.Split(";")[0]).ToList();
                    if (checkIdentityListCookies.Count == 0)
                    {
                        throw new Exception("Get ServiceToken Error");
                    }

                    var identitySession = checkIdentityListCookies.FirstOrDefault(it => it.StartsWith("identity_session"))
                            ?.Replace("identity_session=", "");

                    if (identitySession.IsNullOrWhiteSpace())
                    {
                        throw new Exception("Get identitySession Error");
                    }

                    var checkIdentityListString = await checkIdentityLisRes.Content.ReadAsStringAsync();

                    if (checkIdentityListString.HasText() && checkIdentityListString.StartsWith(startValue))
                    {
                        checkIdentityListString = checkIdentityListString.Replace(startValue, "");
                        var checkIdentityLisObj = JsonConvert.DeserializeObject<CheckIdentityLisResult>(checkIdentityListString);
                        if (checkIdentityLisObj == null)
                        {
                            throw new Exception("获取checkIdentityListString失败");
                        }

                        var options = checkIdentityLisObj.options.ToList();
                        if (options.Count == 0)
                        {
                            options = new List<int>() { checkIdentityLisObj.flag };
                        }

                        var verifyDic = new Dictionary<int, string>()
                        {
                            { 4, "identity/auth/verifyPhone" },
                            { 8, "identity/auth/verifyEmail" },
                        };

                        foreach (var option in options)
                        {
                            var isGet = verifyDic.TryGetValue(option, out var verifyUrl);
                            if (isGet)
                            {
                                var dc = (int)DateTime.UtcNow.UtcDateTimeToUnixTimeStamp() * 1000;
                                var ffff = await xiaoMiLoginService.VerifyTicket(verifyUrl, new LoginVerifyTicketInputDto()
                                {
                                    _flag = option,
                                    _json = true,
                                    trust = true,
                                    ticket = ""
                                }, dc.ToString());
                                checkIdentityLisRes.EnsureSuccessStatusCode();
                                var fdsfsdf = await ffff.Content.ReadAsStringAsync();
                            }
                        }

                    }
                }

                //var clientSign = "";
                //var temp = $"nonce={result2Obj.nonce}&{result2Obj.ssecurity}";
                //var gearr = Encoding.UTF8.GetBytes(temp);
                //var geresult = gearr.ToSha1ThenBase64();
                //result2Obj.location = result2Obj.location + $"&clientSign={geresult}";

                var result3 = await xiaoMiLoginService.Login(result2Obj.location);
                result3.EnsureSuccessStatusCode();
                var cookies = result3.Headers.Where(it => it.Key == "Set-Cookie").SelectMany(it => it.Value).ToList()
                    .Select(it => it.Split(";")[0]).ToList();
                if (cookies.Count == 0)
                {
                    throw new Exception("Get ServiceToken Error");
                }

                var serviceToken = cookies.FirstOrDefault(it => it.StartsWith("serviceToken"))
                    ?.Replace("serviceToken=", "");
                fegiFeignUnitOfWork.StopCookie();
                var loginInfo = new LoginInfoDto()
                {
                    DeviceId = clientId,
                    ServiceToken = serviceToken,
                    Ssecurity = result2Obj.ssecurity,
                    UserId = result2Obj.userId
                };
                return loginInfo;
            }
        }

        var errorMsg = "login fail";
        logger?.LogError(errorMsg);
        throw new Exception(errorMsg);
    }

    private string GetClientId()
    {
        var letters = "ABCDEF";
        var resultList = new List<string>();
        for (int i = 0; i < 13; i++)
        {
            var index = RandomNumberGenerator.GetInt32(0, 6);
            var letter = letters[index].ToString();
            resultList.Add(letter);
        }

        var result = string.Join("", resultList);
        return result;
    }

    /// <summary>
    /// 对参数添加rc4加密
    /// </summary>
    /// <param Name="method"></param>
    /// <param Name="url"></param>
    /// <param Name="data"></param>
    /// <param Name="ssecurity"></param>
    /// <returns></returns>
    private Dictionary<string, string> GetRc4Params(string method, string url, object data, string ssecurity)
    {
        Dictionary<string, string> dat = new Dictionary<string, string>();
        dat["data"] = data.ToJson();

        var nonce = CalculateNonce();
        var signedNonce = SignedNonce(ssecurity, nonce);
        dat["rc4_hash__"] = Sha1Sign(url, dat, signedNonce, method);
        foreach (var pair in dat)
        {
            dat[pair.Key] = EncryptData(signedNonce, pair.Value);
        }

        dat["signature"] = Sha1Sign(url, dat, signedNonce, method);
        dat["_nonce"] = nonce;
        dat["ssecurity"] = ssecurity;
        dat["signedNonce"] = signedNonce;
        return dat;
    }

    /// <summary>
    /// 对数据使用sha1进行加密
    /// </summary>
    /// <param Name="url"></param>
    /// <param Name="dat"></param>
    /// <param Name="nonce"></param>
    /// <param Name="method"></param>
    /// <returns></returns>
    private string Sha1Sign(string url, Dictionary<string, string> dat, string nonce, string method = "POST")
    {
        Uri uri = new Uri(url);
        var path = uri.AbsolutePath;
        if (path.Length > 5 && path.Substring(0, 5) == "/app/")
        {
            path = path.Substring(4);
        }

        var arr = new List<string>();
        arr.Add(method.ToUpper());
        arr.Add(path);
        foreach (var pair in dat)
        {
            arr.Add($"{pair.Key}={pair.Value}");
        }

        arr.Add(nonce);
        var sign = string.Join("&", arr);
        var result = sign.ToSha1ThenBase64();
        return result;
    }

    /// <summary>
    /// 加密消息体
    /// </summary>
    /// <param Name="key"></param>
    /// <param Name="data"></param>
    /// <returns></returns>
    private string EncryptData(string key, string data)
    {
        var p = new Rc4(key).Init1024().Crypt(data.GetBytes()).ToBase64();
        return p;
    }

    /// <summary>
    /// 解密消息体
    /// </summary>
    /// <param Name="key"></param>
    /// <param Name="data"></param>
    /// <returns></returns>
    private string DecryptData(string key, string data)
    {
        var baseData = Convert.FromBase64String(data);
        var p = new Rc4(key).Init1024().Crypt(baseData);
        return Encoding.UTF8.GetString(p);
    }

    /// <summary>
    /// 生成校验码
    /// </summary>
    /// <param Name="secret"></param>
    /// <param Name="nonce"></param>
    /// <returns></returns>
    private string SignedNonce(string secret, string nonce)
    {
        var secretBytes = Convert.FromBase64String(secret);
        var nonceBytes = Convert.FromBase64String(nonce);
        var finalBytes = secretBytes.Concat(nonceBytes).ToArray();
        var finalResult = finalBytes.ToSha256ThenBase64();
        return finalResult;
    }

    /// <summary>
    /// 获取nonce一次性随机数
    /// </summary>
    /// <returns></returns>
    private string CalculateNonce()
    {
        //Allocate a buffer
        var firstByteArray = new byte[8];
        //Generate a cryptographically random set of bytes
        using (var Rnd = RandomNumberGenerator.Create())
        {
            Rnd.GetBytes(firstByteArray);
        }

        var secondByteArr = ((int)(SbUtil.CurrentSeconds() / 60)).ToBytesBig(4);
        var finalByteArr = firstByteArray.Concat(secondByteArr).ToArray();
        //Base64 encode and then return
        return Convert.ToBase64String(finalByteArr);
    }

    private string ConvertStringtoMD5(string strword)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(strword);
        byte[] hash = md5.ComputeHash(inputBytes);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public async Task<List<XiaoMiDeviceInfo>> GetDeviceListAsync()
    {
        await BeginControlDeviceCookieAsync();
        var loginInfoDto = await GetLoginInfoAsync();
        var inputDto = new GetDeviceInputDto()
        {
            GetVirtualModel = true,
            GetHuamiDevices = 1,
            Get_split_device = false,
            Support_smart_home = true
        };
        var param = GetRc4Params("POST", "https://api.io.mi.com/app/home/device_list", inputDto, loginInfoDto.Ssecurity);
        var signedNonce = param["signedNonce"];
        var deviceListResultString = await xiaoMiControlDevicesService.GetDeviceList(param);
        await StopControlDeviceCookieAsync();
        var decryptData = DecryptData(signedNonce, deviceListResultString);
        var result = JsonConvert.DeserializeObject<GetDeviceListOutputResult>(decryptData);
        if (result?.Code == 0)
        {
            return result.Result.List;
        }

        var errorMsg = $"get device List error,reason:{result?.Message}";
        logger?.LogError(errorMsg);
        throw new Exception(errorMsg);

    }

    public async Task<List<GetPropOutputItemDto>> GetPropertiesAsync(List<GetPropertyDto> properties)
    {
        await BeginControlDeviceCookieAsync();
        var loginInfoDto = await GetLoginInfoAsync();
        var postData = new GetPropPostData()
        {
            Params = properties
        };
        var param = GetRc4Params("POST", "https://api.io.mi.com/app/miotspec/prop/get", postData,
            loginInfoDto.Ssecurity);
        var signedNonce = param["signedNonce"];
        var resultString = await xiaoMiControlDevicesService.PropGet(param);
        await StopControlDeviceCookieAsync();
        var decryptData = DecryptData(signedNonce, resultString);
        var result = JsonConvert.DeserializeObject<GetPropOutputDto>(decryptData);
        if (result?.Code == 0)
        {
            return result.Result;
        }

        var errorMsg = $"propGet error,reason:{result?.Message}";
        logger?.LogError(errorMsg);
        throw new Exception(errorMsg);
    }

    public async Task<GetPropOutputItemDto> GetPropertyAsync(GetPropertyDto property)
    {
        var result = await this.GetPropertiesAsync(new List<GetPropertyDto>() { property });
        return result.First();
    }

    public async Task<string> CallActionAsync(CallActionInputDto callActionParam)
    {
        await BeginControlDeviceCookieAsync();
        var loginInfoDto = await GetLoginInfoAsync();
        var postData = new DoActionPostData()
        {
            AccessKey = "",
            Params = callActionParam
        };
        var param = GetRc4Params("POST", "https://api.io.mi.com/app/miotspec/action", postData,
            loginInfoDto.Ssecurity);
        var signedNonce = param["signedNonce"];
        var resultString = await xiaoMiControlDevicesService.ActionCall(param);
        await StopControlDeviceCookieAsync();
        var decryptData = DecryptData(signedNonce, resultString);
        var result = JsonConvert.DeserializeObject<CallActionOutputDto>(decryptData);
        if (result?.Code == 0)
        {
            return "";
        }

        var errorMsg = $"propGet error,reason:{result?.Message}";
        logger?.LogError(errorMsg);
        throw new Exception(errorMsg);
    }

    public async Task<List<SetPropOutputItemDto>> SetPropertiesAsync(List<SetPropertyDto> properties)
    {
        await BeginControlDeviceCookieAsync();
        var loginInfoDto = await GetLoginInfoAsync();
        var postData = new SetPropPostData()
        {
            Params = properties
        };

        var param = GetRc4Params("POST", "https://api.io.mi.com/app/miotspec/prop/set", postData,
            loginInfoDto.Ssecurity);
        var signedNonce = param["signedNonce"];
        var resultString = await xiaoMiControlDevicesService.PropSet(param);
        await StopControlDeviceCookieAsync();
        var decryptData = DecryptData(signedNonce, resultString);
        var result = JsonConvert.DeserializeObject<SetPropOutputDto>(decryptData);
        if (result?.Code == 0)
        {
            return result.Result;
        }

        var errorMsg = $"propGet error,reason:{result?.Message}";
        logger?.LogError(errorMsg);
        throw new Exception(errorMsg);
    }

    public async Task<SetPropOutputItemDto> SetPropertyAsync(SetPropertyDto property)
    {
        var result = await this.SetPropertiesAsync(new List<SetPropertyDto>() { property });
        return result.First();
    }
}