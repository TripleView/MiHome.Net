using System.Net;
using MiHome.Net.Dto;
using MiHome.Net.Utils;
using Newtonsoft.Json;

namespace MiHome.Net.FeignService;

public interface IXiaoMiLoginService
{

    Task<string> ServiceLogin(string deviceId);

    Task<string> ServiceLoginAuth2(ServiceLoginAuth2InputDto dto);

    Task<HttpResponseMessage> Login(string url);

    Task SetCookie(Dictionary<string, string> cookies);
}

public class XiaoMiLoginService : IXiaoMiLoginService
{
    private readonly IHttpClientFactory httpClientFactory;
    private Dictionary<string, string> cookies;
    public XiaoMiLoginService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<string> ServiceLogin(string deviceId)
    {
        var httpClient = GetClient();
        httpClient.BaseAddress = new Uri("https://account.xiaomi.com/");

        var httpResponse = await httpClient.GetAsync("pass/serviceLogin?sid=xiaomiio&_json=true");
        var temp = await httpResponse.Content.ReadAsStringAsync();
        return temp;
    }

    public async Task<HttpResponseMessage> Login(string url)
    {
        var httpClient = GetClient();

        var httpResponse = await httpClient.GetAsync(url);
        return httpResponse;
    }

    public async Task<string> ServiceLoginAuth2(ServiceLoginAuth2InputDto dto)
    {
        var httpClient = GetClient();
        httpClient.BaseAddress = new Uri("https://account.xiaomi.com/");

        var dic = new Dictionary<string, string>();
        dic.Add("_sign", dto._sign);
        dic.Add("callback", dto.callback);
        dic.Add("hash", dto.hash);
        dic.Add("qs", dto.qs);
        dic.Add("sid", dto.sid);
        dic.Add("user", dto.user);
        var httpContent = new FormUrlEncodedContent(dic);

        var httpResponse = await httpClient.PostAsync("pass/serviceLoginAuth2?_json=true", httpContent);
        var temp = await httpResponse.Content.ReadAsStringAsync();
        return temp;
    }

    private HttpClient GetClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "APP/com.xiaomi.mihome APPV/6.0.103 iosPassportSDK/3.9.0 iOS/14.4 miHSTS");
        if (this.cookies != null)
        {
            var key = "Cookie";
            var cookieList = new List<string>();
            foreach (var cookie in cookies)
            {
                cookieList.Add($"{cookie.Key}={Uri.EscapeUriString(cookie.Value)}");
            }
            var keyValue = string.Join("; ", cookieList);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, keyValue);
        }
        return httpClient;
    }

    public async Task SetCookie(Dictionary<string, string> cookies)
    {
        if (this.cookies != null)
        {
            this.cookies.Clear();
        }
        this.cookies = cookies;
    }
}

public interface IXiaoMiControlDevicesService
{
    Task<string> GetDeviceList(Dictionary<string, string> dto);


    Task<string> PropSet(Dictionary<string, string> dto);

    Task<string> PropGet(Dictionary<string, string> dto);

    Task<string> ActionCall(Dictionary<string, string> dto);

    Task SetCookie(Dictionary<string, string> cookies);
}


public class XiaoMiControlDevicesService : IXiaoMiControlDevicesService
{
    private readonly IHttpClientFactory httpClientFactory;

    private Dictionary<string, string> cookies;
    public XiaoMiControlDevicesService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }
    public async Task<string> GetDeviceList(Dictionary<string, string> dto)
    {
        var url = "home/device_list";
        var result = await postData(dto, url);
        return result;
    }

    public async Task<string> PropSet(Dictionary<string, string> dto)
    {
        var url = "miotspec/prop/set";
        var result = await postData(dto, url);
        return result;
    }

    public async Task<string> PropGet(Dictionary<string, string> dto)
    {
        var url = "miotspec/prop/get";
        var result = await postData(dto, url);
        return result;
    }

    public async Task<string> ActionCall(Dictionary<string, string> dto)
    {
        var url = "miotspec/action";
        var result = await postData(dto, url);
        return result;
    }

    private async Task<string> postData(Dictionary<string, string> dto, string url)
    {
        using var httpClient = GetClient(url);
        var httpContent = new FormUrlEncodedContent(dto);
        //var c= httpContent.ToJson();
        var httpResponse = await httpClient.PostAsync(url, httpContent);
        var temp = await httpResponse.Content.ReadAsStringAsync();
        return temp;
    }

    private HttpClient GetClient(string url)
    {
        HttpClient httpClient;
        if (this.cookies != null)
        {
            var cookieContainer = new CookieContainer();

            foreach (var cookie in cookies)
            {
                cookieContainer.Add(new Uri("https://api.io.mi.com/app/" + url), new Cookie(cookie.Key, cookie.Value));
            }

            var httpClientHandler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };
            httpClient = new HttpClient(httpClientHandler);
        }
        else
        {
            httpClient = new HttpClient();
        }

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "APP/com.xiaomi.mihome APPV/6.0.103 iosPassportSDK/3.9.0 iOS/14.4 miHSTS");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-XIAOMI-PROTOCAL-FLAG-CLI", "PROTOCAL-HTTP2");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "identity");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("MIOT-ENCRYPT-ALGORITHM", "ENCRYPT-RC4");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Transfer-Encoding", "chunked");

        httpClient.BaseAddress = new Uri("https://api.io.mi.com/app/");
        return httpClient;
    }

    public async Task SetCookie(Dictionary<string, string> cookies)
    {
        if (this.cookies != null)
        {
            this.cookies.Clear();
        }
        this.cookies = cookies;
    }
}
