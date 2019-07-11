using System.Collections;
using System.Drawing;

namespace LedStrip.Controllers
{
    public sealed class HuePingPongLEDController : ThreadLEDController
    {
        public const int DEFAULT_STEP = 1;
        private int _minHue;
        private int _maxHue;

        public int Steps { get; set; }
        public int MinHue
        {
            get => this._minHue; set
            {
                this._minHue = value;
                this.makeHueRangeValid();
            }
        }
        public int MaxHue
        {
            get => this._maxHue; set
            {
                this._maxHue = value;
                this.makeHueRangeValid();
            }
        }

        public HuePingPongLEDController(ILedMessageSender messageSender, int updatesPerSecond, int step, int hueMin, int hueMax)
            : base(messageSender, updatesPerSecond)
        {
            this.Steps = step;
            this._minHue = hueMin;
            this._maxHue = hueMax;
            this.makeHueRangeValid();
        }
        public HuePingPongLEDController(ILedMessageSender messageSender, int updatesPerSecond, int step, Color hue1, Color hue2)
            : this(messageSender, updatesPerSecond, step, (int)hue1.GetHue(), (int)hue2.GetHue())
        {
        }

        private void makeHueRangeValid()
        {
            // make sure hue2 is always greater than hue1
            if (this._maxHue < this._minHue)
            {
                this._maxHue += 360;
            }
            System.Diagnostics.Debug.Assert(this._maxHue < this._minHue, "Invalid Hue range, Max Hue is smaller than Min Hue. Make sure inputs are correct");
        }

        protected override IEnumerator loop()
        {
            for (int hue = this.MinHue; hue < this.MaxHue; hue += this.Steps)
            {
                var color = ColorHelper.HueToColor(hue);
                this.messageSender.SendColor(color);

                yield return null;
            }
            for (int hue = this.MaxHue; hue > this.MinHue; hue -= this.Steps)
            {
                var color = ColorHelper.HueToColor(hue);
                this.messageSender.SendColor(color);

                yield return null;
            }
        }
    }
}
