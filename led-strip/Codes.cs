namespace LedStrip
{
    public static class Codes
    {
        public const int LED_COUNT = 60;

        public const int MIN_BRIGHTNESS = 0;
        public const int MAX_BRIGHTNESS = 255;
        public const int MIN_UPDATES_PER_SECOND = 0;
        public const int MAX_UPDATES_PER_SECOND = 200;
        public const int MIN_PALETTE_CHANGE_DIVIDER = 500;
        public const int MAX_PALETTE_CHANGE_DIVIDER = 30000;

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
            UNSET = 0,
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
    }
}

