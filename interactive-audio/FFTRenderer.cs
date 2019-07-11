using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;

namespace interactive_audio
{
    public class FFTRenderer : BaseRenderer
    {
        private const int FFT_LENGTH = 8192;
        private readonly SolidBrush brush;
        private readonly SolidBrush bandBrush;
        private readonly Pen pen;
        private readonly SampleAggregator sampleAggregator;
        private RunningAverage runningAverage;
        private SmoothBuffer lineBuffer;
        private SmoothBuffer bandBuffer;

        public int runningAverageCount = 10;

        public FFTRenderer(ImageForm form) : base(form)
        {
            this.brush = new SolidBrush(Color.Green);
            this.bandBrush = new SolidBrush(Color.Red);
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
            float[] bands = this.calculateBands(freqs);

            const float smoothness = 0.5f;
            if (this.lineBuffer == null)
            {
                this.lineBuffer = new SmoothBuffer(averages.Length, smoothness, true);
            }
            if (this.bandBuffer == null)
            {
                this.bandBuffer = new SmoothBuffer(bands.Length, smoothness, true);
            }

            var bufferedAverage = this.lineBuffer.ApplyBuffer(averages);
            var bufferedBands = this.bandBuffer.ApplyBuffer(bands);


            float scale = this.getScale(averages);
            Console.WriteLine(scale);
            this.GFX.Clear(Color.FromArgb(20, Color.Black));
            this.drawBands(bufferedBands, scale);
            this.draw(bufferedAverage, DrawMode.Line, scale);
            this.finishDraw();
        }


        private float getScale(float[] averages)
        {
            //var max = averages.Max();
            //this.runningAverage.AddNext(max);
            //var avgMax = this.runningAverage.GetAverage();
            //avgMax = Math.Max(avgMax, 0.000000001f); // small non-zero value
            //var scale = this.formHeight / avgMax;
            const float maxAverageMax = 20f + 10f; // value that seems to be the normal high value + a bit
            return this.formHeight * maxAverageMax;
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

        [System.Obsolete("use Log averages instead")]
        private float[] calculateAverages(float[] freqs)
        {
            var width = this.formWidth / this.pixelsPerLine;
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
            var realWidth = this.formWidth / this.pixelsPerLine;
            const int shift = 0;
            var width = realWidth + shift;// width used to ignore low frequences

            int samples = freqs.Length;
            var averages = new WeightedAverage(realWidth);

            float logRatio = width / (float)Math.Log(samples, 2);
            for (int i = 1; i < samples; i++)
            {
                float freq = i;
                float amp = freqs[i];

                float logFreq = (float)Math.Log(i, 2);

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
            6k-11k
            11k-20k

            4000 samples between 20hz - 20khz
            ~ 1 samples every 5hz

            use https://www.szynalski.com/tone-generator/ 
            for testing



            Bands From HeadPhone software
            32
            64
            128
            250
            500
            1000
            2000
            4000
            8000
            16000

            using above as mid points

            0= 0-48hz
            1= 48-96hz
            2= 96-..
            3= 192
            4= 384
            5= 768
            6= 1536
            7= 3072
            8= 6144
            9= 12288
            
            can use powers of 2 times 1.5
            2^n * 1.5
            eg 2^5 * 1.5 = 48
             */


            const int bandCount = 10;
            int samples = freqs.Length;
            var averages = new WeightedAverage(bandCount)
            {
                squareValues = true
            };

            const int startPower = 5;
            var bandThresholds = new float[bandCount];
            for (int n = 0; n < bandCount; n++)
            {
                var max = (float)Math.Pow(2, n + startPower) * 1.5f;
                bandThresholds[n] = max;
            }

            float ratio = 20000f / samples;
            for (int i = 1; i < samples; i++)
            {
                float freq = i * ratio;
                float amp = freqs[i];
                findBand(averages, bandThresholds, freq, amp);
            }

            return averages.GetAverges();
        }


        private static void findBand(WeightedAverage averages, float[] bandThresholds, float freq, float amp)
        {
            for (int n = 0; n < averages.Length; n++)
            {
                if (freq < bandThresholds[n])
                {
                    averages.Add(n, amp);
                    return;
                }
            }

            throw new Exception("Should have returned in for loop");
        }

        private void draw(float[] averages, DrawMode drawMode, float scale)
        {
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
                this.GFX.FillRectangle(this.brush, x * this.pixelsPerLine, y, this.pixelsPerLine, height);
            }
        }
        private void drawPoint(float[] averages, float scale)
        {
            for (int x = 0; x < averages.Length; x++)
            {
                var value = (averages[x] * scale);
                var amp = value * this.formHeight;
                float y = this.formHeight / 2 + this.pixelsPerLine / 2 + amp;
                float height = this.pixelsPerLine;
                this.GFX.FillRectangle(this.brush, x * this.pixelsPerLine, y, this.pixelsPerLine, height);
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


                y1 = y1.Clamp(0, this.formHeight);
                y2 = y2.Clamp(0, this.formHeight);
                x1 = x1.Clamp(0, this.formWidth);
                x2 = x2.Clamp(0, this.formWidth);


                this.GFX.DrawLine(this.pen, x1, y1, x2, y2);
            }
        }

        private void drawBands(float[] bands, float scale)
        {
            const int padding = 2;

            var pixelsPerBand = (this.formWidth / bands.Length);
            var bandWidth = pixelsPerBand - padding;
            var bandOffset = padding / 2;

            for (int x = 0; x < bands.Length; x++)
            {
                var value = bands[x];
                var y = 0;
                float height = Math.Min(value * scale, this.formHeight);
                this.GFX.FillRectangle(this.bandBrush, x * pixelsPerBand + bandOffset, y, bandWidth, height);
            }
        }

        protected override void _dispose()
        {
            this.brush.Dispose();
            this.bandBrush.Dispose();
            this.pen.Dispose();
        }
    }
}
