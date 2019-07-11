using System.Drawing;

namespace LedStrip
{
    public static class ColorExtension
    {
        public static byte GetByteHue(this Color c)
        {
            var hue = c.GetHue();
            // hue range from 0 to 360
            var b = (byte)((int)(hue / 360f * 255f));

            return b;
        }
        public static byte GetByteSaturation(this Color c, float saturationIncrease = 1f)
        {
            var colorSat = c.GetSaturation();
            // increase saturation by halving the distance to 1
            var oneMinusSat = 1 - colorSat;
            var realSat = 1 - (oneMinusSat / saturationIncrease);

            // sat range from 0 to 1
            var b = (byte)((int)(realSat * 255f));

            return b;
        }
        public static byte GetByteValue(this Color c)
        {
            var val = c.GetBrightness();
            // val range from 0 to 1
            var b = (byte)((int)(val * 255f));

            return b;
        }
    }
}

