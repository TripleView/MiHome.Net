using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiHome.Net.Cache;
using MiHome.Net.Dto;
using MiHome.Net.FeignService;
using MiHome.Net.Miio;
using MiHome.Net.Service;

namespace MiHome.Net.Middleware;

public static class MiHomeDriverExtension
{
    public static IServiceCollection AddMiHomeDriver(this IServiceCollection services,Action<MiHomeAccountOption> option)
    {
        var accountOption = new MiHomeAccountOption();
        option(accountOption);
        services.AddHttpClient();
        services.AddScoped<IMiotCloudService, MiotCloudService>();
        services.AddScoped<IXiaoMiLoginService, XiaoMiLoginService>();
        services.AddScoped<IXiaoMiControlDevicesService, XiaoMiControlDevicesService>();
        services.AddMemoryCache();
        services.TryAddScoped<ICacheDeserializer, JsonCacheDeserializer>();
        services.TryAddScoped<ICacheSerializer, JsonCacheSerializer>();
        services.TryAddScoped<ICache, MemoryCache>();
        services.AddScoped<IMiotLocal, MiotLocal>();
        services.AddScoped<IMiotCloud, MIotCloud>();
        services.AddSingleton<MiHomeAccountOption>(it=>accountOption);
        services.AddSingleton<MiioProtocol>();
        services.AddScoped<IMiHomeDriver, MiHomeDriver>();
        return services;
    }
}