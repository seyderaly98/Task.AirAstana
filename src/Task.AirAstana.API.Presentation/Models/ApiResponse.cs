namespace Task.AirAstana.API.Presentation.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow
        };
    }

    public static ApiResponse<T> FailureResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}