using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiHome.Net.Dto;
using MiHome.Net.Middleware;
using MiHome.Net.Miio;
using MiHome.Net.Service;

namespace Demo
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            //添加小米米家的驱动服务，需要小米账号和密码
            hostBuilder.ConfigureServices(it => it.AddMiHomeDriver(x =>
            {
                x.UserName = "18050050949";
                x.Password = "lean5426";
            }));
            var host = hostBuilder.Build();

            var miHomeDriver = host.Services.GetService<IMiHomeDriver>();
            //列出家庭里所有的智能家居设备
            var deviceList = await miHomeDriver.Cloud.GetDeviceListAsync();
            //通过米家app里自己设置的智能家居名称找出自己想要操作的智能家居设备
            var moonLight = deviceList.FirstOrDefault(it => it.Name == "月球灯");
            var xiaoAi = deviceList.FirstOrDefault(it => it.Name == "小爱音箱Play增强版");
            var cp5pro = deviceList.FirstOrDefault(it => it.Name == "Gosund智能排插CP5 Pro");
            //通过设备型号获取设备规格
            var result = await miHomeDriver.Cloud.GetDeviceSpec(moonLight.Model);
            var result2 = await miHomeDriver.Cloud.GetDeviceSpec(xiaoAi.Model);
            var result3 = await miHomeDriver.Cloud.GetDeviceSpec(cp5pro.Model);



            //使用云端方式调用Gosund智能排插CP5 Pro中4个开关中第3个开关的toggle方法
            var r11 = await miHomeDriver.Cloud.CallActionAsync(new CallActionInputDto()
            {
                Did = cp5pro.Did,
                Aiid = 1,
                Siid = 5,
                In = new List<string>() { }
            });

            //使用本地方式调用Gosund智能排插CP5 Pro中4个开关中第3个开关的toggle方法
            var r10 = await miHomeDriver.Local.CallActionAsync(cp5pro.LocalIp, cp5pro.Token, new CallActionPayload()
            {
                Siid = 5,
                Aiid = 1,
                In = new List<string>() { }
            });

            //使用小爱音箱Play增强版播放我们的自定义文字
            var r9 = await miHomeDriver.Cloud.CallActionAsync(new CallActionInputDto()
            {
                Did = xiaoAi.Did,
                Aiid = 3,
                Siid = 5,
                In = new List<string>() { "门前大桥下，游过一群鸭" }
            });

            //通过本地方式获取属性值
            var r1 = await miHomeDriver.Local.GetPropertyAsync(moonLight.LocalIp, moonLight.Token, new GetPropertyPayload()
            {
                Siid = 2,
                Piid = 1
            });
            //通过云端方式获取属性值
            var r7 = await miHomeDriver.Cloud.GetPropertyAsync(new GetPropertyDto()
            {
                Did = moonLight.Did,
                Siid = 2,
                Piid = 1
            });

            //通过本地方式设置属性值
            var r2 = await miHomeDriver.Local.SetPropertyAsync(moonLight.LocalIp, moonLight.Token, new SetPropertyPayload()
            {
                Siid = 2,
                Piid = 1,
                Value = true
            });

            //通过云端方式设置属性值
            var r8 = await miHomeDriver.Cloud.SetPropertyAsync(new SetPropertyDto()
            {
                Did = moonLight.Did,
                Siid = 2,
                Piid = 1,
                Value = false
            });

            //通过本地方式批量获取属性值
            var r3 = await miHomeDriver.Local.GetPropertiesAsync(moonLight.LocalIp, moonLight.Token, new List<GetPropertyPayload>(){new GetPropertyPayload()
            {
                Siid = 2,
                Piid = 1
            }});

            //通过云端方式批量获取属性值
            var r5 = await miHomeDriver.Cloud.GetPropertiesAsync(new List<GetPropertyDto>()
            {
                new GetPropertyDto()
                {
                    Did = moonLight.Did,
                    Siid = 2,
                    Piid = 1
                }
            });

            //通过本地方式批量设置属性值
            var r4 = await miHomeDriver.Local.SetPropertiesAsync(moonLight.LocalIp, moonLight.Token, new List<SetPropertyPayload>(){new SetPropertyPayload()
            {
                Siid = 2,
                Piid = 1,
                Value =true
            }});


            //通过云端方式批量设置属性值
            var r6 = await miHomeDriver.Cloud.SetPropertiesAsync(new List<SetPropertyDto>()
          {
              new SetPropertyDto()
              {
                  Did = moonLight.Did,
                  Siid = 2,
                  Piid = 1,
                  Value = true
              }
          });


        }
    }
}
