namespace MiHome.Net.Dto;

public class CheckIdentityLisResult
{
    public int code { get; set; }
    public int flag { get; set; }
    public int option { get; set; }
    public int[] options { get; set; }
    public string version { get; set; }
    public bool showFastUpdateEmailLink { get; set; }
    public string externalId { get; set; }
    public string retrieveType { get; set; }
    public bool trustCheckBox { get; set; }
    public bool trustCheckBoxSelected { get; set; }
    public bool directVerify { get; set; }
}
