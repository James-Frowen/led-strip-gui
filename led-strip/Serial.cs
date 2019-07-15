using System;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace LedStrip
{
    public class Serial : ILedMessageSender
    {
        private static Serial _instance;
        public static Serial Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Serial();
                    MessageBuilder.BufferSize = Serial.ARDUINO_UNO_BUFFER_SIZE;
                }
                return _instance;
            }
        }

        public const int ARDUINO_UNO_BUFFER_SIZE = 64;

        public const string DEFAULT_PORT = "COM3";
        public const int DEFAULT_BAUDRATE = 9600;

        public delegate void OnData(byte[] data);
        public static event OnData onData;

        private SerialPort port;

        private bool waitingForData = false;

        /// <summary>
        /// Opens Connection to Serial Port
        /// </summary>
        /// <returns>true if successful Opened</returns>
        public bool Open()
        {
            return this.Open(DEFAULT_PORT, DEFAULT_BAUDRATE);
        }
        public bool Open(string portName, int baudrate)
        {
            if (this.port != null)
            {
                Console.WriteLine("Cant open port, one is already open");

                return false;
            }

            this.port = new SerialPort
            {
                PortName = portName,
                BaudRate = baudrate,
                Encoding = Encoding.ASCII,
            };

            bool opened = true;
            try
            {
                this.port.DataReceived += this.dataReceived;
                this.port.ErrorReceived += port_ErrorReceived;
                this.port.Open();
            }
            catch (Exception e)
            {
                opened = false;
                this.Close();

                throw e;
            }

            return opened;
        }
        public void Close()
        {
            if (this.port == null) { return; }
            this.port.Close();
            this.port.Dispose();
            this.port = null;
        }

        private static void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this.waitingForData) { return; }

            this.waitingForData = true;
            new Task(async () =>
            {
                await Task.Delay(50);

                byte[] data = new byte[this.port.BytesToRead];
                this.port.Read(data, 0, data.Length);

                if (onData != null)
                {
                    onData.Invoke(data);
                }

                this.waitingForData = false;
            }).Start();
        }

        public void SendRawBytes(byte[] bytes)
        {
            this.port.Write(bytes, 0, bytes.Length);
        }
        public void SendMessage(MessageBuilder builder)
        {
            var msg = builder.Build();

            this.port.Write(msg, 0, msg.Length);
        }
        public void SendColor(Color color)
        {
            var bytes = new byte[5];
            bytes[0] = 4;
            bytes[1] = (byte)Codes.MessageType.COLOR;
            bytes[2] = color.R;
            bytes[3] = color.G;
            bytes[4] = color.B;
            this.port.Write(bytes, 0, 5);
        }
        public void SendValue(byte value, Codes.MessageType type)
        {
            var bytes = new byte[3];
            bytes[0] = 2;
            bytes[1] = (byte)type;
            bytes[2] = value;
            this.port.Write(bytes, 0, 3);
        }
        public void SendValue(short value, Codes.MessageType type)
        {
            var shortBytes = BitConverter.GetBytes(value);
            var bytes = new byte[4];
            bytes[0] = 3;
            bytes[1] = (byte)type;
            bytes[2] = shortBytes[0];
            bytes[3] = shortBytes[1];
            this.port.Write(bytes, 0, 4);
        }
        public void SendValues(byte[] values, Codes.MessageType type)
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
            this.port.Write(bytes, 0, fullLength);
        }
    }
}
