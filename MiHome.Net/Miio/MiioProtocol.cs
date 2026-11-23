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
    /// 诊断回调，默认 null（不输出任何内容）。
    /// 用来排查"设备明明在线却一直超时"：能区分"丢了多少非目标设备的报文"和"收到但解析失败"。
    /// 回调在调用线程上触发，UI 场景请自行切回主线程；回调内部抛的异常会被吞掉，不影响协议本身。
    /// </summary>
    public static Action<string>? Trace { get; set; }
    /// <summary>
    /// 接收等待的分片长度（毫秒）。
    /// 阻塞的 Receive 没法从外部直接打断，所以等待拆成小片，
    /// 片与片之间检查 CancellationToken，取消最迟一个分片（100ms）内生效。
    /// </summary>
    private const int ReceiveSliceMilliseconds = 100;
    /// <summary>
    /// token值的16进制
    /// </summary>
    private byte[] tokenBytes;

    /// <summary>
    /// 批量获取属性
    /// </summary>
    /// <param name="propertiesPayloads"></param>
    /// <returns></returns>
    public Task<GetPropertiesResult> GetPropertiesAsync(List<GetPropertyPayload> propertiesPayloads)
    {
        return GetPropertiesAsync(propertiesPayloads, CancellationToken.None);
    }

    /// <summary>
    /// 批量获取属性，支持取消
    /// </summary>
    /// <param name="propertiesPayloads"></param>
    /// <param name="cancellationToken">取消后不再等待回复，抛 <see cref="OperationCanceledException"/></param>
    /// <returns></returns>
    public async Task<GetPropertiesResult> GetPropertiesAsync(List<GetPropertyPayload> propertiesPayloads, CancellationToken cancellationToken)
    {
        await SendAsync("get_properties", propertiesPayloads, cancellationToken);
        var result = JsonConvert.DeserializeObject<GetPropertiesResult>(this.requestCommand.Data);
        return result;
    }

    /// <summary>
    /// 批量设置属性
    /// </summary>
    /// <param name="propertiesPayloads"></param>
    /// <returns></returns>
    public Task<SetPropertiesResult> SetPropertiesAsync(List<SetPropertyPayload> propertiesPayloads)
    {
        return SetPropertiesAsync(propertiesPayloads, CancellationToken.None);
    }

    /// <summary>
    /// 批量设置属性，支持取消
    /// </summary>
    /// <param name="propertiesPayloads"></param>
    /// <param name="cancellationToken">取消后不再等待回复，抛 <see cref="OperationCanceledException"/></param>
    /// <returns></returns>
    public async Task<SetPropertiesResult> SetPropertiesAsync(List<SetPropertyPayload> propertiesPayloads, CancellationToken cancellationToken)
    {
        await SendAsync("set_properties", propertiesPayloads, cancellationToken);
        var result = JsonConvert.DeserializeObject<SetPropertiesResult>(this.requestCommand.Data);
        return result;
    }

    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="callActionPayload"></param>
    /// <returns></returns>
    public Task<CallActionResult> CallActionAsync(CallActionPayload callActionPayload)
    {
        return CallActionAsync(callActionPayload, CancellationToken.None);
    }

    /// <summary>
    /// 调用方法，支持取消
    /// </summary>
    /// <param name="callActionPayload"></param>
    /// <param name="cancellationToken">取消后不再等待回复，抛 <see cref="OperationCanceledException"/></param>
    /// <returns></returns>
    public async Task<CallActionResult> CallActionAsync(CallActionPayload callActionPayload, CancellationToken cancellationToken)
    {
        await SendAsync("action", callActionPayload, cancellationToken);
        var result = JsonConvert.DeserializeObject<CallActionResult>(this.requestCommand.Data);
        return result;
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <returns></returns>
    public Task<GetDeviceInfoResult> GetDeviceInfoAsync()
    {
        return GetDeviceInfoAsync(CancellationToken.None);
    }

    /// <summary>
    /// 获取设备信息，支持取消
    /// </summary>
    /// <param name="cancellationToken">取消后不再等待回复，抛 <see cref="OperationCanceledException"/></param>
    /// <returns></returns>
    public async Task<GetDeviceInfoResult> GetDeviceInfoAsync(CancellationToken cancellationToken)
    {
        await SendAsync("miIO.info", null, cancellationToken);
        var result = JsonConvert.DeserializeObject<GetDeviceInfoResult>(this.requestCommand.Data);
        return result;
        
    }

    private async Task SendAsync(string command, object parameter, CancellationToken cancellationToken)
    {
        if (!this.isDiscovered)
        {
            await this.SendHandshake(cancellationToken);
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
            this.requestCommand = ReceiveCommand(serverSocket, 4096, cancellationToken);
        }
        catch (Exception ex)
        {
            // 一次收发失败就把"已握手"状态作废：下次调用重新握手。
            // 否则设备重启、换了 IP、或者路由器重启之后，这里会一直按旧会话发包，
            // 每次都稳定失败到超时，永远等不到自愈。
            WriteTrace($"收发失败，握手状态复位: {ex.Message}");
            this.isDiscovered = false;
            throw;
        }
    }

    /// <summary>
    /// 输出一条诊断信息。回调为空时零开销，回调抛异常时也不会影响协议本身。
    /// </summary>
    private void WriteTrace(string message)
    {
        var callback = Trace;
        if (callback == null)
        {
            return;
        }

        try
        {
            callback($"[miio {this.ipAddress}] {message}");
        }
        catch
        {
            // 诊断输出永远不能把功能搞挂
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
        return ReceiveCommand(socket, bufferSize, CancellationToken.None);
    }

    /// <summary>
    /// 同上，额外支持取消。
    /// 阻塞的 Receive 无法被外部打断，所以按 ReceiveSliceMilliseconds 分片等待，
    /// 每一片之间检查取消信号；未取消时行为和不带参数的版本一致，
    /// 总等待超过 ReceiveTimeoutMilliseconds 仍然抛 SocketException(TimedOut)。
    /// </summary>
    private Command ReceiveCommand(Socket socket, int bufferSize, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        EndPoint from = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var remain = ReceiveTimeoutMilliseconds - (int)stopwatch.ElapsedMilliseconds;
            if (remain <= 0)
            {
                // 总等待时间到点：即使一直在收无关报文，也不会无限延长
                throw new SocketException((int)SocketError.TimedOut);
            }

            // 分片等待，让取消能在一片之内生效
            socket.ReceiveTimeout = Math.Min(remain, ReceiveSliceMilliseconds);

            var buffer = new byte[bufferSize];
            int len;
            try
            {
                len = socket.ReceiveFrom(buffer, ref from);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                // 这一片只是暂时没数据，回到循环顶部重新判断总超时和取消
                continue;
            }

            var remote = from as IPEndPoint;
            if (remote == null || !remote.Address.Equals(this.ipAddress))
            {
                WriteTrace($"丢弃非目标设备报文: {from}");
                continue;
            }
            if (len <= 0)
            {
                WriteTrace("忽略空报文");
                continue;
            }

            // miIO 报文 = 16 字节头 + 16 字节校验，短于 32 字节的必然是垃圾包；
            // 长度字段也必须落在 [32, 实收长度] 之间，否则 Parse 会切出错位的数据段
            // （length = 0 时 Parse 不会抛异常，而是返回一个 Data 为空的 Command）
            if (len < 32)
            {
                WriteTrace($"报文过短，已跳过: {len} 字节");
                continue;
            }
            var declaredLength = (buffer[2] << 8) | buffer[3];
            if (declaredLength < 32 || declaredLength > len)
            {
                WriteTrace($"报文长度字段非法，已跳过: 实收 {len} 字节，声明 {declaredLength} 字节");
                continue;
            }

            try
            {
                return Command.Parse(buffer, this.token);
            }
            catch (Exception ex)
            {
                WriteTrace($"报文解析失败，已跳过: {ex.Message}");
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
    private async Task SendHandshake(CancellationToken cancellationToken)
    {
        // 握手同样必须有接收超时：设备不应答时立即失败返回，
        // 否则这里会永久阻塞，调用方只能另起任务，留下无法回收的悬挂线程。
        using var serverSocket = CreateSocket();
        var endPoint = new IPEndPoint(this.ipAddress, 54321);
        var helloBytes = "21310020ffffffffffffffffffffffffffffffffffffffffffffffffffffffff".HexToBytes();
        try
        {
            serverSocket.SendTo(helloBytes, endPoint);
            this.requestCommand = ReceiveCommand(serverSocket, 1024, cancellationToken);
        }
        catch (Exception ex)
        {
            // 握手失败同样作废状态，保证下一次调用会重新握手而不是沿用半成品
            WriteTrace($"握手失败，握手状态复位: {ex.Message}");
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