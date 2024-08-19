using MiHome.Net.Miio;
using Newtonsoft.Json.Linq;
using System.Net;

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


public class MiotLocal : IMiotLocal
{
 
    public async Task<CallActionResult> CallActionAsync(string ipAddress, string token,CallActionPayload callActionPayload)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.CallActionAsync(callActionPayload);
    }

    public async Task<GetDeviceInfoResult> GetDeviceInfoAsync(string ipAddress, string token)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.GetDeviceInfoAsync();
    }

    public async Task<GetPropertiesResult> GetPropertiesAsync(string ipAddress, string token, List<GetPropertyPayload> propertiesPayloads)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.GetPropertiesAsync(propertiesPayloads);
    }

    public async Task<GetPropertiesResult> GetPropertyAsync(string ipAddress, string token, GetPropertyPayload propertyPayload)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.GetPropertiesAsync(new List<GetPropertyPayload>(){ propertyPayload });
    }

    public async Task<SetPropertiesResult> SetPropertiesAsync(string ipAddress, string token, List<SetPropertyPayload> propertiesPayloads)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.SetPropertiesAsync(propertiesPayloads);
    }

    public async Task<SetPropertiesResult> SetPropertyAsync(string ipAddress, string token, SetPropertyPayload propertyPayload)
    {
        var miioProtocol = new MiioProtocol(ipAddress, token);
        return await miioProtocol.SetPropertiesAsync(new List<SetPropertyPayload>(){propertyPayload});
    }
}