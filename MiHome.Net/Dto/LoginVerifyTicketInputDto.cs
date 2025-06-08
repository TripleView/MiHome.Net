namespace MiHome.Net.Dto;

public class LoginVerifyTicketInputDto
{
    public int _flag { get; set; }

    public string ticket { get; set; }

    public bool trust { get; set; }
    public bool _json { get; set; }
}