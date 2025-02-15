using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.Cryptography
{
    public class ChecksumFactoryException : Exception
    {
        public ChecksumFactoryException()
        {
        }

        public ChecksumFactoryException(string message) : base(message)
        {
        }

        public ChecksumFactoryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ChecksumFactoryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
