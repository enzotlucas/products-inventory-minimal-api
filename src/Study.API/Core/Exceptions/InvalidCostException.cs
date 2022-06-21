namespace Study.API.Core.Exceptions
{
    public class InvalidCostException : BusinessException
    {
        public InvalidCostException(string message = "Invalid cost") 
            : base(message)
        {
        }
    }
}
