using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using SummerBoot.Core;

namespace MiHome.Net.Miio;

public class MiioProtocol
{
    public MiioProtocol(string ip,string token)
    {
        this.ipAddress = IPAddress.Parse(ip);
        this.token = token;
        this.isDiscovered = false;
        this.tokenBytes = token.HexToBytes();
        this.id = 0;
    }
    /// <summary>
    /// 是否有发现
    /// </summary>
    private bool isDiscovered;

    private IPAddress ipAddress;
    /// <summary>
    /// 设备的token值
    /// </summary>
    private string token;

    private Command requestCommand;

    private int id;

    /// <summary>
    /// UDP 接收超时（毫秒）。
    /// 不设置时 Receive 会无限阻塞：设备离线、丢包或响应慢时，
    /// 调用线程会一直卡在 socket 上且无法从外部中断。
    /// </summary>
    private const int ReceiveTimeoutMilliseconds = 5000;
    /// <summary>
    /// token值的16进制
    /// </summary>
    private byte[] tokenBytes;

    /// <summary>
    /// 批量获取属性
    /// </summary>
    /// <param name="propertiesPayloads"></param>
    /// <returns></returns>
    public async Task<GetPropertiesResult> GetPropertiesAsync(List<GetPropertyPayload> propertiesPayloads)
    {
        await SendAsync("get_properties", propertiesPayloads);
        var result= JsonConvert.DeserializeObject<GetPropertiesResult>(this.requestCommand.Data);
        return result;
    }

    /// <summary>
    /// 批量设置属性
    /// </summary>
    /// <param name="propertiesPayloads"></param>
    /// <returns></returns>
    public async Task<SetPropertiesResult> SetPropertiesAsync(List<SetPropertyPayload> propertiesPayloads)
    {
        await SendAsync("set_properties", propertiesPayloads);
        var result = JsonConvert.DeserializeObject<SetPropertiesResult>(this.requestCommand.Data);
        return result;
    }

    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="callActionPayload"></param>
    /// <returns></returns>
    public async Task<CallActionResult> CallActionAsync(CallActionPayload callActionPayload)
    {
        await SendAsync("action", callActionPayload);
        var result = JsonConvert.DeserializeObject<CallActionResult>(this.requestCommand.Data);
        return result;
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <returns></returns>
    public async Task<GetDeviceInfoResult> GetDeviceInfoAsync()
    {
        await SendAsync("miIO.info");
        var result = JsonConvert.DeserializeObject<GetDeviceInfoResult>(this.requestCommand.Data);
        return result;
        
    }

    private async Task SendAsync(string command, object parameter =null)
    {
        if (!this.isDiscovered)
        {
            await this.SendHandshake();
            this.isDiscovered = true;
        }
        var body = CreateBody(command, parameter);
        var sendCommand = new Command()
        {
            DeviceIdBytes = requestCommand.DeviceIdBytes,
            UnknownBytes = requestCommand.UnknownBytes,
            Ts = requestCommand.Ts.AddSeconds(1),
            TokenBytes = this.tokenBytes
        };
        sendCommand.SetData(body);
        
        var sendBytes= sendCommand.Build();
        using var serverSocket = CreateSocket();
        var endPoint = new IPEndPoint(this.ipAddress, 54321);
        try
        {
            serverSocket.SendTo(sendBytes, endPoint);
            this.requestCommand = ReceiveCommand(serverSocket, 4096);
        }
        catch
        {
            // 一次收发失败就把"已握手"状态作废：下次调用重新握手。
            // 否则设备重启、换了 IP、或者路由器重启之后，这里会一直按旧会话发包，
            // 每次都稳定失败到超时，永远等不到自愈。
            this.isDiscovered = false;
            throw;
        }
    }

    /// <summary>
    /// 创建用于本地通信的 UDP socket：开启广播并统一设置接收超时，
    /// 配合 using 保证每次调用结束后 socket 被释放，不会长期占用句柄。
    /// </summary>
    private Socket CreateSocket()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        socket.ReceiveTimeout = ReceiveTimeoutMilliseconds;
        return socket;
    }

