using LedStrip;
using LedStripGui;
using System;
using System.Text;
using System.Threading;

namespace SerialDebug
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            new Thread(() => new LEDStripForm().ShowDialog()).Start();

            Serial.onData += onData;
            Console.ReadLine();
        }

        private static void onData(byte[] data)
        {
            var text = Encoding.ASCII.GetString(data);
            Console.WriteLine(DateTime.Now.TimeOfDay);
            Console.WriteLine(text);
            Console.WriteLine();
        }
    }
}
