using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace interactive_audio
{
    public class AudioDebuging : IDisposable
    {
        public bool StopAfterTime { get; set; } = false;
        public int RecordingSeconds { get; set; } = 20;
        public bool WriteToFile { get; set; } = false;
        public bool LogVolume { get; set; } = false;
        public bool DrawAudio { get; set; } = false;

        private string outputFolder;
        private string outputFilePath;
        private WasapiLoopbackCapture capture;
        private WaveFileWriter writer;
        private ImageForm form;
        private DateTime then;
        public WaveformRenderer renderer;
        private bool disposed = false;


        public AudioDebuging() { }
        public AudioDebuging(ImageForm form) : this()
        {
            this.form = form;
            this.DrawAudio = true;
        }

        public void Start()
        {
            this.outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "NAudio");
            this.outputFilePath = Path.Combine(this.outputFolder, "recorded.txt");
            if (this.WriteToFile)
            {
                Directory.CreateDirectory(this.outputFolder);
            }
            this.capture = new WasapiLoopbackCapture();
            if (this.WriteToFile)
            {
                this.writer = new WaveFileWriter(this.outputFilePath, this.capture.WaveFormat);
            }
            if (this.DrawAudio)
            {
                this.renderer = new WaveformRenderer(this.form);
            }
            this.then = DateTime.Now;
            this.capture.DataAvailable += this.capture_DataAvailable;

            this.capture.RecordingStopped += this.capture_RecordingStopped;

            this.capture.StartRecording();

            if (this.WriteToFile)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (this.capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("_____{0}_____", sw.Elapsed.Seconds);
                }
                sw.Stop();
            }
        }

        private void capture_DataAvailable(object s, WaveInEventArgs a)
        {
            var now = DateTime.Now;
            var diff = now - this.then;
            this.then = now;
            Console.WriteLine("Time {0:0} --- Samples {1}", diff.TotalMilliseconds, a.BytesRecorded / 4);
            if (this.WriteToFile)
            {
                this.writer.Write(a.Buffer, 0, a.BytesRecorded);
            }
            if (this.LogVolume)
            {
                this.checkMaxVolume(a);
            }
            if (this.DrawAudio)
            {
                this.renderer.OnData(a);
            }
            this.checkStop();
        }
        private void capture_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (this.WriteToFile)
            {
                this.writer.Dispose();
                this.writer = null;
            }
            this.capture.Dispose();
        }


        private void checkStop()
        {
            if (this.StopAfterTime)
            {
                if (this.writer.Position > this.capture.WaveFormat.AverageBytesPerSecond * this.RecordingSeconds)
                {
                    this.capture.StopRecording();
                }
            }
        }
        private void checkMaxVolume(WaveInEventArgs a)
        {
            float max = 0;
            var buffer = new WaveBuffer(a.Buffer);
            // interpret as 32 bit floating point audio
            for (int index = 0; index < a.BytesRecorded / 4; index++)
            {
                var sample = buffer.FloatBuffer[index];

                // absolute value 
                if (sample < 0)
                {
                    sample = -sample;
                }
                // is this the max value?
                if (sample > max)
                {
                    max = sample;
                }
            }
            Console.WriteLine(max);
        }



        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            if (this.renderer != null)
            {
                this.renderer.Dispose();
            }

            this.disposed = true;
        }
    }
}
