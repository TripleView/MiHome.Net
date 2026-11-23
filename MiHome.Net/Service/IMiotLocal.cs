using MiHome.Net.Miio;
using Newtonsoft.Json.Linq;
using SummerBoot.Core;
using System.Net;
using YamlDotNet.Core.Tokens;

namespace MiHome.Net.Service;

public interface IMiotLocal
{
    /// <summary>
    /// get property；获取属性
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="token"></param>
    /// <param name="propertyPayload"></param>
    /// <returns></returns>
    Task<GetPropertiesResult> GetPropertyAsync(string ipAddress, string token, GetPropertyPayload propertyPayload);
    /// <summary>
    /// set property；设置属性 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="token"></param>
    /// <param name="propertyPayload"></param>
    /// <returns></returns>
    Task<SetPropertiesResult> SetPropertyAsync(string ipAddress, string token, SetPropertyPayload propertyPayload);

    Task<GetPropertiesResult> GetPropertiesAsync(string ipAddress, string token, List<GetPropertyPayload> propertiesPayloads);

    Task<SetPropertiesResult> SetPropertiesAsync(string ipAddress, string token, List<SetPropertyPayload> propertiesPayloads);

    Task<CallActionResult> CallActionAsync(string ipAddress, string token, CallActionPayload callActionPayload);

    Task<GetDeviceInfoResult> GetDeviceInfoAsync(string ipAddress, string token);
}


[AutoRegister(typeof(IMiotLocal))]
public class MiotLocal : IMiotLocal
{
 
    public Task<CallActionResult> CallActionAsync(string ipAddress, string token, CallActionPayload callActionPayload)
    {
        return CallActionAsync(ipAddress, token, callActionPayload, CancellationToken.None);
    }

    /// <summary>
    /// 调用方法，支持取消
    /// </summary>
    public async Task<CallActionResult> CallActionAsync(string ipAddress, string token, CallActionPayload callActionPayload, CancellationToken cancellationToken)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.CallActionAsync(callActionPayload, cancellationToken);
    }

    public Task<GetDeviceInfoResult> GetDeviceInfoAsync(string ipAddress, string token)
    {
        return GetDeviceInfoAsync(ipAddress, token, CancellationToken.None);
    }

    /// <summary>
    /// 获取设备信息，支持取消
    /// </summary>
    public async Task<GetDeviceInfoResult> GetDeviceInfoAsync(string ipAddress, string token, CancellationToken cancellationToken)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.GetDeviceInfoAsync(cancellationToken);
    }

    public Task<GetPropertiesResult> GetPropertiesAsync(string ipAddress, string token, List<GetPropertyPayload> propertiesPayloads)
    {
        return GetPropertiesAsync(ipAddress, token, propertiesPayloads, CancellationToken.None);
    }

    /// <summary>
    /// 批量获取属性，支持取消
    /// </summary>
    public async Task<GetPropertiesResult> GetPropertiesAsync(string ipAddress, string token, List<GetPropertyPayload> propertiesPayloads, CancellationToken cancellationToken)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.GetPropertiesAsync(propertiesPayloads, cancellationToken);
    }

    public Task<GetPropertiesResult> GetPropertyAsync(string ipAddress, string token, GetPropertyPayload propertyPayload)
    {
        return GetPropertyAsync(ipAddress, token, propertyPayload, CancellationToken.None);
    }

    /// <summary>
    /// 获取单个属性，支持取消
    /// </summary>
    public async Task<GetPropertiesResult> GetPropertyAsync(string ipAddress, string token, GetPropertyPayload propertyPayload, CancellationToken cancellationToken)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.GetPropertiesAsync(new List<GetPropertyPayload>() { propertyPayload }, cancellationToken);
    }

    public Task<SetPropertiesResult> SetPropertiesAsync(string ipAddress, string token, List<SetPropertyPayload> propertiesPayloads)
    {
        return SetPropertiesAsync(ipAddress, token, propertiesPayloads, CancellationToken.None);
    }

    /// <summary>
    /// 批量设置属性，支持取消
    /// </summary>
    public async Task<SetPropertiesResult> SetPropertiesAsync(string ipAddress, string token, List<SetPropertyPayload> propertiesPayloads, CancellationToken cancellationToken)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.SetPropertiesAsync(propertiesPayloads, cancellationToken);
    }

    public Task<SetPropertiesResult> SetPropertyAsync(string ipAddress, string token, SetPropertyPayload propertyPayload)
    {
        return SetPropertyAsync(ipAddress, token, propertyPayload, CancellationToken.None);
    }

    /// <summary>
    /// 设置单个属性，支持取消
    /// </summary>
    public async Task<SetPropertiesResult> SetPropertyAsync(string ipAddress, string token, SetPropertyPayload propertyPayload, CancellationToken cancellationToken)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.SetPropertiesAsync(new List<SetPropertyPayload>() { propertyPayload }, cancellationToken);
    }
}