using LedStrip.Forms;
using NAudio.Wave;
using System;
using System.Drawing;

namespace interactive_audio
{
    public abstract class BaseRenderer : IDisposable, IAudioRenderer
    {
        private const int NUMBER_OF_LEDS = 120;
        protected readonly int formWidth;
        protected readonly int formHeight;
        private readonly FormRenderer renderer;
        public int pixelsPerLine;

        protected bool disposed = false;

        protected Graphics GFX => this.renderer.GFX;

        public BaseRenderer(ImageForm form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            this.formWidth = form.Width;
            this.formHeight = form.Height;
            this.pixelsPerLine = this.formWidth / NUMBER_OF_LEDS;
            this.renderer = new FormRenderer(form, this.formWidth, this.formHeight);
        }
        protected void finishDraw()
        {
            this.renderer.FinishDraw();
        }

        public abstract void OnData(WaveInEventArgs a);

        public void Dispose()
        {
            if (this.disposed) { return; }

            this.renderer.Dispose();
            this._dispose();

            this.disposed = true;
        }
        protected abstract void _dispose();
    }
}
