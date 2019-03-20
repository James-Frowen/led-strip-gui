using System.Collections;

namespace LedStripGui
{
    public class HueCyleLEDController : ThreadLEDController
    {
        private int steps = 1;

        public HueCyleLEDController(Settings settings) : base(settings)
        {
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
