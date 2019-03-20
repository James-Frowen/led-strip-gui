using System;
using System.Collections;
using System.Drawing;

namespace LedStripGui
{
    public class RandomLEDController : ThreadLEDController
    {
        private int minHue;
        private int maxHue;
        private LED[] leds;
        private byte[] buffer;

        public RandomLEDController(Settings settings) : base(settings)
        {
            this.minHue = (int)Color.Red.GetHue();
            this.maxHue = (int)Color.Yellow.GetHue();
            if (this.maxHue < this.minHue)
            {
                this.maxHue += 360;
            }
            this.leds = new LED[settings.ledCount];
            this.buffer = new byte[settings.ledCount];
            for (int i = 0; i < this.leds.Length; i++)
            {
                this.leds[i] = new LED(this)
                {
                    hue = (this.maxHue + this.minHue) / 2
                };
            }
        }

        protected override IEnumerator loop()
        {
            for (int i = 0; i < this.leds.Length; i++)
            {
                this.leds[i].Update();
            }
            LED.UpdateBytes(this.buffer, this.leds);
            //Serial.Send(ArduinoCodes.CONTROL_HUE);
            //Serial.SendBytes(this.buffer);

            throw new System.NotImplementedException();
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

                if (this.hue < this.controller.minHue) { this.hue += CHANGE; }
                if (this.hue > this.controller.maxHue) { this.hue -= CHANGE; }
            }
            public byte ToByte()
            {
                return ColorHelper.HueToColor(this.hue).GetByteHue();
            }

            public static void UpdateBytes(byte[] buffer, LED[] leds)
            {
                for (int i = 0; i < leds.Length; i++)
                {
                    buffer[i] = leds[i].ToByte();
                }
            }
        }
    }
}
