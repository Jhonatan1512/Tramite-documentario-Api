namespace ProcessingSystem.Api.Middlewares
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
        public DateTime TimesTamp {  get; set; } = DateTime.UtcNow;

        public ApiErrorResponse(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
