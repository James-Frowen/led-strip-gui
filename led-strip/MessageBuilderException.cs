using System;

namespace LedStrip
{
    [Serializable]
    public class MessageBuilderException : Exception
    {
        public MessageBuilderException() { }
        public MessageBuilderException(string message) : base(message) { }
        public MessageBuilderException(string message, Exception inner) : base(message, inner) { }
        protected MessageBuilderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

