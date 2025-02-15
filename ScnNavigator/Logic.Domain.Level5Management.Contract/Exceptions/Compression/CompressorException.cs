using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.Compression
{
    public class CompressorException : Exception
    {
        public CompressorException()
        {
        }

        public CompressorException(string message) : base(message)
        {
        }

        public CompressorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CompressorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