    /// <summary>
    /// 接收一条来自目标设备的报文。
    /// 只接受 this.ipAddress 发来的包：局域网里其它设备的广播、心跳会被直接丢弃，
    /// 既不会被误当成回复，也不会让等待时间被无关报文无限"续命"；
    /// 解析失败（不完整、格式不符）的报文一律跳过，继续等真正的回复。
    /// </summary>
    private Command ReceiveCommand(Socket socket, int bufferSize)
    {
        var stopwatch = Stopwatch.StartNew();
        EndPoint from = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            var remain = ReceiveTimeoutMilliseconds - (int)stopwatch.ElapsedMilliseconds;
            if (remain <= 0)
            {
                // 总等待时间到点：即使一直在收无关报文，也不会无限延长
                throw new SocketException((int)SocketError.TimedOut);
            }
            socket.ReceiveTimeout = remain;

            var buffer = new byte[bufferSize];
            var len = socket.ReceiveFrom(buffer, ref from);

            var remote = from as IPEndPoint;
            if (remote == null || !remote.Address.Equals(this.ipAddress))
            {
                continue;
            }
            if (len <= 0)
            {
                continue;
            }

            try
            {
                return Command.Parse(buffer, this.token);
            }
            catch
            {
                continue;
            }
        }
    }

    private CommandPayload CreateBody(string command, object parameter = null)
    {
        this.id++;
        if (this.id >= 9999)
        {
            this.id = 1;
        }
        var methodCallDto = new CommandPayload()
        {
            Id = this.id,
            Method = command,
            Params = parameter
        };
      
        return methodCallDto;
    }

    //private byte[] EncryptBody(byte[] body)
    //{
    //    var key = tokenBytes.BytesToMd5Bytes();
    //    var ivList = new List<byte>();
    //    for (int i = 0; i < key.Length; i++)
    //    {
    //        ivList.Add(key[i]);
    //    }
    //    ivList.AddRange(tokenBytes);
    //    var iv = ivList.ToArray().BytesToMd5Bytes();
    //    var aes = new RijndaelManaged();
    //    aes.BlockSize = 128;
    //    aes.KeySize = 256;
    //    aes.Mode = CipherMode.CBC;
    //    aes.Padding = PaddingMode.PKCS7;
    //    aes.Key = key;
    //    aes.IV = iv;
    //    var crypt = aes.CreateEncryptor();

    //    var cipherText = crypt.TransformFinalBlock(body.ToArray(), 0, body.ToArray().Length);
    //    return cipherText;
    //}
    /// <summary>
    /// 发送握手包
    /// </summary>
    /// <returns></returns>
    private async Task SendHandshake()
    {
        // 握手同样必须有接收超时：设备不应答时立即失败返回，
        // 否则这里会永久阻塞，调用方只能另起任务，留下无法回收的悬挂线程。
        using var serverSocket = CreateSocket();
        var endPoint = new IPEndPoint(this.ipAddress, 54321);
        var helloBytes = "21310020ffffffffffffffffffffffffffffffffffffffffffffffffffffffff".HexToBytes();
        try
        {
            serverSocket.SendTo(helloBytes, endPoint);
            this.requestCommand = ReceiveCommand(serverSocket, 1024);
        }
        catch
        {
            // 握手失败同样作废状态，保证下一次调用会重新握手而不是沿用半成品
            this.isDiscovered = false;
            throw;
        }
    }

    //public async Task<string> GetUserDeviceData(GetUserDeviceDataInputDto dto)
    //{
    //    await BeginControlDeviceCookieAsync();
    //    var loginInfoDto = await GetLoginInfoAsync();
    //    dto.Uid= loginInfoDto.UserId;


    //    var param = GetRc4Params("POST", "https://api.io.mi.com/app/user/get_user_device_data", dto,
    //        loginInfoDto.Ssecurity);
    //    var signedNonce = param["signedNonce"];
    //    var resultString = await xiaoMiControlDevicesService.GetUserDeviceData(param);
    //    await StopControlDeviceCookieAsync();
    //    var decryptData = DecryptData(signedNonce, resultString);
    //    var result = JsonConvert.DeserializeObject<SetPropOutputDto>(decryptData);
    //    if (result?.Code == 0)
    //    {
    //        //return result.Result;
    //    }

    //    var errorMsg = $"propGet error,reason:{result?.Message}";
    //    logger?.LogError(errorMsg);
    //    throw new Exception(errorMsg);
    //}
}