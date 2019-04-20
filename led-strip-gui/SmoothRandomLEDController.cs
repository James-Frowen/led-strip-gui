using System.Collections;
using System.Drawing;

namespace LedStripGui
{
    public class SmoothRandomLEDController : ThreadLEDController
    {
        private readonly float secondsBetweenChange;
        private readonly float transiationTime;

        private Color current;

        public SmoothRandomLEDController(Settings settings) : base(settings)
        {
            this.secondsBetweenChange = 25f;
            this.transiationTime = 5f;
        }

        protected override IEnumerator loop()
        {
            var next = ColorHelper.RandomHue();
            var dt = 1f / this.settings.UpdatesPerSecond;
            for (float t = 0f; t < this.transiationTime; t += dt)
            {
                float percent = t / this.transiationTime;
                var c = ColorHelper.Lerp(this.current, next, percent);
                Serial.SendColor(c);
                yield return null;
            }

            this.current = next;

            Serial.SendColor(this.current);

            for (float t = 0f; t < this.secondsBetweenChange; t += dt)
            {
                yield return null;
            }
        }
    }
}
