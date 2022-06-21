using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ProductsInventory.API.Application.Extensions
{
    public static class ResultsExtensions
    {
        public static IResult NotFoundWithLog(this ILogger logger, string responseMessage, string logComplementMessage)
        {
            logger.LogWarning(responseMessage + logComplementMessage);

            return Results.NotFound(responseMessage);
        }

        public static IResult ProblemWithLog(this ILogger logger, string responseMessage, string logComplementMessage)
        {
            logger.LogError(responseMessage + logComplementMessage);

            return Results.Problem(responseMessage);
        }

        public static IResult BadRequestWithLog(this ILogger logger, string responseMessage, string logComplementMessage)
        {
            logger.LogWarning(responseMessage + logComplementMessage);

            return Results.BadRequest(responseMessage);
        }

        public static IResult NoContentWithLog(this ILogger logger, string logMessage)
        {
            logger.LogInformation(logMessage);

            return Results.NoContent();
        }

        public static IResult ValidationProblemsWithLog(this ILogger logger, string logMessage, IDictionary<string, string[]> response)
        {
            logger.LogWarning(logMessage);

            return Results.ValidationProblem(response);
        }

        public static IResult OkWithLog(this ILogger logger, string logMessage, object response = null)
        {
            logger.LogInformation(logMessage);

            return Results.Ok(response);
        }
    }
}
