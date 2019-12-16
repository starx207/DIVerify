using System;
using System.Runtime.Serialization;

namespace DIVerify {
    public class ServiceVerificationException : Exception
    {
        public ServiceVerificationException()
        {
        }

        public ServiceVerificationException(string message) : base(message)
        {
        }

        public ServiceVerificationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServiceVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}