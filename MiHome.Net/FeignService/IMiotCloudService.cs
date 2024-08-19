using MiHome.Net.Dto;
using Newtonsoft.Json;

namespace MiHome.Net.FeignService;

public interface IMiotCloudService
{
    Task<GetAllInstanceResult> GetAllInstancesAsync();

    /// <summary>
    /// 通过设备类型获取设备规格，包括服务，属性，方法，事件等
    /// </summary>
    /// <param name="deviceType"></param>
    /// <returns></returns>
    Task<MiotSpec> GetSpecByDeviceType(string deviceType);
}

public class MiotCloudService : IMiotCloudService
{
    private readonly IHttpClientFactory httpClientFactory;

    public MiotCloudService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<GetAllInstanceResult> GetAllInstancesAsync()
    {
        var httpClient = getClient();
        var httpResponse=await httpClient.GetAsync("instances?status=released");
        var temp=await httpResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetAllInstanceResult>(temp);
        return result;
    }

    public async Task<MiotSpec> GetSpecByDeviceType(string deviceType)
    {
        var httpClient = getClient();
        var httpResponse = await httpClient.GetAsync($"instance?type={deviceType}");
        var temp = await httpResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<MiotSpec>(temp);
        return result;
    }

    private HttpClient getClient()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress =new Uri( "https://miot-spec.org/miot-spec-v2/");
        return httpClient;
    }
}