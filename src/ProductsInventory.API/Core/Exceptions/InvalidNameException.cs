namespace ProductsInventory.API.Core.Exceptions
{
    public class InvalidNameException : BusinessException
    {
        public InvalidNameException(string message = "Invalid name")
            : base(message)
        {
        }
    }
}
