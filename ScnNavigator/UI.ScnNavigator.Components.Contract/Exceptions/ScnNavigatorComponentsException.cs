using System.Runtime.Serialization;

namespace UI.ScnNavigator.Components.Contract.Exceptions
{
    public class ScnNavigatorComponentsException : Exception
    {
        public ScnNavigatorComponentsException()
        {
        }

        public ScnNavigatorComponentsException(string message) : base(message)
        {
        }

        public ScnNavigatorComponentsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScnNavigatorComponentsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
