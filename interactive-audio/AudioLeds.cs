using led_strip_simulation;
using LedStrip;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace interactive_audio
{
    public class AudioLeds
    {
        private readonly ILedMessageSender messageSender;
        private readonly MessageBuilder builder;

        private SmoothBuffer max;
        private SmoothBuffer buffer;
        private int values = 0;
        private const int minTime = 66;
        private const int minValues = 4;

        public AudioLeds()
        {
            //this.messageSender = new SimulationMessageSender();
            this.messageSender = Serial.Instance;
            Serial.onData += onData;
            this.messageSender.Open();
            this.messageSender.SendValue(64, Codes.MessageType.BRIGHTNESS);
            this.builder = new MessageBuilder
            {
                MessageType = Codes.MessageType.CONTROL_AUDIOBANDS
            };
            this.buffer = new SmoothBuffer(10, 0.2f, true);
            this.max = new SmoothBuffer(1, 0.05f, true);
            new Thread(this.updater).Start();
        }

        private static void onData(byte[] data)
        {
            var text = Encoding.ASCII.GetString(data);
            Console.WriteLine(DateTime.Now.TimeOfDay);
            Console.WriteLine(text);
            Console.WriteLine();
        }
        private void updater()
        {
            int timer = 0;
            while (true)
            {

                Thread.Sleep(1);
                timer++;
                if (timer >= minTime && this.values >= minValues)
                {
                    timer = 0;
                    this.values = 0;


                    float[] bands;
                    lock (this.buffer)
                    {
                        bands = this.buffer.GetBuffer();
                    }
                    this.sendMesssage(bands);
                }

            }
        }

        public void Show(float[] bands)
        {
            this.values++;
            lock (this.buffer)
            {
                this.buffer.AddToBuffer(bands);
            }
            this.max.AddToBuffer(new float[] { bands.Max() });
        }

        private void sendMesssage(float[] bands)
        {
            this.builder.Clear();

            //const float scale = 6000f / 25 / 2; // max * formHeight
            for (int i = 0; i < bands.Length; i++)
            {
                int value = (int)(bands[i] / this.max.GetBuffer()[0] * 255 * 1.2);
                int clamp = Math.Min(value, 255);
                this.builder.Add((byte)clamp);
            }
            this.messageSender.SendMessage(this.builder);
        }

        private void sendMesssageLong(float[] bands)
        {
            this.builder.Clear();

            const float scale = 6000f / 25 / 2; // max * formHeight

            int ledsPerBand = LEDS.LED_PER_ROW / bands.Length; // 60 / 10 = 6
            int ledPerSide = ledsPerBand / 2; // 3


            //Console.WriteLine(string.Join(",", bands.Select(x => (x * scale).ToString("0.0"))));
            for (int i = 0; i < bands.Length; i++)
            {
                var percent = bands[i] * scale;
                for (int j = 0; j < ledPerSide; j++)
                {
                    this.calculateLedValues(percent, j);
                }
                for (int j = ledPerSide - 1; j >= 0; j--)
                {
                    this.calculateLedValues(percent, j);
                }
            }
            //this.builder.DuplicateInReverse();
            this.messageSender.SendMessage(this.builder);
        }

        private void calculateLedValues(float percent, int index)
        {
            float map;

            if (index == 0)
            {
                map = (percent * 2);
            }
            else if (index == 1)
            {
                map = (float)Math.Max(0, (percent * 2 - 0.25));
            }
            else if (index == 2)
            {
                map = (float)Math.Max(0, (percent * 2 - 0.5));
            }
            else
            {
                throw new System.Exception("index should not be greater than 2");
            }
            //int mapScaled = (int)(map * 255);
            byte v = (byte)((int)Math.Min(map * 255, 255));
            byte s = 0;

            byte h = this.hueStep(v);

            this.builder.Add(h);
            //this.builder.Add(s);
            //this.builder.Add(v);
        }

        private byte hueStep(byte v)
        {
            return (byte)(v / 3);
        }
    }
}
