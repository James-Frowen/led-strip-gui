using NAudio.Wave;
using System;

namespace interactive_audio
{
    public enum Renderer
    {
        FFT,
        WaveForm
    }
    public class AudioDrawing : IDisposable
    {
        private readonly ImageForm form;
        private readonly Renderer rendererType;

        private WasapiLoopbackCapture capture;
        private DateTime then;
        public IAudioRenderer renderer;
        private bool disposed = false;


        public AudioDrawing(ImageForm form, Renderer rendererType)
        {
            this.form = form;
            this.rendererType = rendererType;
        }

        public void Start()
        {
            this.renderer = this.createRenderer(this.rendererType);


            this.then = DateTime.Now;

            this.capture = new WasapiLoopbackCapture();
            this.capture.DataAvailable += this.capture_DataAvailable;
            this.capture.RecordingStopped += this.capture_RecordingStopped;

            this.capture.StartRecording();
        }

        private IAudioRenderer createRenderer(Renderer rendererType)
        {
            switch (rendererType)
            {
                case Renderer.FFT:
                    return new FFTRenderer(this.form);
                case Renderer.WaveForm:
                    return new WaveformRenderer(this.form);
                default:
                    throw new System.NotImplementedException("Enum value not Implemented");
            }
        }

        private void capture_DataAvailable(object s, WaveInEventArgs a)
        {
            this.logTime(a);
            this.renderer.OnData(a);
        }

        private void logTime(WaveInEventArgs a)
        {
            var now = DateTime.Now;
            var diff = now - this.then;
            this.then = now;
            Console.WriteLine("Time {0:0} --- Samples {1}", diff.TotalMilliseconds, a.BytesRecorded / 4);
        }

        private void capture_RecordingStopped(object sender, StoppedEventArgs e)
        {
            this.capture.Dispose();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            if (this.capture.CaptureState == NAudio.CoreAudioApi.CaptureState.Capturing)
            {
                this.capture.StopRecording();
            }
            if (this.renderer != null)
            {
                this.renderer.Dispose();
            }
        }
    }
}
