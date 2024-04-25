namespace MiHome.Net.Miio;

public class SetPropertyPayload
{
    //public string Did { get; set; }
    public string Did => $"set-{Siid}-{Piid}";
    public int Siid { get; set; }
    public int Piid { get; set; }

    public object Value { get; set; }
}