using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;

namespace interactive_audio
{
    public class WaveformRenderer : BaseRenderer
    {
        public readonly SolidBrush green;
        public readonly SolidBrush yellow;
        public readonly SolidBrush orange;
        public readonly SolidBrush red;
        public readonly Pen pen;

        public WaveformRenderer(ImageForm form) : base(form)
        {
            this.green = new SolidBrush(Color.Green);
            this.yellow = new SolidBrush(Color.Yellow);
            this.orange = new SolidBrush(Color.Orange);
            this.red = new SolidBrush(Color.Red);

            this.pen = new Pen(this.green);
        }

        public override void OnData(WaveInEventArgs a)
        {
            var width = this.formWidth / this.pixelsPerLine;
            int bytesPerSample = 4;
            var samples = a.BytesRecorded / (bytesPerSample);
            int samplesPerPixel = samples / width;

            var buffer = new WaveBuffer(a.Buffer);
            var averages = new float[width];
            for (int x = 0; x < width; x++)
            {
                float sum = 0;
                for (int s = 0; s < samplesPerPixel; s++)
                {
                    var sample = buffer.FloatBuffer[x * samplesPerPixel + s];
                    sum += sample;
                    if (sum > float.MaxValue * 0.8)
                    {
                        Debug.Fail("Too Close to max");
                    }
                }
                averages[x] = sum / samplesPerPixel;
            }

            this.draw(averages, DrawMode.Line, this.formHeight);
        }

        private void draw(float[] averages, DrawMode drawMode, float scale)
        {
            this.GFX.Clear(Color.FromArgb(100, Color.Black));
            switch (drawMode)
            {
                case DrawMode.Solid:
                    this.drawSolid(averages, scale);
                    break;
                case DrawMode.Points:

                    this.drawPoint(averages, scale);
                    break;
                case DrawMode.Line:
                    this.drawLine(averages, scale);
                    break;
                default:
                    throw new System.NotImplementedException("Enum value not Implemented");

            }
        }
        private void drawSolid(float[] averages, float scale)
        {
            for (int x = 0; x < averages.Length; x++)
            {
                var value = (averages[x] * scale);
                var brush = this.getColor(value);

                // wave from 0 to 2
                var amp = value * this.formHeight;
                float y = this.formHeight / 2 + (amp >= 0 ? 0 : amp);
                float height = Math.Abs(amp);
                this.GFX.FillRectangle(brush, x * this.pixelsPerLine, y, this.pixelsPerLine, height);
            }
        }
        private void drawPoint(float[] averages, float scale)
        {
            for (int x = 0; x < averages.Length; x++)
            {
                var value = (averages[x] * scale);
                var brush = this.getColor(value);
                // wave from 0 to 2
                var amp = value * this.formHeight;
                float y = this.formHeight / 2 + this.pixelsPerLine / 2 + amp;
                float height = this.pixelsPerLine;
                this.GFX.FillRectangle(brush, x * this.pixelsPerLine, y, this.pixelsPerLine, height);
            }
        }
        private void drawLine(float[] averages, float scale)
        {
            for (int x = 0; x < averages.Length - 1; x++)
            {
                var y1 = (averages[x] * scale) + this.formHeight / 2;
                var y2 = (averages[x + 1] * scale) + this.formHeight / 2;
                this.GFX.DrawLine(this.pen, x * this.pixelsPerLine, y1, (x + 1) * this.pixelsPerLine, y2);
            }
        }

        private Brush getColor(float value)
        {
            var abs = Math.Abs(value);

            if (abs < 0.1) { return this.green; }
            else if (abs < 0.2) { return this.yellow; }
            else if (abs < 0.35) { return this.orange; }
            else { return this.red; }
        }

        protected override void _dispose()
        {
            this.green.Dispose();
            this.yellow.Dispose();
            this.orange.Dispose();
            this.red.Dispose();
            this.pen.Dispose();
        }
    }
}
