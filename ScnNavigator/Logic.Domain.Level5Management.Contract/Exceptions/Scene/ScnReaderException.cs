using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.Scene
{
    [Serializable]
    public class ScnReaderException : Exception
    {
        public ScnReaderException()
        {
        }

        public ScnReaderException(string message) : base(message)
        {
        }

        public ScnReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScnReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
