namespace MiHome.Net.Miio;

public class CommandPayload
{
    public int Id { get; set; }
    public string Method { get; set; }
    public object Params { get; set; }
}