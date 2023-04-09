using Microsoft.AspNetCore.Mvc;
using Translation.Classes;
using Translation.Interfaces;

namespace Translation.Extensions;

public static class ResponseExtension
{
    public static IApiResponse<T> ApiResponse<T>(
        this ControllerBase controller,
        int statusCode,
        string message,
        T? data = default
    )
    {
        controller.Response.StatusCode = statusCode;

        return new ApiResponse<T>()
        {
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }
}