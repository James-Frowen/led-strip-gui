using interactive_audio;
using System;
using System.Drawing;
using System.Threading;

namespace interactive_audio_console
{
    internal class Program
    {
        private const int WIDTH = 360 * 2;
        private const int HEIGHT = 100 * 2;

        public static ImageForm Form;
        private static void Main(string[] args)
        {
            new Thread(() =>
            {
                Form = new ImageForm
                {
                    StartPosition = System.Windows.Forms.FormStartPosition.Manual,
                    Location = new Point(1920 - 100 - WIDTH, 1080 - 100 - HEIGHT),
                    TopMost = true,
                    Width = WIDTH,
                    Height = HEIGHT
                };
                //Form.Paint += (s, e) =>
                //{
                //    using (var gfx = Form.GetGraphics())
                //    {
                //        using (var brush = new SolidBrush(Color.Red))
                //        {
                //            gfx.FillRectangle(brush, 0, 0, 400, 100);
                //        }
                //        using (var brush = new SolidBrush(Color.Blue))
                //        {
                //            gfx.FillRectangle(brush, 10, 10, 380, 80);
                //        }
                //    }
                //};
                Form.ShowDialog();
            }).Start();

            new Thread(() =>
               {
                   // wait 1 seconds before starting audio to make sure that the form is fully loaded
                   Thread.Sleep(1000);

                   var audio = new AudioDrawing(Form, Renderer.FFT)
                   {
                   };
                   audio.Start();
               }).Start();
            Console.ReadKey();
        }
    }
}
