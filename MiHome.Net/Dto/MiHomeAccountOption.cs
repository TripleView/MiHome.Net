﻿namespace MiHome.Net.Dto;

public class MiHomeAccountOption
{
    /// <summary>
    /// xiaomi account 小米账号
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// xiaomi account password 小米账号的密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// QR code save path
    /// 二维码保存路径
    /// </summary>
    public string QrCodeSavePath { get; set; }
}