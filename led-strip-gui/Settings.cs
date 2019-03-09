using System.Drawing;

namespace LedStripGui
{
    public class Settings
    {
        public const int MIN_BRIGHTNESS = 0;
        public const int MAX_BRIGHTNESS = 255;
        public const int MIN_UPDATES_PER_SECOND = 0;
        public const int MAX_UPDATES_PER_SECOND = 200;
        public const int MIN_PALETTE_CHANGE_DIVIDER = 500;
        public const int MAX_PALETTE_CHANGE_DIVIDER = 30000;

        public readonly int ledCount;
        private int _brightness = 4;
        private int _updatesPerSecond = 10;
        private int _paletteChangeDivider = 1000;
        private bool _rgbControlMode = true;
        public Color color = Color.White;
        public ArduinoCodes.Mode mode = ArduinoCodes.Mode.Manual;
        public ArduinoCodes.Palette palette = ArduinoCodes.Palette.RainbowColors;

        public Settings(int ledCount)
        {
            this.ledCount = ledCount;
        }

        public int Brightness
        {
            get
            {
                return this._brightness;
            }

            set
            {
                if (value > MAX_BRIGHTNESS) { this._brightness = MAX_BRIGHTNESS; }
                else if (value < MIN_BRIGHTNESS) { this._brightness = MIN_BRIGHTNESS; }
                else { this._brightness = value; }
            }
        }

        public int UpdatesPerSecond
        {
            get
            {
                return this._updatesPerSecond;
            }

            set
            {
                if (value > MAX_UPDATES_PER_SECOND) { this._updatesPerSecond = MAX_UPDATES_PER_SECOND; }
                else if (value < MIN_UPDATES_PER_SECOND) { this._updatesPerSecond = MIN_UPDATES_PER_SECOND; }
                else { this._updatesPerSecond = value; }
            }
        }

        public int PaletteChangeDivider
        {
            get
            {
                return this._paletteChangeDivider;
            }

            set
            {
                if (value > MAX_PALETTE_CHANGE_DIVIDER) { this._paletteChangeDivider = MAX_PALETTE_CHANGE_DIVIDER; }
                else if (value < MIN_PALETTE_CHANGE_DIVIDER) { this._paletteChangeDivider = MIN_PALETTE_CHANGE_DIVIDER; }
                else { this._paletteChangeDivider = value; }
            }
        }

        public bool RgbControlMode
        {
            get
            {
                return this._rgbControlMode;
            }

            set
            {
                this._rgbControlMode = value;
            }
        }
    }
}
