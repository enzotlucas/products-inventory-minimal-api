namespace Study.API.Core.Exceptions
{
    public class InvalidProductException : BusinessException
    {
        public InvalidProductException(string message = "Invalid product") : base(message)
        {
        }
    }
}
