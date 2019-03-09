using System.Collections;

namespace LedStripGui
{
    public class HueLEDController : ThreadLEDController
    {
        private int steps = 1;
        private byte[] bytes;

        public HueLEDController(Settings settings) : base(settings)
        {
            this.bytes = new byte[this.count];
        }

        protected override IEnumerator loop()
        {
            for (int hue = 0; hue < 255; hue += this.steps)
            {
                for (int i = 0; i < this.count; i++)
                {
                    this.bytes[i] = (byte)hue;
                }
                Serial.Send(ArduinoCodes.CONTROL_HUE);
                Serial.SendBytes(this.bytes);

                yield return null;
            }
        }
    }
}
