using ScreenColor;
using System.Collections;
using System.Drawing;

namespace LedStripGui
{
    public class ScreenLEDController : ThreadLEDController
    {
        private ReadScreenColor readScreenColor;

        public ScreenLEDController(Settings settings) : base(settings)
        {
        }

        protected override IEnumerator loop()
        {
            this.sendColorsOfScreenToSerial();
            yield return null;
        }

        private void sendColorsOfScreenToSerial()
        {
            Color[] colors = this.getColorsOfScreen();

            byte[] bytes;
            string controlCode;
            if (this.settings.RgbControlMode)
            {
                bytes = this.getHSVBytes(colors);
                controlCode = ArduinoCodes.CONTROL_HVS;
            }
            else
            {
                bytes = this.getHueBytes(colors);
                controlCode = ArduinoCodes.CONTROL_HUE;
            }

            throw new System.NotImplementedException();

            //Serial.Send(controlCode);
            //Serial.SendBytes(bytes);
        }

        private byte[] getRGBBytes(Color[] colors)
        {
            byte[] bytes = new byte[this.count * 3];
            for (int i = 0; i < this.count; i++)
            {
                bytes[i * 3] = colors[i].R;
                bytes[i * 3 + 1] = colors[i].G;
                bytes[i * 3 + 2] = colors[i].B;
            }

            return bytes;
        }
        private byte[] getHSVBytes(Color[] colors)
        {
            byte[] bytes = new byte[this.count * 3];
            for (int i = 0; i < this.count; i++)
            {
                bytes[i * 3] = colors[i].GetByteHue();
                bytes[i * 3 + 1] = colors[i].GetByteSaturation(1.5f);
                bytes[i * 3 + 2] = colors[i].GetByteValue();
            }

            return bytes;
        }
        private byte[] getHueBytes(Color[] colors)
        {
            byte[] bytes = new byte[this.count];
            for (int i = 0; i < this.count; i++)
            {
                bytes[i] = colors[i].GetByteHue();
            }

            return bytes;
        }

        private Color[] getColorsOfScreen()
        {
            this.readScreenColor = new ReadScreenColor();
            ReadScreenColor.ScreenSize = new Size(1920 * 2, 1080);
            ReadScreenColor.AverageSize = new Size(this.count, 1);
            this.readScreenColor.CopyFromScreen();
            var colors = this.readScreenColor.GraphicsDrawImage();
            return colors;
        }
    }
}
