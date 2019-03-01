using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace led_strip_gui
{
    public static class Serial
    {
        private const string DEFAULT_PORT = "COM3";
        private const int DEFAULT_BAUDRATE = 9600;
        private static SerialPort port;
        public static event OnData onData;
        public delegate void OnData(byte[] data);

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
                BaudRate = DEFAULT_BAUDRATE
            };

            bool opened = true;
            try
            {
                port.Open();
                port.DataReceived += dataReceived;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                opened = false;
                Close();
            }

            return opened;
        }

        private static void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[port.BytesToRead];
            port.Read(data, 0, data.Length);

            if (onData != null)
            {
                onData.Invoke(data);
            }
        }

        public static void Close()
        {
            if (port == null) { return; }
            port.Close();
            port.Dispose();
            port = null;
        }
        public static void Send(string text)
        {
            port.Write(text);
        }
    }
}
