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
        var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        serverSocket.ReceiveTimeout = 5000;
        //serverSocket.EnableBroadcast = true;
        //serverSocket.Connect(IPAddress.Parse("192.168.50.141"),54321 );
        var endPoint = new IPEndPoint(this.ipAddress, 54321);
        serverSocket.SendTo(sendBytes, endPoint);

        while (true)
        {
            byte[] buffer = new byte[4096];
            var len = serverSocket.Receive(buffer);
            if (len > 0)
            {
                this.requestCommand = Command.Parse(buffer,this.token);
                return;
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
        var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //serverSocket.EnableBroadcast = true;
        //serverSocket.Connect(IPAddress.Parse("192.168.50.141"),54321 );
        var endPoint = new IPEndPoint(this.ipAddress, 54321);
        var helloBytes = "21310020ffffffffffffffffffffffffffffffffffffffffffffffffffffffff".HexToBytes();
        serverSocket.SendTo(helloBytes, endPoint);

        while (true)
        {
            byte[] buffer = new byte[1024];
            var len = serverSocket.Receive(buffer);
            if (len > 0)
            {
                this.requestCommand = Command.Parse(buffer,this.token);
                return;
            }
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