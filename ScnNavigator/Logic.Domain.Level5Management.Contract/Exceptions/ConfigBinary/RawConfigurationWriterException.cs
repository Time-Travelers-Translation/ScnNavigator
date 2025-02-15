using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.ConfigBinary
{
    [Serializable]
    public class RawConfigurationWriterException : Exception
    {
        public RawConfigurationWriterException()
        {
        }

        public RawConfigurationWriterException(string message) : base(message)
        {
        }

        public RawConfigurationWriterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RawConfigurationWriterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
