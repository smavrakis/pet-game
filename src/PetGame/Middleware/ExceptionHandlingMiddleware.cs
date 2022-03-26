using PetGame.Exceptions;
using System.Net;
using System.Text.Json;

namespace PetGame.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _requestDelegate;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate requestDelegate)
        {
            _logger = logger;
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception e)
            {
                if (e is not ResourceNotFoundException && e is not TaskCanceledException)
                {
                    _logger.LogError(e, "Error when handling a request");
                }

                await HandleExceptionAsync(context, e);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            var code = HttpStatusCode.InternalServerError;
            var errorMessage = "Something went wrong, please try again later.";

            if (e is TaskCanceledException)
            {
                code = HttpStatusCode.BadRequest;
                errorMessage = "Request cancelled by user.";
            }
            else if (e is ResourceNotFoundException)
            {
                code = HttpStatusCode.NotFound;
                errorMessage = e.Message;
            }

            var result = JsonSerializer.Serialize(new { Error = errorMessage });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            await context.Response.WriteAsync(result);
        }
    }
}
