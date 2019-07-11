using ScreenColor;
using System.Collections;
using System.Drawing;

namespace LedStrip.Controllers
{
    public class ScreenLEDController : ThreadLEDController
    {
        private readonly ReadScreenColor readScreenColor;
        private readonly int ledCount;
        private MessageMode controlMode;

        private readonly byte[] buffer;
        private readonly Color[] colorBuffer;
        private readonly MessageBuilder builder;
        private readonly IMessageControl control;

        public ScreenLEDController(ILedMessageSender messageSender, int updatesPerSecond, int ledCount, MessageMode controlMode, bool useBothSceens)
            : base(messageSender, updatesPerSecond)
        {
            this.ledCount = ledCount;
            this.controlMode = controlMode;

            this.readScreenColor = new ReadScreenColor();
            var screenWidth = (useBothSceens ? 2 : 1) * 1920;
            ReadScreenColor.ScreenSize = new Size(screenWidth, 1080);
            ReadScreenColor.AverageSize = new Size(this.ledCount, 1);

            this.builder = new MessageBuilder();

            int bufferLength;
            switch (this.controlMode)
            {
                case MessageMode.RGB:
                    this.builder.MessageType = Codes.MessageType.CONTROL_HVS;
                    this.control = new RGBMessage();
                    bufferLength = ledCount * 3;
                    break;
                case MessageMode.HSV:
                    this.builder.MessageType = Codes.MessageType.CONTROL_HVS;
                    this.control = new HSVMessage();
                    bufferLength = ledCount * 3;
                    break;
                case MessageMode.H:
                    this.builder.MessageType = Codes.MessageType.CONTROL_HUE;
                    this.control = new HMessage();
                    bufferLength = ledCount;
                    break;
                default:
                    throw new System.ArgumentException("MessageMode has invalid value");
            }


            this.buffer = new byte[bufferLength];
            this.colorBuffer = new Color[ledCount];
        }

        protected override IEnumerator loop()
        {
            this.sendColorsOfScreenToSerial();
            yield return null;
        }

        private void sendColorsOfScreenToSerial()
        {
            this.getColorsOfScreen();

            this.control.UpdateBuffer(this.buffer, this.colorBuffer);
            this.builder.Add(this.buffer);

            this.messageSender.SendMessage(this.builder);
        }

        private void getColorsOfScreen()
        {
            this.readScreenColor.CopyFromScreen();
            this.readScreenColor.GraphicsDrawImageNonAlloc(this.colorBuffer);
        }


        public enum MessageMode
        {
            /// <summary>
            /// Red Green Blue  
            /// </summary>
            RGB,
            /// <summary>
            /// Hue Saturation Value
            /// </summary>
            HSV,
            /// <summary>
            /// Hue
            /// </summary>
            H
        }
        private interface IMessageControl
        {
            void UpdateBuffer(byte[] buffer, Color[] colors);
        }
        private class RGBMessage : IMessageControl
        {
            public void UpdateBuffer(byte[] buffer, Color[] colors)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i * 3] = colors[i].R;
                    buffer[i * 3 + 1] = colors[i].G;
                    buffer[i * 3 + 2] = colors[i].B;
                }
            }
        }
        private class HSVMessage : IMessageControl
        {
            public void UpdateBuffer(byte[] buffer, Color[] colors)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i * 3] = colors[i].GetByteHue();
                    buffer[i * 3 + 1] = colors[i].GetByteSaturation(1.5f);
                    buffer[i * 3 + 2] = colors[i].GetByteValue();
                }

            }
        }
        private class HMessage : IMessageControl
        {
            public void UpdateBuffer(byte[] buffer, Color[] colors)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = colors[i].GetByteHue();
                }

            }
        }
    }
}
