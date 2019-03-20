using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LedStripGui.ArduinoCodes;

namespace LedStripGui
{
    public static class Serial
    {
        private const int ARDUINO_UNO_BUFFER_SIZE = 64;

        private const string DEFAULT_PORT = "COM3";
        private const int DEFAULT_BAUDRATE = 9600;
        private static SerialPort port;
        public static event OnData onData;
        public delegate void OnData(byte[] data);

        private static bool waitingForData = false;

        /// <summary>
        /// Opens Connection to Serial Port
        /// </summary>
        /// <returns>true if successful Opened</returns>
        public static bool Open()
        {
            if (port != null)
            {
                MessageBox.Show("Cant open port, one is already open");

                return false;
            }

            port = new SerialPort
            {
                PortName = DEFAULT_PORT,
                BaudRate = DEFAULT_BAUDRATE,
                Encoding = Encoding.ASCII,
            };

            bool opened = true;
            try
            {
                port.Open();
                port.DataReceived += dataReceived;
                port.ErrorReceived += Port_ErrorReceived;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                opened = false;
                Close();
            }

            return opened;
        }

        private static void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private static void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (waitingForData) { return; }

            waitingForData = true;
            new Task(async () =>
            {
                await Task.Delay(50);

                byte[] data = new byte[port.BytesToRead];
                port.Read(data, 0, data.Length);

                if (onData != null)
                {
                    onData.Invoke(data);
                }

                waitingForData = false;
            }).Start();
        }

        public static void Close()
        {
            if (port == null) { return; }
            port.Close();
            port.Dispose();
            port = null;
        }

        public static void SendRawBytes(byte[] bytes)
        {
            port.Write(bytes, 0, bytes.Length);
        }
        public static void SendMessage(MessageBuilder builder)
        {
            var msg = builder.Build();

            port.Write(msg, 0, msg.Length);
        }
        public static void SendColor(Color color)
        {
            var bytes = new byte[5];
            bytes[0] = 4;
            bytes[1] = (byte)MessageType.COLOR;
            bytes[2] = color.R;
            bytes[3] = color.G;
            bytes[4] = color.B;
            port.Write(bytes, 0, 5);
        }
        public static void SendValue(byte value, MessageType type)
        {
            var bytes = new byte[3];
            bytes[0] = 2;
            bytes[1] = (byte)type;
            bytes[2] = value;
            port.Write(bytes, 0, 3);
        }
        public static void SendValue(short value, MessageType type)
        {
            var shortBytes = BitConverter.GetBytes(value);
            var bytes = new byte[4];
            bytes[0] = 3;
            bytes[1] = (byte)type;
            bytes[2] = shortBytes[0];
            bytes[3] = shortBytes[1];
            port.Write(bytes, 0, 4);
        }
        public static void SendValues(byte[] values, MessageType type)
        {
            var dataLength = values.Length;
            var msgLength = dataLength + 1; // with control code
            var fullLength = msgLength + 1; // with length
            if (fullLength > ARDUINO_UNO_BUFFER_SIZE)
            {
                throw new SerialException("Message is larger than buffer size");
            }

            var bytes = new byte[fullLength];
            bytes[0] = (byte)msgLength;
            bytes[1] = (byte)type;
            Array.Copy(values, 0, bytes, 2, dataLength);
            port.Write(bytes, 0, fullLength);
        }

        public class MessageBuilder
        {
            private MessageType messageType;
            private List<byte> message;

            public MessageBuilder(MessageType messageType)
            {
                this.messageType = messageType;
                this.message = new List<byte>();
            }

            public void Add(byte item)
            {
                this.message.Add(item);
            }
            public void Add(IEnumerable<byte> collection)
            {
                this.message.AddRange(collection);
            }

            public byte[] Build()
            {
                var length = this.message.Count + 1;
                if (length > ARDUINO_UNO_BUFFER_SIZE)
                {
                    throw new SerialException("Message is larger than buffer size");
                }


                byte[] bytes = new byte[length + 1];
                bytes[0] = (byte)length;
                bytes[1] = (byte)this.messageType;
                Array.Copy(this.message.ToArray(), 0, bytes, 2, length - 1);

                return bytes;
            }
        }

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
}
