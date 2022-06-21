namespace Study.API.Core.Exceptions
{
    public class InvalidQuantityException : BusinessException
    {
        public InvalidQuantityException(string message = "Invalid quantity") 
            : base(message)
        {
        }
    }
}
