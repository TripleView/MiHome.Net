using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using MiHome.Net.Cache;
using MiHome.Net.Dto;
using MiHome.Net.FeignService;
using MiHome.Net.Utils;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

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

public class MIotCloud : IMiotCloud
{
    private readonly IMiotCloudService miotCloudService;
    private readonly IXiaoMiLoginService xiaoMiLoginService;
    private readonly IXiaoMiControlDevicesService xiaoMiControlDevicesService;
    private readonly ICache  cache;
    private readonly ILogger<MiHomeDriver> logger;
    private readonly MiHomeAccountOption option;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IMiotCloudService iMiotCloudService;

    public MIotCloud(IMiotCloudService miotCloudService, IXiaoMiLoginService xiaoMiLoginService, IXiaoMiControlDevicesService xiaoMiControlDevicesService, ICache cache, ILogger<MiHomeDriver> logger, MiHomeAccountOption option, IHttpClientFactory httpClientFactory, IMiotCloudService iMiotCloudService)
    {
        this.miotCloudService = miotCloudService;
        this.xiaoMiLoginService = xiaoMiLoginService;
        this.xiaoMiControlDevicesService = xiaoMiControlDevicesService;
        this.cache = cache;
        this.logger = logger;
        this.option = option;
        this.httpClientFactory = httpClientFactory;
        this.iMiotCloudService = iMiotCloudService;
    }
    public async Task<MiotSpec> GetDeviceSpec(string model)
    {
        var modelSchema = await GetModelSchema(model);
        return modelSchema;
    }

    private async Task<MiotSpec> GetModelSchema(string model)
    {
        var allInstances = await GetAllInstancesAsync();
        var modelInfo = allInstances.Instances.Where(it => it.Model == model).OrderByDescending(it => it.Version).FirstOrDefault();
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

            var instances = await iMiotCloudService.GetAllInstancesAsync();
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

    }

    /// <summary>
    /// 开启控制设备的cookie
    /// </summary>
    /// <returns></returns>
    private async Task BeginControlDeviceCookieAsync()
    {
        var loginInfoDto = await GetLoginInfoAsync();

        var dic = new Dictionary<string, string>();
        dic.Add("userId", loginInfoDto.UserId);
        dic.Add("serviceToken", loginInfoDto.ServiceToken);
        dic.Add("yetAnotherServiceToken", loginInfoDto.ServiceToken);
        dic.Add("is_daylight", "0");
        dic.Add("channel", "MI_APP_STORE");
        dic.Add("dst_offset", "0");
        dic.Add("locale", "zh_CN");
        dic.Add("timezone", "GMT+08:00");
        dic.Add("sdkVersion", "3.9");
        dic.Add("deviceId", loginInfoDto.DeviceId);
        await xiaoMiControlDevicesService.SetCookie(dic);

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

        //var c = loginInfoDto.ToJson();
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

        var clientId = GetClientId();
        var url = "https://account.xiaomi.com/";


        var dic = new Dictionary<string, string>();
        dic.Add("sdkVersion", "3.9");
        dic.Add("deviceId", clientId);

        await xiaoMiLoginService.SetCookie(dic);

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
                
                var result3 = await xiaoMiLoginService.Login(result2Obj.location);
                result3.EnsureSuccessStatusCode();
                var cookies = result3.Headers.Where(it => it.Key == "Set-Cookie").SelectMany(it => it.Value).ToList()
                    .Select(it => it.Split(';')[0]).ToList();
                if (cookies.Count == 0)
                {
                    throw new Exception("Get ServiceToken Error");
                }

                var serviceToken = cookies.FirstOrDefault(it => it.StartsWith("serviceToken"))
                    ?.Replace("serviceToken=", "");

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
        var Random = new Random();
        for (int i = 0; i < 13; i++)
        {
            var index = Random.Next(0, 6);
            //var index = RandomNumberGenerator.GetInt32(0, 6);
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
        //nonce = "1Xg55NnRgGYBtm/A";
        var signedNonce = SignedNonce(ssecurity, nonce);
        dat["rc4_hash__"] = Sha1Sign(url, dat, signedNonce, method);
        var newDat = new Dictionary<string, string>();
        foreach (var pair in dat)
        {
            newDat.Add(pair.Key, EncryptData(signedNonce, pair.Value));
          
        }

        foreach (var pair in newDat)
        {
            dat[pair.Key] = pair.Value;
        }



        dat["signature"] = Sha1Sign(url, dat, signedNonce, method);
        dat["_nonce"] = nonce;
        dat["ssecurity"] = ssecurity;
        dat["signedNonce"] = signedNonce;
        //var c = dat.ToJson();
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

        var seconds = DateTimeUtils.CurrentSeconds();

        var secondByteArr = ((int)(seconds / 60)).ToBytesBig(4);
        var finalByteArr = firstByteArray.Concat(secondByteArr).ToArray();
        //Base64 encode and then return
        var result= Convert.ToBase64String(finalByteArr);
        return result;
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
        //loginInfoDto.Ssecurity = "zDTRUvzVmlrNSIiaT4Z1/w==";
        var param = GetRc4Params("POST", "https://api.io.mi.com/app/home/device_list", inputDto, loginInfoDto.Ssecurity);
        //var cffff = param.ToJson();
        //param = JsonConvert.DeserializeObject<Dictionary<string, string>>(
        //    "{\"data\":\"G2ld8LvUP1J8RSDRz7S3k24duYX/6h/lQhlgUQwTzwwlNHvJcezcIP8+K6odtKRJ1HYK0vfSlX25UefZFAtNxi2b5c7zwv0kjJK1oGeXKKWY5kYzQmTRqOWqIiqZ/OI=\",\"rc4_hash__\":\"FHlC2uCxBG8+YXXTt76LoWtI5pnE2TuTMDlMGA==\",\"signature\":\"k5MD2qRO1+UWdyP2UYk0Kfi3Fmk=\",\"_nonce\":\"1Xg55NnRgGYBtm/A\",\"ssecurity\":\"0vk9nAzVavNMvnahav+S8g==\",\"signedNonce\":\"iJhbwk03C3ykSqqyNjRHoDwdk34QjIhwm14Zkdv1l5E=\"}");

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