using System;
using System.Collections.Generic;

namespace LedStrip
{
    public sealed class MessageBuilder
    {
        public Codes.MessageType MessageType { get; set; } = Codes.MessageType.UNSET;
        public static int BufferSize { get; set; } = int.MaxValue;

        private List<byte> message = new List<byte>();

        public MessageBuilder(Codes.MessageType messageType)
        {
            this.MessageType = messageType;
        }
        public MessageBuilder()
        {
        }

        public void Add(byte item)
        {
            this.message.Add(item);
        }
        public void Add(IEnumerable<byte> collection)
        {
            this.message.AddRange(collection);
        }

        public void Clear()
        {
            this.message.Clear();
        }

        public byte[] Build()
        {
            if (this.MessageType == Codes.MessageType.UNSET)
            {
                throw new MessageBuilderException("MessageType should not be 'UNSET'");
            }

            var length = this.message.Count + 1;
            if (length > BufferSize)
            {
                throw new SerialException("Message is larger than buffer size");
            }

            byte[] bytes = new byte[length + 1];
            bytes[0] = (byte)length;
            bytes[1] = (byte)this.MessageType;
            Array.Copy(this.message.ToArray(), 0, bytes, 2, length - 1);

            return bytes;
        }
    }
}

