using NAudio.Wave;
using System;
using System.Drawing;

namespace interactive_audio
{
    public abstract class BaseRenderer : IDisposable, IAudioRenderer
    {
        private const int NUMBER_OF_LEDS = 120;
        protected readonly int width;
        protected readonly int height;
        public int pixelsPerLine;
        protected readonly Graphics gfx;
        protected bool disposed = false;


        public BaseRenderer(ImageForm form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            this.width = form.Width;
            this.height = form.Height;
            this.pixelsPerLine = this.width / NUMBER_OF_LEDS;
            this.gfx = form.CreateGraphics();
        }

        public abstract void OnData(WaveInEventArgs a);

        public void Dispose()
        {
            if (this.disposed) { return; }

            this.gfx.Dispose();
            this._dispose();
        }
        protected abstract void _dispose();
    }
}
