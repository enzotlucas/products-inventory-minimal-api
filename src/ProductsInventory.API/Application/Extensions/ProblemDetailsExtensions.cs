namespace ProductsInventory.API.Application.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error)
        {
            return new Dictionary<string, string[]>
            {
                { "Error", error.Select(e => e.Description).ToArray() }
            };
        }
    }
}
