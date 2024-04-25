namespace MiHome.Net.Miio;

public class GetPropertyPayload
{
    //public string Did { get; set; }
    public string Did => $"{Siid}-{Piid}";
    public int Siid { get; set; }
    public int Piid { get; set; }
}