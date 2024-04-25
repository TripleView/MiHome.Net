namespace MiHome.Net.Dto;

public class SetPropPostData
{
    public string AccessKey { get; set; }
    public List<SetPropertyDto> Params { get; set; }
}