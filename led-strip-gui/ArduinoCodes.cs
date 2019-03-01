using System.Drawing;

namespace led_strip_gui
{
    public static class ArduinoCodes
    {
        public const string BRIGHTNESS = "b";
        public const string COLOR = "c";
        public const string MODE = "m";
        public const string PALETTE = "p";

        public enum Mode
        {
            Manual = 0,
            Palette = 1,
            PeriodicPalette = 2,
        }
        public enum Palette
        {
            RainbowColors = 0,
            RainbowStripeColors_NoBlend = 1,
            RainbowStripeColors = 2,
            PurpleAndGreenPalette = 3,
            TotallyRandomPalette = 4,
            BlackAndWhiteStripedPalette_NoBlend = 5,
            BlackAndWhiteStripedPalette = 6,
            CloudColors = 7,
            PartyColors = 8,
            RedWhiteBluePalette_NoBlend = 9,
            RedWhiteBluePalette = 10,
        }

        public static string SetBrightnessText(int brightness)
        {
            return BRIGHTNESS + brightness.ToString();
        }
        public static string SetColorText(Color color)
        {
            return string.Format("{0}{1},{2},{3}", COLOR, (int)color.R, (int)color.B, (int)color.G);
        }
        public static string SetModeText(Mode mode)
        {
            return MODE + mode.ToString();
        }
        public static string SetPaletteText(Palette palette)
        {
            return PALETTE + ((int)palette).ToString();
        }
    }
}
