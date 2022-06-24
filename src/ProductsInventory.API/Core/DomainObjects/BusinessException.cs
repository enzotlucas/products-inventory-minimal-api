namespace ProductsInventory.API.Core.DomainObjects
{
    public class BusinessException : Exception
    {
        public IDictionary<string, string[]> ValidationErrors;

        public BusinessException()
        {
            ValidationErrors = new Dictionary<string, string[]>();
        }

        public BusinessException(string message) : base(message)
        {
            ValidationErrors = new Dictionary<string, string[]>();
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
            ValidationErrors = new Dictionary<string, string[]>();
        }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ValidationErrors = new Dictionary<string, string[]>();
        }
    }
}
