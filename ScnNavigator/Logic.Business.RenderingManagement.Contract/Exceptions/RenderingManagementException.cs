using System.Runtime.Serialization;

namespace Logic.Business.RenderingManagement.Contract.Exceptions
{
    public class RenderingManagementException : Exception
    {
        public RenderingManagementException()
        {
        }

        public RenderingManagementException(string message) : base(message)
        {
        }

        public RenderingManagementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RenderingManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
