using System;
using System.Linq;

namespace interactive_audio
{
    public class WeightedAverage
    {
        private float[] sums;
        private int[] counts;
        public bool squareValues;

        public int Length { get; }

        public WeightedAverage(int count)
        {
            this.Length = count;
            this.sums = new float[count];
            this.counts = new int[count];
        }

        public void Add(float index, float value)
        {
            this.Add((int)index, value);
        }
        public void Add(int index, float value)
        {
            if (this.squareValues)
            {
                this.sums[index] += (value * value);
            }
            else
            {
                this.sums[index] += value;
            }
            this.counts[index]++;
        }
        public float[] GetAverges()
        {
            float[] averages = new float[this.counts.Length];
            for (int i = 0; i < this.counts.Length; i++)
            {
                averages[i] = this.counts[i] == 0 ? 0 : this.sums[i] / this.counts[i];
            }
            if (this.squareValues)
            {
                return averages.Select(x => (float)Math.Sqrt(x)).ToArray();
            }
            else
            {
                return averages;
            }
        }
    }
}
