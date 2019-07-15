using interactive_audio;
using System;
using System.Drawing;
using System.Threading;

namespace interactive_audio_console
{
    internal class Program
    {
        private const int WIDTH = 720;
        private const int HEIGHT = 200;

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
                Form.ShowDialog();
            }).Start();

            new Thread(() =>
               {
                   // wait 1 seconds before starting audio to make sure that the form is fully loaded
                   Thread.Sleep(1000);

                   var audio = new AudioDrawing(Form, Renderer.FFT)
                   {
                       ShouldLogTime = false
                   };
                   audio.Start();
               }).Start();
            Console.ReadKey();
        }
    }
}
