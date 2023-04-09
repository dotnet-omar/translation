using Newtonsoft.Json;

namespace Translation.Interfaces;

public interface IApiResponse<T>
{
    [JsonProperty(PropertyName = "data")] public T? Data { set; get; }

    [JsonProperty(PropertyName = "message")]
    public string Message { set; get; }

    [JsonProperty(PropertyName = "statusCode")]
    public int StatusCode { set; get; }
}