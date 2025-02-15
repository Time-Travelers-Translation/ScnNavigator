using System.Runtime.Serialization;

namespace UI.ScnNavigator.Resources.Contract.Exceptions
{
    public class ScnNavigatorResourcesException : Exception
    {
        public ScnNavigatorResourcesException()
        {
        }

        public ScnNavigatorResourcesException(string message) : base(message)
        {
        }

        public ScnNavigatorResourcesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScnNavigatorResourcesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
