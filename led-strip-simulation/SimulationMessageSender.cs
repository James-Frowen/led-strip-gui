using LedStrip;
using System;
using System.Drawing;
using System.Threading;
using static led_strip_simulation.LEDS;

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
                LEDS.SetFormSize = false;
                this.form = new LEDSimulationForm()
                {
                    StartPosition = System.Windows.Forms.FormStartPosition.Manual,
                    Location = new Point(1920 - 100 - WIDTH, 1080 - 100 + 4),
                    TopMost = true,
                    Width = WIDTH,
                    Height = HEIGHT
                };
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
            this.leds.SetLedColor(color);
            this.leds.Render();
        }
        public void SendValue(byte value, Codes.MessageType type)
        {
            switch (type)
            {
                case Codes.MessageType.BRIGHTNESS:
                    this.leds.SetBrightness(value);
                    break;
                case Codes.MessageType.PALETTE_CHANGE_DIVIDER:
                case Codes.MessageType.PALETTE:
                case Codes.MessageType.MODE:
                case Codes.MessageType.UPDATES_PER_SECOND:
                    Console.WriteLine("Does nothing with Simulation Message Sender");
                    break;
                default:
                case Codes.MessageType.CONTROL_HUE:
                case Codes.MessageType.CONTROL_RGB:
                case Codes.MessageType.CONTROL_HVS:
                case Codes.MessageType.COLOR:
                case Codes.MessageType.UNSET:
                    throw new System.NotImplementedException("enum value not impleted with for this value");
            }
            this.leds.Render();
        }
        public void SendValue(short value, Codes.MessageType type)
        {
            switch (type)
            {
                case Codes.MessageType.PALETTE_CHANGE_DIVIDER:
                case Codes.MessageType.PALETTE:
                case Codes.MessageType.MODE:
                case Codes.MessageType.UPDATES_PER_SECOND:
                    Console.WriteLine("Does nothing with Simulation Message Sender");
                    break;
                default:
                case Codes.MessageType.BRIGHTNESS:
                case Codes.MessageType.CONTROL_HUE:
                case Codes.MessageType.CONTROL_RGB:
                case Codes.MessageType.CONTROL_HVS:
                case Codes.MessageType.COLOR:
                case Codes.MessageType.UNSET:
                    throw new System.NotImplementedException("enum value not impleted with for this value");
            }
            this.leds.Render();
        }
        public void SendMessage(MessageBuilder builder)
        {
            this.SendValues(builder.Messages, builder.MessageType);
        }
        public void SendRawBytes(byte[] bytes)
        {
            var length = bytes[0];
            var msgType = bytes[1];
            var message = new byte[bytes.Length - 2];
            Array.Copy(bytes, 2, message, 0, bytes.Length - 2);
            this.SendValues(message, (Codes.MessageType)msgType);
        }
        public void SendValues(byte[] values, Codes.MessageType type)
        {
            switch (type)
            {
                case Codes.MessageType.CONTROL_HUE:
                    this.setHue(values);
                    break;
                case Codes.MessageType.CONTROL_RGB:
                    this.setRGB(values);
                    break;
                case Codes.MessageType.CONTROL_HVS:
                    this.setHVS(values);
                    break;
                case Codes.MessageType.COLOR:
                    this.setColor(values);
                    break;
                case Codes.MessageType.PALETTE_CHANGE_DIVIDER:
                case Codes.MessageType.PALETTE:
                case Codes.MessageType.MODE:
                case Codes.MessageType.UPDATES_PER_SECOND:
                    Console.WriteLine("Does nothing with Simulation Message Sender");
                    break;
                default:
                case Codes.MessageType.BRIGHTNESS:
                case Codes.MessageType.UNSET:
                    throw new System.NotImplementedException("enum value not impleted with for this value");
            }
            this.leds.Render();
        }

        private void setHue(byte[] values)
        {
            var colors = new Color[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                var hue = (int)(values[i] / 255f * 360f);
                colors[i] = ColorHelper.HueToColor(hue);
            }
            this.leds.SetLedColor(colors);
        }

        private void setRGB(byte[] values)
        {
            var colors = new Color[values.Length / 3];
            for (int i = 0; i < values.Length / 3; i++)
            {
                byte r = values[i + 0];
                byte g = values[i + 1];
                byte b = values[i + 2];
                colors[i] = Color.FromArgb(r, g, b);
            }
            this.leds.SetLedColor(colors);
        }

        private void setHVS(byte[] values)
        {
            var colors = new Color[values.Length / 3];
            for (int i = 0; i < values.Length / 3; i++)
            {
                float h = values[i + 0] / 255f * 360f;
                float v = values[i + 1] / 255f;
                float s = values[i + 2] / 255f;
                colors[i] = ColorHelper.FromHls(h, v, s);
            }
            this.leds.SetLedColor(colors);
        }

        private void setColor(byte[] values)
        {
            byte r = values[0];
            byte g = values[1];
            byte b = values[2];
            var color = Color.FromArgb(r, g, b);
            this.leds.SetLedColor(color);
        }
    }
}
