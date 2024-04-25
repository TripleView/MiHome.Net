using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SummerBoot.Core;

namespace MiHome.Net.Miio;

/// <summary>
/// 命令
/// </summary>
public class Command
{
    public static Command Parse(byte[] bytes, string token)
    {
        var command = new Command();

        var magicNumber = bytes.Take(2).ToArray().ByteArrayToHexStringThenToInt();
        command.MagicNumber = magicNumber;
        var length = bytes.Skip(2).Take(2).ToArray().ByteArrayToHexStringThenToInt();
        command.Length = length;
        var unknown = bytes.Skip(4).Take(4).ToArray().ByteArrayToHexStringThenToInt();
        command.Unknown = unknown;
        var deviceId = bytes.Skip(8).Take(4).ToArray().ByteArrayToHexStringThenToInt();
        command.DeviceId = deviceId;
        var ts = ((double)(bytes.Skip(12).Take(4).ToArray().ByteArrayToHexStringThenToInt())).UnixTimeStampToUtcDateTime();
        command.Ts = ts;

        command.MagicNumberBytes = bytes.Take(2).ToArray();
        command.LengthBytes = bytes.Skip(2).Take(2).ToArray();
        command.UnknownBytes = bytes.Skip(4).Take(4).ToArray();
        command.DeviceIdBytes = bytes.Skip(8).Take(4).ToArray();
        command.TsBytes = bytes.Skip(12).Take(4).ToArray();
        command.CheckSum = bytes.Skip(16).Take(16).ToArray();
        command.Token = token;
        command.TokenBytes = token.HexToBytes(); ;
        if (length > 0)
        {
            var dataBytes= bytes.Skip(32).Take(length - 32).ToArray();
            var decryptDataBytes = command.Decrypt(dataBytes);
            if (decryptDataBytes.Length > 0 && decryptDataBytes.Last() == 0)
            {
                decryptDataBytes = decryptDataBytes.Take(decryptDataBytes.Length - 1).ToArray();
            }

            command.DataBytes = decryptDataBytes;
            if (decryptDataBytes.Length > 0)
            {
                command.Data = decryptDataBytes.GetString();
            }
        }
        return command;
    }

    public byte[] Build()
    {
        var result = new List<byte>();
        result.AddRange(this.MagicNumberBytes);
        result.AddRange(this.LengthBytes);
        result.AddRange(this.UnknownBytes);
        result.AddRange(this.DeviceIdBytes);
        this.TsBytes = ((int)this.Ts.UtcDateTimeToUnixTimeStamp()).ToString("X8").HexToBytes();
        result.AddRange(this.TsBytes);
        result.AddRange(this.TokenBytes);
        result.AddRange(this.DataBytes);
        var checkSum = result.ToArray().BytesToMd5Bytes();
        result = new List<byte>();
        result.AddRange(this.MagicNumberBytes);
        result.AddRange(this.LengthBytes);
        result.AddRange(this.UnknownBytes);
        result.AddRange(this.DeviceIdBytes);
        this.TsBytes = ((int)this.Ts.UtcDateTimeToUnixTimeStamp()).ToString("X8").HexToBytes();
        result.AddRange(this.TsBytes);
        result.AddRange(checkSum);
        result.AddRange(this.DataBytes);
        return result.ToArray();
    }

    private void CheckToken()
    {
        if (TokenBytes == null || TokenBytes.Length == 0)
        {
            throw new Exception("token can not be null");
        }
    }

    private byte[] Encrypt(byte[] body)
    {
        if (TokenBytes == null || TokenBytes.Length == 0)
        {
            return body;
        }
        var key = TokenBytes.BytesToMd5Bytes();
        var ivList = new List<byte>();
        for (int i = 0; i < key.Length; i++)
        {
            ivList.Add(key[i]);
        }
        ivList.AddRange(TokenBytes);
        var iv = ivList.ToArray().BytesToMd5Bytes();
        var aes = new RijndaelManaged();
        aes.BlockSize = 128;
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = iv;
        var crypt = aes.CreateEncryptor();

        var cipherText = crypt.TransformFinalBlock(body.ToArray(), 0, body.ToArray().Length);
        return cipherText;
    }

    private byte[] Decrypt(byte[] body)
    {
        if (TokenBytes == null || TokenBytes.Length == 0)
        {
            return body;
        }
        var key = TokenBytes.BytesToMd5Bytes();
        var ivList = new List<byte>();
        for (int i = 0; i < key.Length; i++)
        {
            ivList.Add(key[i]);
        }
        ivList.AddRange(TokenBytes);
        var iv = ivList.ToArray().BytesToMd5Bytes();
        var aes = new RijndaelManaged();
        aes.BlockSize = 128;
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = iv;
        var crypt = aes.CreateDecryptor();

        var cipherText = crypt.TransformFinalBlock(body.ToArray(), 0, body.ToArray().Length);
        return cipherText;
    }

    public int MagicNumber { get; set; }
    /// <summary>
    /// 魔术数
    /// </summary>
    public byte[] MagicNumberBytes { get; set; } = new byte[2] { 33, 49 };
    /// <summary>
    /// 数据长度
    /// </summary>
    public int Length { get; set; }
    /// <summary>
    /// 数据长度
    /// </summary>
    public byte[] LengthBytes { get; set; }

    public int Unknown { get; set; }
    /// <summary>
    /// 分割
    /// </summary>
    public byte[] UnknownBytes { get; set; } = new byte[4] { 0, 0, 0, 0 };
    /// <summary>
    /// 设备id
    /// </summary>
    public int DeviceId { get; set; }
    /// <summary>
    /// 设备id
    /// </summary>
    public byte[] DeviceIdBytes { get; set; }
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Ts { get; set; }

    public byte[] TsBytes { get; set; }
    /// <summary>
    /// token值
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// token值
    /// </summary>
    public byte[] TokenBytes { get; set; }
    /// <summary>
    /// 数据
    /// </summary>
    public byte[] DataBytes { get; private set; }

    public string Data { get; set; }
    public void SetData(CommandPayload commandPayload)
    {
        var op = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };
        var methodCallDtos = JsonConvert.SerializeObject(commandPayload, op).Replace(":", ": ").Replace(",", ", ");
        var bytes = methodCallDtos.GetBytes().ToList();
        bytes.Add(0);
        var result = Encrypt(bytes.ToArray());
        this.DataBytes = result;
        this.Length = result.Length + 32;
        this.LengthBytes = this.Length.ToString("X4").HexToBytes();
    }
    /// <summary>
    /// 检查和，用来进行校验
    /// </summary>
    public byte[] CheckSum { get; set; }

}
