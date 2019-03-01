using led_strip_gui;
using System;
using System.Threading;

namespace SerialDebug
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            new Thread(() => new Form1().ShowDialog()).Start();

            Serial.onData += onData;
            Console.ReadLine();
        }

        private static void onData(byte[] data)
        {
            foreach (var item in data)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();

            //int b;

            //if (int.TryParse(data, out b))
            //{
            //    char c = (char)b;
            //    Console.Write(b);
            //}
            //else
            //{
            //    Console.Write(data);
            //}
        }
    }
}
