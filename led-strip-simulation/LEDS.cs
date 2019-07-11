using LedStrip.Forms;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace led_strip_simulation
{
    public class LEDS
    {
        private const int LED_COUNT = 120;
        private const int ROW_COUNT = 2;
        private const int SPACE_ROUND_LED = 20;
        private const int PIXELS_PER_LED = 4;

        private readonly LEDSimulationForm form;
        private readonly LED[] leds;
        private readonly FormRenderer renderer;


        public LEDS(LEDSimulationForm form)
        {
            this.form = form;
            this.leds = new LED[LED_COUNT];

            var ledPerRow = LED_COUNT / ROW_COUNT;
            var width = ledPerRow * (PIXELS_PER_LED + SPACE_ROUND_LED);
            var height = ROW_COUNT * (PIXELS_PER_LED + SPACE_ROUND_LED);
            form.Width = width;
            form.Height = height;

            int x = SPACE_ROUND_LED / 2;
            int y = SPACE_ROUND_LED / 2;
            bool goingUp = true;
            for (int i = 0; i < ROW_COUNT; i++)
            {
                for (int j = 0; j < ledPerRow; j++)
                {
                    this.leds[i].color = Color.Red;
                    this.leds[i].position = new Point(x, y);

                    if (goingUp)
                    {
                        x += PIXELS_PER_LED * SPACE_ROUND_LED;
                    }
                    else
                    {
                        x -= PIXELS_PER_LED * SPACE_ROUND_LED;
                    }
                }
                goingUp = !goingUp;
                y += PIXELS_PER_LED * SPACE_ROUND_LED;
            }

            new Task(() =>
            {
                Thread.Sleep(1000);
                this.Render();
            }).Start();

            this.renderer = new FormRenderer(form, width, height);
        }

        public void UpdateLeds(Color[] colors)
        {
            for (int i = 0; i < LED_COUNT; i++)
            {
                this.leds[i].color = colors[i];
            }
        }
        public void Render()
        {
            using (var brush = new SolidBrush(Color.Red))
            {
                for (int i = 0; i < LED_COUNT; i++)
                {
                    brush.Color = this.leds[i].color;
                    var x = this.leds[i].position.X - (PIXELS_PER_LED / 2);
                    var y = this.leds[i].position.X - (PIXELS_PER_LED / 2);

                    this.renderer.GFX.FillRectangle(brush, x, y, PIXELS_PER_LED, PIXELS_PER_LED);
                }
            }
        }

        public class LED
        {
            public Color color;
            public Point position;
        }
    }
}
