using Microsoft.Extensions.DependencyInjection;
using MiHome.Net.Dto;
using MiHome.Net.Miio;
using MiHome.Net.Service;
using SummerBoot.Core;

namespace MiHome.Net.Middleware;

public static class MiHomeDriverExtension
{
    public static IServiceCollection AddMiHomeDriver(this IServiceCollection services,Action<MiHomeAccountOption> option)
    {
        var accountOption = new MiHomeAccountOption();
        option(accountOption);
        services.AddSummerBoot();
        services.AddSummerBootFeign();
        services.AddSummerBootCache(it =>
        {
            it.UseMemory();
        });
        services.AddSingleton<MiHomeAccountOption>(it=>accountOption);
        services.AddScoped<IMiHomeDriver, MiHomeDriver>();
        return services;
    }
}