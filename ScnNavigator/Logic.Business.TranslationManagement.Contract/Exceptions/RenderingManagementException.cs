using System.Runtime.Serialization;

namespace Logic.Business.TranslationManagement.Contract.Exceptions
{
    public class TranslationManagementException : Exception
    {
        public TranslationManagementException()
        {
        }

        public TranslationManagementException(string message) : base(message)
        {
        }

        public TranslationManagementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TranslationManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
