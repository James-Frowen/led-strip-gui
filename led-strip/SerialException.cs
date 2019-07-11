using System;

namespace LedStrip
{
    [Serializable]
    public class SerialException : Exception
    {
        public SerialException() { }
        public SerialException(string message) : base(message) { }
        public SerialException(string message, Exception inner) : base(message, inner) { }
        protected SerialException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

