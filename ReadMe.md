# 米家中的一些基本概念
米家中每一个智能家居设备称为一个设备(Device)，每个设备拥有多个服务（Services），每个服务又有多个属性（Property）以及（0-N）个方法（Action,大家把方法理解为封装好的函数即可，我们可以直接调用方法来完成一些操作），所有设备都有设备信息服务，这个服务里包含了多个基本属性，包括设备型号，设备制造商，设备序列号等，并且设备根据功能还有另外一些独有的服务，我们以一个开关为例，开关本身就是一个设备，他拥有一个服务叫Switch，这个服务下面，有一个属性叫Switch Status，也就是开关，我们给这个属性赋值true，就代表开，赋值false，就代表关，同时米家设备支持2种操控方式，基于云端和基于本地，这两者的主要区别就在于云端需要设备以及手机有互联网连接，用户的操作指令是先发送到小米服务器，然后小米服务器下达指令给到智能设备，而基于本地则不同，他是直接通过ip和token，将指令发送给智能设备，这个过程，只需要局域网即可。所有米家智能家居设备都支持云端操作，但不是所有设备都支持本地操作，主要看设备情况。
# MiHome.Net存在的意义
使用本依赖包，用户可以通过云端或者本地的方式用c#原生api来操作米家智能家居设备。

# 分支
本项目共2个分支，main分支和Aot分支，aot分支主要为aot编译准备，比如unity

# Getting Started
## Nuget
 接下来我将演示如何使用【MiHome.Net】,你可以运行以下命令在你的项目中安装 MiHome.Net 。
 
 ```PM> Install-Package MiHome.Net ```

如果是net framework框架，请使用AOT版本，你可以运行以下命令在你的项目中安装 MiHome.Net 。

 ```PM> Install-Package MiHome.Net.Aot ```

# 支持框架
net6.0;net8.0;netstandard2.0
# api使用讲解
本库基于依赖注入，首先新建一个控制台应用，引入MiHome.Net的nuget包，接着添加小米米家的驱动服务，需要配置米家账号和密码，代码如下:
````csharp
 var hostBuilder = Host.CreateDefaultBuilder();
 //添加小米米家的驱动服务，需要小米账号和密码
 hostBuilder.ConfigureServices(it => it.AddMiHomeDriver(x =>
 {
     x.UserName = "<这里填写米家账号>";
     x.Password = "<这里填写米家密码>";
 }));
 var host = hostBuilder.Build();
 var miHomeDriver = host.Services.GetService<IMiHomeDriver>();
````

获取到米家驱动服务以后，我们首先通过云端的方式调用接口，列出家庭里所有的智能家居设备，接着通过米家app里自己设置的设备名称找出自己想要操作的智能家居设备，我这里演示的是一个【米家智能插座2】，![米家智能插座2](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426000632027-1120180539.png)
我将3D打印出来的月球灯连到了这个插座上，并在米家app里将这个插座命名为月球灯，这样就将月球灯接入了智能家居。继续讲解api，接下来通过设备型号获取设备规格，这一步的目的，主要是了解我们要操作的智能家居设备都有哪些服务，哪些方法，哪些属性，并获得它们的id,因为我们操作智能家居需要用到设备id（即did），服务id(即siid)，属性id(即piid)，方法id（即aiid），代码如下：

````csharp
  //列出家庭里所有的智能家居设备
  var deviceList = await miHomeDriver.Cloud.GetDeviceListAsync();
  //通过米家app里自己设置的智能家居名称找出自己想要操作的智能家居设备
  var moonLight = deviceList.FirstOrDefault(it => it.Name == "月球灯");
  //通过设备型号获取设备规格
  var result = await miHomeDriver.Cloud.GetDeviceSpec(moonLight.Model);
````
我这里将获取到的规格结果截图出来给大家讲解一下
![设备规格讲解](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426001530185-2122158528.png)
从上图中可以看到，这个设备总共有8个服务，逐一点开后我发现我只需要了解switch服务（即开关服务）即可，它的iid为2（即siid为2），同时这个服务下有3个属性，逐一点开查看后我发现，我只需要了解Switch Status属性即可，他的iid为1（即piid为1），同时这个属性值的格式（Format）是bool类型,即这个属性值只能为true或者false，接下来我将演示如何获取开关状态，代码如下：
````csharp
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
````
如上代码所示，支持本地以及云端的方式获取属性值，本地获取的方式需要传入智能家居设备的ip，智能家居设备的token，这两者都是之前通过云端设备接口GetDeviceListAsync返回的，以及参数siid和piid，这两者是我们之前通过查看设备规格获得的，通过云端的方式获取属性值，则额外需要did（即设备id），它同样是通过云端设备接口GetDeviceListAsync返回的，本地或云端调用后我们就获取到了开关当前的状态，调用结果如下图，value值为false，即代表开关当前处于关闭状态
![本地调用获取属性值](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426003536606-915110906.png)
![云端调用获取属性值](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426004045730-338349616.png)


