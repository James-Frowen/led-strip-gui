using System.Drawing;

namespace LedStrip
{
    public interface ILedMessageSender
    {
        void Close();
        bool Open();
        bool Open(string portName, int baudrate);
        void SendColor(Color color);
        void SendMessage(MessageBuilder builder);
        void SendRawBytes(byte[] bytes);
        void SendValue(byte value, Codes.MessageType type);
        void SendValue(short value, Codes.MessageType type);
        void SendValues(byte[] values, Codes.MessageType type);
    }
}

