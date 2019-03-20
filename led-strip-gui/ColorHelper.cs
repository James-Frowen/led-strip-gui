using System.Drawing;

namespace LedStripGui
{
    public static class ColorHelper
    {
        public static Color HueToColor(int hue)
        {
            if (hue > 360) { hue -= 360; }
            if (hue < 0) { hue += 360; }
            int r, g, b;
            HlsToRgb(hue, 0.5, 1, out r, out g, out b);
            return Color.FromArgb(r, g, b);
        }
        // Convert an HLS value into an RGB value.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="h">0 -> 360</param>
        /// <param name="l">0 -> 1</param>
        /// <param name="s">0 -> 1</param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5)
            {
                p2 = l * (1 + s);
            }
            else
            {
                p2 = l + s - l * s;
            }

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }
        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360)
            {
                hue -= 360;
            }
            else if (hue < 0)
            {
                hue += 360;
            }

            if (hue < 60)
            {
                return q1 + (q2 - q1) * hue / 60;
            }

            if (hue < 180)
            {
                return q2;
            }

            if (hue < 240)
            {
                return q1 + (q2 - q1) * (240 - hue) / 60;
            }

            return q1;
        }
    }
}
