namespace ProductsInventory.API.Core.Exceptions
{
    public class InvalidProductException : BusinessException
    {
        public InvalidProductException(string message = "Invalid product") : base(message)
        {
        }

        public InvalidProductException(IDictionary<string, string[]> validationErrors, string message = "Invalid product") : base(message)
        {
            ValidationErrors = validationErrors;
        }
    }
}
