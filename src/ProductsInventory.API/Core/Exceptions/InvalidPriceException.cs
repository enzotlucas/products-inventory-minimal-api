namespace ProductsInventory.API.Core.Exceptions
{
    public class InvalidPriceException : BusinessException
    {
        public InvalidPriceException(string message = "Invalid price") 
            : base(message)
        {
        }
    }
}
