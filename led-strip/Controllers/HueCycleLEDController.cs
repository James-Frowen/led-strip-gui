using System.Collections;

namespace LedStrip.Controllers
{
    public sealed class HueCycleLEDController : ThreadLEDController
    {
        public const int DEFAULT_STEP = 1;

        /// <summary>
        /// Hue step each update
        /// </summary>
        public int Step { get; set; }

        public HueCycleLEDController(int ledCount, ILedMessageSender messageSender, int updatesPerSecond, int step)
            : base(messageSender, updatesPerSecond)
        {
            this.Step = step;
        }

        protected override IEnumerator loop()
        {
            for (int hue = 0; hue < 360; hue += this.Step)
            {
                var color = ColorHelper.HueToColor(hue);
                this.messageSender.SendColor(color);

                yield return null;
            }
        }
    }
}
