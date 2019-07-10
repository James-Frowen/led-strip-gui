using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace interactive_audio
{
    public class FFTRenderer : BaseRenderer
    {
        private const int FFT_LENGTH = 8192;
        private readonly SolidBrush brush;
        private readonly Pen pen;
        private readonly SampleAggregator sampleAggregator;
        private RunningAverage runningAverage;

        public int runningAverageCount = 10;

        public FFTRenderer(ImageForm form) : base(form)
        {
            this.brush = new SolidBrush(Color.Green);
            this.pen = new Pen(this.brush);

            this.sampleAggregator = new SampleAggregator(FFT_LENGTH)
            {
                PerformFFT = true
            };
            this.sampleAggregator.FftCalculated += this.fftCalculated;
            this.runningAverage = new RunningAverage(this.runningAverageCount);
        }

        public override void OnData(WaveInEventArgs a)
        {
            this.sampleAggregator.Add(a.Buffer);
        }
        private void fftCalculated(object sender, FftEventArgs e)
        {
            bool useLog = false;

            float[] freqs = calculateFrequencies(e, useLog);

            float[] averages = this.calculateLogAverages(freqs);
            //float[] blands = this.calculateBands(freqs);

            float scale = this.getScale(averages);
            this.draw(averages, DrawMode.Line, scale);
        }

        private float getScale(float[] averages)
        {
            var max = averages.Max();
            this.runningAverage.AddNext(max);
            var avgMax = this.runningAverage.GetAverage();
            avgMax = Math.Max(avgMax, 0.000000001f); // small non-zero value
            var scale = this.height / avgMax;
            return scale;
        }

        private static float[] calculateFrequencies(FftEventArgs e, bool useLog)
        {
            var fftPoints = e.Result.Length;
            var freqsPoints = fftPoints / 2;
            var freqs = new float[freqsPoints];
            for (int i = 0; i < freqsPoints; i++)
            {
                var fftLeft = Math.Abs(e.Result[i].X + e.Result[i].Y);
                var fftRight = Math.Abs(e.Result[fftPoints - 1 - i].X + e.Result[fftPoints - 1].Y);

                freqs[i] = fftLeft + fftRight;
            }

            return freqs;
        }

        private float[] calculateAverages(float[] freqs)
        {
            var width = this.width / this.pixelsPerLine;
            var averages = new float[width];

            int samples = freqs.Length;
            int samplesPerPixel = samples / width;

            for (int x = 0; x < width; x++)
            {
                float sum = 0;
                for (int s = 0; s < samplesPerPixel; s++)
                {
                    var sample = freqs[x * samplesPerPixel + s];
                    sum += sample;
                    if (sum > float.MaxValue * 0.8)
                    {
                        Debug.Fail("Too Close to max");
                    }
                }
                averages[x] = sum / samplesPerPixel;
            }

            return averages;
        }

        private float[] calculateLogAverages(float[] freqs)
        {
            var realWidth = this.width / this.pixelsPerLine;
            const int shift = 0;
            var width = realWidth + shift;// width used to ignore low frequences

            int samples = freqs.Length;
            var averages = new WeightedAverage(realWidth);

            float logRatio = width / (float)Math.Log(samples);
            for (int i = 1; i < samples; i++)
            {
                float freq = i;
                float amp = freqs[i];

                float logFreq = (float)Math.Log(i);

                int newIndex = (int)(logFreq * logRatio) - shift;
                if (newIndex >= 0)
                {
                    averages.Add(newIndex, amp);
                }
            }

            return averages.GetAverges();
        }
        private float[] calculateBands(float[] freqs)
        {
            /**
             * Source https://youtu.be/mHk3ZiKNH48
            7 hz Bands 
            20-60 
            60-250 
            250-500 
            500-2k
            2k-4k
            4k-6k
            6k-20k

            4000 samples between 20hz - 20khz
            ~ 1 samples every 5hz

            use https://www.szynalski.com/tone-generator/ 
            for testing
             */

            const int bandCount = 7;
            int samples = freqs.Length;
            var averages = new WeightedAverage(bandCount);

            float ratio = 20000f / samples;
            for (int i = 1; i < samples; i++)
            {
                float freq = i * ratio;
                float amp = freqs[i];
                if (freq < 60f)
                {
                    averages.Add(0, amp);
                }
                else if (freq < 250f)
                {
                    averages.Add(1, amp);
                }
                else if (freq < 500)
                {
                    averages.Add(2, amp);
                }
                else if (freq < 2000)
                {
                    averages.Add(3, amp);
                }
                else if (freq < 4000)
                {
                    averages.Add(4, amp);
                }
                else if (freq < 6000)
                {
                    averages.Add(5, amp);
                }
                else
                {
                    averages.Add(6, amp);
                }
            }

            return averages.GetAverges();
        }


        private void draw(float[] averages, DrawMode drawMode, float scale)
        {
            this.gfx.Clear(Color.FromArgb(100, Color.Black));

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
                float y = 20f;
                float height = value;
                this.gfx.FillRectangle(this.brush, x * this.pixelsPerLine, y, this.pixelsPerLine, height);
            }
        }
        private void drawPoint(float[] averages, float scale)
        {
            for (int x = 0; x < averages.Length; x++)
            {
                var value = (averages[x] * scale);
                var amp = value * this.height;
                float y = this.height / 2 + this.pixelsPerLine / 2 + amp;
                float height = this.pixelsPerLine;
                this.gfx.FillRectangle(this.brush, x * this.pixelsPerLine, y, this.pixelsPerLine, height);
            }
        }
        private void drawLine(float[] averages, float scale)
        {
            for (int x = 0; x < averages.Length - 1; x++)
            {
                float y1 = (averages[x] * scale);
                float y2 = (averages[x + 1] * scale);

                float x1 = x * this.pixelsPerLine;
                float x2 = (x + 1) * this.pixelsPerLine;


                y1 = y1.Clamp(0, this.height);
                y2 = y2.Clamp(0, this.height);
                x1 = x1.Clamp(0, this.width);
                x2 = x2.Clamp(0, this.width);


                this.gfx.DrawLine(this.pen, x1, y1, x2, y2);
            }
        }


        protected override void _dispose()
        {
            this.brush.Dispose();
            this.pen.Dispose();
        }
    }
    public static class FloatExtensions
    {
        public static float Clamp(this float v, float min, float max)
        {
            if (v > max)
            {
                return max;
            }
            if (v < min)
            {
                return min;
            }
            return v;
        }
        public static float Clamp(this int v, float min, float max)
        {
            if (v > max)
            {
                return max;
            }
            if (v < min)
            {
                return min;
            }
            return v;
        }
    }
    public enum DrawMode
    {
        Solid,
        Points,
        Line
    }
}
