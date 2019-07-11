using System;
using System.Collections;

namespace LedStrip.Controllers
{
    public class RandomLEDController : ThreadLEDController
    {
        private LED[] leds;
        private byte[] buffer;

        private int _minHue;
        private int _maxHue;
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

        private void makeHueRangeValid()
        {
            // make sure hue2 is always greater than hue1
            if (this._maxHue < this._minHue)
            {
                this._maxHue += 360;
            }
            System.Diagnostics.Debug.Assert(this._maxHue < this._minHue, "Invalid Hue range, Max Hue is smaller than Min Hue. Make sure inputs are correct");
        }

        public RandomLEDController(ILedMessageSender messageSender, int updatesPerSecond, int ledCount, int minHue, int maxHue)
            : base(messageSender, updatesPerSecond)
        {
            this._minHue = minHue;
            this._maxHue = maxHue;
            this.makeHueRangeValid();

            this.leds = new LED[ledCount];
            this.buffer = new byte[ledCount];
            for (int i = 0; i < ledCount; i++)
            {
                this.leds[i] = new LED(this)
                {
                    hue = (this.MaxHue + this.MinHue) / 2
                };
            }
        }

        protected override IEnumerator loop()
        {
            for (int i = 0; i < this.leds.Length; i++)
            {
                this.leds[i].Update();
            }
            LED.UpdateBuffer(this.buffer, this.leds);

            var builder = new MessageBuilder(Codes.MessageType.CONTROL_HUE);
            builder.Add(this.buffer);
            this.messageSender.SendMessage(builder);

            yield return null;

        }

        private struct LED
        {
            private static Random r = new Random();
            private RandomLEDController controller;
            public int hue;
            private const int CHANGE = 10;

            public LED(RandomLEDController controller) : this()
            {
                this.controller = controller;
            }

            public void Update()
            {
                bool increase = r.Next(2) == 1;
                this.hue += (increase ? 1 : -1) * CHANGE;

                if (this.hue < this.controller.MinHue) { this.hue += CHANGE; }
                if (this.hue > this.controller.MaxHue) { this.hue -= CHANGE; }
            }
            public byte ToByte()
            {
                return ColorHelper.HueToColor(this.hue).GetByteHue();
            }

            public static void UpdateBuffer(byte[] buffer, LED[] leds)
            {
                for (int i = 0; i < leds.Length; i++)
                {
                    buffer[i] = leds[i].ToByte();
                }
            }
        }
    }
}
