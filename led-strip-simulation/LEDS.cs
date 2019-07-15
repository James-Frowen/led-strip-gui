using LedStrip.Forms;
using System;
using System.Drawing;

namespace led_strip_simulation
{
    public class LEDS
    {
        public const int LED_COUNT = 120;
        public const int ROW_COUNT = 2;
        private const int SPACE_ROUND_LED = 20;
        private const int PIXELS_PER_LED = 4;

        public const int LED_PER_ROW = LED_COUNT / ROW_COUNT;
        public const int WIDTH = LED_PER_ROW * DRAW_STEP;
        public const int HEIGHT = ROW_COUNT * DRAW_STEP;
        private const int DRAW_STEP = PIXELS_PER_LED + SPACE_ROUND_LED;

        public static bool SetFormSize { get; set; } = true;

        private readonly LEDSimulationForm form;
        private readonly LED[] leds;
        private FormRenderer renderer;

        public int Count => LED_COUNT;

        public LEDS(LEDSimulationForm form)
        {
            this.form = form;
            this.leds = new LED[LED_COUNT];

            form.Load += this.form_Ready;

            int x = SPACE_ROUND_LED / 2;
            int y = SPACE_ROUND_LED / 2;

            bool goingUp = true;
            for (int i = 0; i < ROW_COUNT; i++)
            {
                for (int j = 0; j < LED_PER_ROW; j++)
                {
                    this.leds[i * LED_PER_ROW + j].color = Color.Red;
                    this.leds[i * LED_PER_ROW + j].position = new Point(x, y);

                    if (goingUp)
                    {
                        x += DRAW_STEP;
                    }
                    else
                    {
                        x -= DRAW_STEP;
                    }
                }
                goingUp = !goingUp;
                y += DRAW_STEP;
                x -= DRAW_STEP;
            }
        }

        public void SetBrightness(int value)
        {
            throw new NotImplementedException();
        }

        private void form_Ready(object sender, EventArgs e)
        {
            if (SetFormSize)
            {
                this.form.Width = WIDTH;
                this.form.Height = HEIGHT;
            }

            this.renderer = new FormRenderer(this.form, WIDTH, HEIGHT);

            this.Render();
        }

        public void SetLedColor(Color color)
        {
            for (int i = 0; i < LED_COUNT; i++)
            {
                this.leds[i].color = color;
            }
        }
        public void SetLedColor(Color[] colors)
        {
            for (int i = 0; i < LED_COUNT; i++)
            {
                this.leds[i].color = colors[i];
            }
        }
        public void Render()
        {
            this.renderer.GFX.Clear(Color.Black);
            using (var brush = new SolidBrush(Color.Red))
            {
                for (int i = 0; i < LED_COUNT; i++)
                {
                    brush.Color = this.leds[i].color;
                    var x = this.leds[i].position.X - (PIXELS_PER_LED / 2);
                    var y = this.leds[i].position.Y - (PIXELS_PER_LED / 2);

                    this.renderer.GFX.FillRectangle(brush, x, y, PIXELS_PER_LED, PIXELS_PER_LED);
                }
            }
            this.renderer.FinishDraw();
        }

        public struct LED
        {
            public Color color;
            public Point position;
        }
    }
}
