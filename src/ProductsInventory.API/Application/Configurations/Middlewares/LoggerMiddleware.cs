namespace ProductsInventory.API.Application.Configurations.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;

        public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await HandleLogsAsync(context);

            await _next(context);
        }

        private Task HandleLogsAsync(HttpContext context)
        {
            if (context.Request.Path.Equals("/health"))
                return Task.CompletedTask;

            if (context.User is null || !context.User.Identity.IsAuthenticated)
            {
                _logger.LogInformation($"Request to route {context.Request.Path} at: {DateTime.Now}.");

                return Task.CompletedTask;
            }

            var userId = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            _logger.LogInformation($"User {userId} requested {context.Request.Path} at: {DateTime.Now}.");

            return Task.CompletedTask;
        }
    }
}
