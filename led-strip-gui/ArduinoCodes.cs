namespace LedStripGui
{
    public static class ArduinoCodes
    {
        public const int LED_COUNT = 60;

        public const string BRIGHTNESS = "b";
        public const string COLOR = "c";
        public const string MODE = "m";
        public const string PALETTE = "p";
        public const string UPDATES_PER_SECOND = "u";
        public const string PALETTE_CHANGE_DIVIDER = "q";
        public const string CONTROL_HUE = "X";
        public const string CONTROL_RGB = "Y";
        public const string CONTROL_HVS = "Z";

        public enum MessageType
        {
            BRIGHTNESS = 'b',
            COLOR = 'c',
            MODE = 'm',
            PALETTE = 'p',
            UPDATES_PER_SECOND = 'u',
            PALETTE_CHANGE_DIVIDER = 'q',
            CONTROL_HUE = 'X',
            CONTROL_RGB = 'Y',
            CONTROL_HVS = 'Z',
        }

        public enum Mode
        {
            Manual = 0, // user set color
            Palette = 1, // palette presets
            PeriodicPalette = 2, // palette presets that change Periodiccally
            Controlled = 3, // controled by Serial (hue data)
            LineFlash = 4, // A dash going across the strip
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

        [System.Obsolete("old Serial", true)]
        public static string SetBrightnessText(int brightness)
        {
            return BRIGHTNESS + brightness.ToString();
        }
        [System.Obsolete("old Serial", true)]
        public static string SetUpdatesPerSecondText(int updates)
        {
            return UPDATES_PER_SECOND + updates.ToString();
        }
        //public static string SetColorText(Color color)
        //{
        //    return string.Format("{0}{1},{2},{3}", COLOR, (int)color.R, (int)color.G, (int)color.B);
        //}
        [System.Obsolete("old Serial", true)]
        public static string SetModeText(Mode mode)
        {
            return MODE + ((int)mode).ToString();
        }
        [System.Obsolete("old Serial", true)]
        public static string SetPaletteText(Palette palette)
        {
            return PALETTE + ((int)palette).ToString();
        }
        [System.Obsolete("old Serial", true)]
        public static string SetPaletteChangeDividerText(int divider)
        {
            return PALETTE_CHANGE_DIVIDER + divider.ToString();
        }
    }
}
