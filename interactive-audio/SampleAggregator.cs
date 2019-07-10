using NAudio.Dsp;
using NAudio.Wave;
using System;

namespace interactive_audio
{
    /// <summary>
    /// Original Source https://stackoverflow.com/a/20414331/8479976
    /// Also found here https://github.com/naudio/NAudio/blob/e359ca0566e9f9b14fee1ba6e0ec17e4482c7844/NAudioWpfDemo/AudioPlaybackDemo/SampleAggregator.cs
    /// </summary>
    public class SampleAggregator
    {
        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;
        public bool PerformFFT { get; set; }

        // This Complex is NAudio's own! 
        private Complex[] fftBuffer;
        private FftEventArgs fftArgs;
        private int fftPos;
        private int fftLength;
        private int m;

        public SampleAggregator(int fftLength)
        {
            if (!this.IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            this.m = (int)Math.Log(fftLength, 2.0);
            this.fftLength = fftLength;
            this.fftBuffer = new Complex[fftLength];
            this.fftArgs = new FftEventArgs(this.fftBuffer);
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Add(float[] values)
        {
            if (this.PerformFFT && FftCalculated != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    this.addToBuffer(values[i]);
                }
            }
        }
        public void Add(byte[] byteBuffer)
        {
            var buffer = new WaveBuffer(byteBuffer);

            if (this.PerformFFT && FftCalculated != null)
            {
                for (int i = 0; i < byteBuffer.Length / 4; i++)
                {
                    this.addToBuffer(buffer.FloatBuffer[i]);
                }
            }
        }


        private void addToBuffer(float value)
        {
            // Remember the window function! There are many others as well.
            this.fftBuffer[this.fftPos].X = (float)(value * FastFourierTransform.HammingWindow(this.fftPos, this.fftLength));
            this.fftBuffer[this.fftPos].Y = 0; // This is always zero with audio.
            this.fftPos++;
            if (this.fftPos >= this.fftLength)
            {
                this.fftPos = 0;
                FastFourierTransform.FFT(true, this.m, this.fftBuffer);
                FftCalculated(this, this.fftArgs);
            }
        }
    }
}
