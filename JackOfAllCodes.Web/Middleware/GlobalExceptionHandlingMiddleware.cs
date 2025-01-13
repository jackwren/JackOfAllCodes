namespace JackOfAllCodes.Web.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(httpContext, ex, StatusCodes.Status400BadRequest, "Invalid argument provided.");
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(httpContext, ex, StatusCodes.Status401Unauthorized, "Unauthorized access.");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception, int statusCode, string userMessage)
        {
            // Log the exception
            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            // Set the response details
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            // Create the error response
            var response = new
            {
                error = userMessage,
                details = exception.Message
            };

            // Write the response as JSON
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }

}
