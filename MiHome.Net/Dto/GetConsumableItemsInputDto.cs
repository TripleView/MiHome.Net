namespace MiHome.Net.Dto;

public class GetConsumableItemsInputDto
{
    public Int64 home_id { get; set; }

    public Int64 owner_id { get; set; }
    /// <summary>
    /// 设备id列表
    /// </summary>
    //public List<string> dids { get; set; }

    //public string accessKey { get; set; }

    //public bool filter_ignore { get; set; }
}