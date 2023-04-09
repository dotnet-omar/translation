using Translation.Interfaces;

namespace Translation.Classes;

public class ApiResponse<T> : IApiResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}