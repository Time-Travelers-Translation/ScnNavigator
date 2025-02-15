using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.Archive
{
    [Serializable]
    public class PckReaderException : Exception
    {
        public PckReaderException()
        {
        }

        public PckReaderException(string message) : base(message)
        {
        }

        public PckReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PckReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