接下来我将演示如何设置开关状态，代码如下：
````csharp
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
     Value = true
 });
````
如上代码所示，支持本地以及云端的方式设置属性值，参数和获取属性值差不多，只是多了一个value参数，代表我们要设置的值，这里根据设备规格中format为bool，我们将它设置为true，即代表开。同时这些操作也支持批量获取和批量设置，代码如下
````csharp
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
````

接下来我给大家演示如何通过云端或者本地的方式调用设备服务里的方法，因为米家智能插座2没啥方法可以调用，所以我将使用【Gosund智能排插CP5 Pro】和【小爱音箱Play增强版】来给大家演示调用，先来【Gosund智能排插CP5 Pro】，这个排插长这样
![Gosund智能排插CP5 Pro](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426011926130-1028007718.png)
他上面的4个插座都支持独立控制，我个人非常喜欢，强烈推荐(手动狗头，厂家打钱！)，接下来我将演示调用4个插座其中插座3的方法来控制插座3的开关，
和上面一样，我们首先查规格，代码如下
````csharp
var cp5pro = deviceList.FirstOrDefault(it => it.Name == "Gosund智能排插CP5 Pro");
var result3 = await miHomeDriver.Cloud.GetDeviceSpec(cp5pro.Model);
````
获得的规格如下图
![Gosund智能排插CP5 Pro规格](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426011306697-1831215499.png)
如上图可以看出，插座3有一个方法（即action）叫toggle，这个方法主要就是改变插座3当前的状态，如果原来是关，调用即为开，如果原来是开，调用即为关，同时iid为1（即aiid为1），服务id为5（即siid为5），入参in为空数组，即不需要传入参数，out也为空数组，表示调用没有返回，不多说了，上代码：
````csharp
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
    In = new List<string>() {  }
});
````
如上代码所示，和设置/获取属性不同，调用方法是通过服务id（即siid）和方法id（即aiid）以及入参in来实现的。
接下来演示调用【小爱音箱Play增强版】的服务方法来播放自定义文本，先上图
![小爱音箱Play增强版](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426014228801-703482241.png)
和上面一样，我们首先查规格，代码如下
````csharp
var xiaoAi = deviceList.FirstOrDefault(it => it.Name == "小爱音箱Play增强版");
var result2 = await miHomeDriver.Cloud.GetDeviceSpec(xiaoAi.Model);
````
获得的规格如下图
![小爱音箱Play增强版规格](https://img2024.cnblogs.com/blog/1323385/202404/1323385-20240426014401295-1754444217.png)

如上图可以看出，小爱音箱有一个方法（即action）叫Play Text，这个方法用来播放自定义语音，同时iid为3（即aiid为3），服务id为5（即siid为5），入参in为数组，需要传入一个参数，即我们的自定义文本，out为空数组，表示调用没有返回，同时这个方法只支持云端调用，不支持本地调用，为啥我知道？因为我试过了，本地方法没反应，这个应该是需要服务器把文本转为语音，再回传到小爱音箱来播放，不多说了，上代码：
````csharp
 //使用小爱音箱Play增强版播放我们的自定义文字
 var r9 = await miHomeDriver.Cloud.CallActionAsync(new CallActionInputDto()
 {
     Did = xiaoAi.Did,
     Aiid = 3,
     Siid = 5,
     In = new List<string>() { "门前大桥下，游过一群鸭" }
 });
````

# 开源地址，欢迎star
本项目基于MIT协议开源，地址为
[https://github.com/TripleView/MiHome.Net](https://github.com/TripleView/MiHome.Net)

同时感谢以下项目

1. [python-miio](https://github.com/rytilahti/python-miio)

2. [hass-xiaomi-miot](https://github.com/al-one/hass-xiaomi-miot)

# 写在最后
如果各位靓仔觉得这个项目不错，欢迎一键三连（推荐，star，关注），有了【MiHome.Net】和【Homekit.Net】，想必各位靓仔应该能自己写程序将米家智能家居设备桥接到HomeKit生态里去了，反正我自己已经用了很久了，然后希望大家不要过于频繁的进行云端调用，以免对米家服务器造成不良影响。最后不要问为什么我的智能家居看起来都脏脏的，因为我比较懒，懒得擦，哈哈哈哈哈。
