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
                // Pass the request to the next middleware
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log the exception using Serilog (which you configured earlier)
                _logger.LogError(ex, "Unhandled exception occurred");

                // Return a generic error message to the user (you can customize this)
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
            }
        }
    }
}
