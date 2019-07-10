using System;
using System.Linq;

namespace interactive_audio
{
    public class RunningAverage
    {
        private float[] values;
        private int index;
        public RunningAverage(int count)
        {
            if (count == 0) { throw new ArgumentException(nameof(count) + " should not be 0"); }
            this.values = new float[count];
        }

        public void AddNext(float value)
        {
            this.values[this.index] = value;
            this.index++;
            if (this.index >= this.values.Length)
            {
                this.index = 0;
            }
        }
        public float GetAverage()
        {
            return this.values.Average();
        }
    }
}
