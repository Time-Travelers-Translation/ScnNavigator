using System.Runtime.Serialization;

namespace UI.ScnNavigator.Forms.Contract.Exceptions
{
    public class ScnNavigatorFormsException : Exception
    {
        public ScnNavigatorFormsException()
        {
        }

        public ScnNavigatorFormsException(string message) : base(message)
        {
        }

        public ScnNavigatorFormsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScnNavigatorFormsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
