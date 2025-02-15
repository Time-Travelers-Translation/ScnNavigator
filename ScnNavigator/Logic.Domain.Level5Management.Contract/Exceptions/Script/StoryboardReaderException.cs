using System;
using System.Runtime.Serialization;

namespace Logic.Domain.Level5Management.Contract.Exceptions.Script
{
    [Serializable]
    public class StoryboardReaderException : Exception
    {
        public StoryboardReaderException()
        {
        }

        public StoryboardReaderException(string message) : base(message)
        {
        }

        public StoryboardReaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StoryboardReaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
