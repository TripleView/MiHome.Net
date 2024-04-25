using MiHome.Net.Dto;
using SummerBoot.Feign.Attributes;

namespace MiHome.Net.FeignService;

[FeignClient(Url = "https://miot-spec.org/miot-spec-v2")]
public interface IMiotCloudService
{
    [GetMapping("instances?status=released")]
    Task<GetAllInstanceResult> GetAllInstancesAsync();

    /// <summary>
    /// 通过设备类型获取设备规格，包括服务，属性，方法，事件等
    /// </summary>
    /// <param name="deviceType"></param>
    /// <returns></returns>
    [GetMapping("instance?type={{deviceType}}")]
    Task<MiotSpec> GetSpecByDeviceType(string deviceType);
}