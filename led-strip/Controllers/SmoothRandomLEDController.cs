using System.Collections;
using System.Drawing;

namespace LedStrip.Controllers
{
    public class SmoothRandomLEDController : ThreadLEDController
    {
        public const float DEFAULT_SECONDS_BETWEEN_CHANGE = 25f;
        public const float DEFAULT_TRANSITION_TIME = 5f;

        public float SecondsBetweenChange { get; set; }
        public float TransitionTime { get; set; }

        private Color current;

        public SmoothRandomLEDController(ILedMessageSender messageSender, int updatesPerSecond, float secondsBetweenChange, float transiationTime)
            : base(messageSender, updatesPerSecond)
        {
            this.SecondsBetweenChange = secondsBetweenChange;
            this.TransitionTime = transiationTime;
        }

        protected override IEnumerator loop()
        {
            var next = ColorHelper.RandomHue();
            var dt = 1f / this.UpdatesPerSecond;
            for (float t = 0f; t < this.TransitionTime; t += dt)
            {
                float percent = t / this.TransitionTime;
                var c = ColorHelper.Lerp(this.current, next, percent);
                this.messageSender.SendColor(c);
                yield return null;
            }

            this.current = next;

            this.messageSender.SendColor(this.current);

            for (float t = 0f; t < this.SecondsBetweenChange; t += dt)
            {
                yield return null;
            }
        }
    }
}
