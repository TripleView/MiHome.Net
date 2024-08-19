using System.Security.Cryptography;
using System.Text;

namespace MiHome.Net.Utils;

public static  class BytesUtils
{
    /// <summary>
    /// 字节数组转为16进制字符串，再转为int类型
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static int ByteArrayToHexStringThenToInt(this byte[] bytes)
    {
        var middle = string.Join(string.Empty, Array.ConvertAll(bytes, b => b.ToString("X2")));
        var result = Convert.ToInt32(middle, 16);
        return result;
    }


    /// <summary>
    /// 十六进制转byte array
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] HexToBytes(this string str)
    {
        str = str.Replace(" ", "");//移除空格
        byte[] comBuffer = new byte[str.Length / 2];
        for (int i = 0; i < str.Length; i += 2)
        {
            comBuffer[i / 2] = (byte)Convert.ToByte(str.Substring(i, 2), 16);
        }

        return comBuffer;
    }

    /// <summary>
    /// byte数组获得字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string GetString(this byte[] bytes)
    {
        if (bytes == null) return string.Empty;
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 字节数组转为md5字节数组
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] BytesToMd5Bytes(this byte[] inputBytes)
    {
        MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(inputBytes);
        return hash;
    }

    public static byte[] GetBytes(this string source, Encoding encoding)
    {
        return encoding.GetBytes(source);
    }

    /// <summary>
    /// 字符串转byte数组
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    /// <summary>
    /// 先使用Sha256再使用base64
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToSha256ThenBase64(this byte[] bytes)
    {
        var sha1 = SHA256.Create();
        //加密
        byte[] newPwd = sha1.ComputeHash(bytes);
        var result = newPwd.ToBase64();
        return result;
    }

    /// <summary>
    /// 转为base64编码
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToBase64(this byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }


    /// <summary>
    /// 先使用Sha1再使用base64
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToSha1ThenBase64(this byte[] bytes)
    {
        var sha1 = SHA1.Create();
        //加密
        byte[] newPwd = sha1.ComputeHash(bytes);
        var result = newPwd.ToBase64();
        return result;
    }
    /// <summary>
    /// 先使用Sha1再使用base64
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToSha1ThenBase64(this string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return bytes.ToSha1ThenBase64();
    }

    /// <summary>
    /// Return an array of bytes representing an integer.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="len">Length of bytes object to use. An OverflowError is raised if the integer is not representable with the given number of bytes.</param>
    /// <returns></returns>
    public static byte[] ToBytesBig(this int number, int len)
    {
        byte[] bytes = BitConverter.GetBytes(number);
        if (BitConverter.IsLittleEndian)                //if little endian, reverse to get big endian
            Array.Reverse(bytes);
        if (bytes.Length == len) return bytes;          //if already desired length, return.
        if (bytes.Length > len)                         //if length is too long, remove some elements
        {
            var bytesTmp = Array.Empty<byte>();
            bytes.CopyTo(bytesTmp, bytes.Length - len);
            bytes = bytesTmp;
        }
        else                                            //if length is too small, add 0's in byte
        {
            Array.Reverse(bytes);
            for (var i = bytes.Length; i < len; i++)
                bytes[i] = (byte)0;
            Array.Reverse(bytes);
        }
        return bytes;
    }
}