using System.Collections;

namespace LedStripGui
{
    public class HueCycleLEDController : ThreadLEDController
    {
        private readonly int steps;

        public HueCycleLEDController(Settings settings) : base(settings)
        {
            this.steps = 1;
        }

        protected override IEnumerator loop()
        {
            for (int hue = 0; hue < 360; hue += this.steps)
            {
                var color = ColorHelper.HueToColor(hue);
                Serial.SendColor(color);

                yield return null;
            }
        }
    }
}
