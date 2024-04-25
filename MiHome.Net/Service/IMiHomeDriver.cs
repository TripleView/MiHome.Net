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
/// 小米智能家居sdk
/// </summary>
public interface IMiHomeDriver
{
    IMiotCloud Cloud {  get; }

    IMiotLocal Local { get; }

}

[AutoRegister(typeof(IMiHomeDriver))]
public class MiHomeDriver : IMiHomeDriver
{

    public  IMiotCloud Cloud { get; }

    public  IMiotLocal Local { get; }

    public MiHomeDriver(IMiotCloud cloud, IMiotLocal local)
    {
        Cloud = cloud;
        Local = local;
    }
}