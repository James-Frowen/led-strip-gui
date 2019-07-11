using LedStrip;
using System.Drawing;
using System.Threading;

namespace led_strip_simulation
{
    public class SimulationMessageSender : ILedMessageSender
    {
        private LEDSimulationForm form;
        private LEDS leds;

        public bool Open()
        {
            if (this.form == null)
            {
                this.form = new LEDSimulationForm();
                new Thread(() => { this.form.ShowDialog(); }).Start();

                this.leds = new LEDS(this.form);
            }
            return true;
        }
        public bool Open(string portName, int baudrate)
        {
            return this.Open();
        }
        public void Close()
        {
            if (this.form != null)
            {
                this.form.Close();
                this.form.Dispose();
                this.form = null;
            }
        }

        public void SendColor(Color color)
        {
            throw new System.NotImplementedException();
        }
        public void SendMessage(MessageBuilder builder)
        {
            throw new System.NotImplementedException();
        }
        public void SendRawBytes(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }
        public void SendValue(byte value, Codes.MessageType type)
        {
            throw new System.NotImplementedException();
        }
        public void SendValue(short value, Codes.MessageType type)
        {
            throw new System.NotImplementedException();
        }
        public void SendValues(byte[] values, Codes.MessageType type)
        {
            throw new System.NotImplementedException();
        }
    }
}
