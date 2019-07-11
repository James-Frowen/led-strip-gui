using LedStrip;
using System.Drawing;

namespace LedStripGui
{
    [System.Obsolete("Old form settings")]
    public class Settings
    {
        public readonly int ledCount;
        private int _brightness = 4;
        private int _updatesPerSecond = 10;
        private int _paletteChangeDivider = 1000;
        private bool _rgbControlMode = false;
        public Color color = Color.White;
        public Codes.Mode mode = Codes.Mode.Manual;
        public Codes.Palette palette = Codes.Palette.RainbowColors;

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
                if (value > Codes.MAX_BRIGHTNESS) { this._brightness = Codes.MAX_BRIGHTNESS; }
                else if (value < Codes.MIN_BRIGHTNESS) { this._brightness = Codes.MIN_BRIGHTNESS; }
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
                if (value > Codes.MAX_UPDATES_PER_SECOND) { this._updatesPerSecond = Codes.MAX_UPDATES_PER_SECOND; }
                else if (value < Codes.MIN_UPDATES_PER_SECOND) { this._updatesPerSecond = Codes.MIN_UPDATES_PER_SECOND; }
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
                if (value > Codes.MAX_PALETTE_CHANGE_DIVIDER) { this._paletteChangeDivider = Codes.MAX_PALETTE_CHANGE_DIVIDER; }
                else if (value < Codes.MIN_PALETTE_CHANGE_DIVIDER) { this._paletteChangeDivider = Codes.MIN_PALETTE_CHANGE_DIVIDER; }
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
