using Newtonsoft.Json;

namespace Translation.Interfaces;

public interface IPagination<T>
{
    [JsonProperty(PropertyName = "items")] public ICollection<T> Items { set; get; }

    [JsonProperty(PropertyName = "pageNumber")]
    public int PageNumber { set; get; }

    [JsonProperty(PropertyName = "pageSize")]
    public int PageSize { set; get; }

    [JsonProperty(PropertyName = "total")] public int Total { set; get; }
}