using System.IO.Ports;
using System.Windows.Forms;

namespace led_strip_gui
{
    public static class USB
    {
        private const string DEFAULT_PORT = "COM3";
        private const int DEFAULT_BAUDRATE = 9600;
        private static SerialPort port;
        public static void Open()
        {
            if (port != null)
            {
                MessageBox.Show("Cant open port, one is already open");

                return;
            }

            port = new SerialPort
            {
                PortName = DEFAULT_PORT,
                BaudRate = DEFAULT_BAUDRATE
            };
            port.Open();
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
