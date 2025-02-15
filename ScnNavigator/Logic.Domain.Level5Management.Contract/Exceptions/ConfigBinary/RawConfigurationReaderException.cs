using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.ConfigBinary
{
    [Serializable]
    public class RawConfigurationReaderException : Exception
    {
        public RawConfigurationReaderException()
        {
        }

        public RawConfigurationReaderException(string message) : base(message)
        {
        }

        public RawConfigurationReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RawConfigurationReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
