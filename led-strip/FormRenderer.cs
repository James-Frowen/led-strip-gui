using System;
using System.Drawing;
using System.Windows.Forms;

namespace LedStrip.Forms
{
    public class FormRenderer : IDisposable
    {
        protected readonly Graphics gfx;
        private readonly Graphics formGfx;
        private readonly Image drawBuffer;

        public Graphics GFX => this.gfx;

        protected bool disposed = false;

        public FormRenderer(Form form, int formWidth, int formHeight)
        {
            this.drawBuffer = new Bitmap(formWidth, formHeight);
            this.gfx = Graphics.FromImage(this.drawBuffer);
            this.formGfx = form.CreateGraphics();
        }

        public void FinishDraw()
        {
            this.formGfx.DrawImage(this.drawBuffer, new Point(0, 0));
        }
        public void Dispose()
        {
            if (this.disposed) { return; }

            this.gfx.Dispose();
            this.drawBuffer.Dispose();
            this.formGfx.Dispose();
            this.disposed = true;
        }
    }
}

