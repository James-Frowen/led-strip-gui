using System.Collections;
using System.Drawing;

namespace LedStripGui
{
    public class HuePingPongLEDController : ThreadLEDController
    {
        private int steps = 1;
        private int hue1;
        private int hue2;

        public HuePingPongLEDController(Settings settings) : base(settings)
        {
            this.hue1 = (int)Color.Red.GetHue();
            this.hue2 = (int)Color.Yellow.GetHue();
            if (this.hue2 < this.hue1)
            {
                this.hue2 += 360;
            }
        }

        protected override IEnumerator loop()
        {
            for (int hue = this.hue1; hue < this.hue2; hue += this.steps)
            {
                var color = ColorHelper.HueToColor(hue);
                Serial.SendColor(color);

                yield return null;
            }
            for (int hue = this.hue2; hue > this.hue1; hue -= this.steps)
            {
                var color = ColorHelper.HueToColor(hue);
                Serial.SendColor(color);

                yield return null;
            }
        }
    }
}
