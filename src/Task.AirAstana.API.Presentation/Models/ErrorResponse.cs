namespace Task.AirAstana.API.Presentation.Models;

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}