using System.Drawing;

namespace led_strip_gui
{
    public class Settings
    {
        private int brightness = 4;
        public Color color = Color.White;
        public ArduinoCodes.Mode mode = ArduinoCodes.Mode.Manual;
        public ArduinoCodes.Palette palette = ArduinoCodes.Palette.RainbowColors;

        public int Brightness
        {
            get
            {
                return this.brightness;
            }

            set
            {
                if (value < 0) { this.brightness = 0; }
                else if (value > 255) { this.brightness = 255; }
                else { this.brightness = value; }
            }
        }
    }
}
