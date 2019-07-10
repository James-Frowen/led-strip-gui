namespace interactive_audio
{
    public class WeightedAverage
    {
        private float[] sums;
        private int[] counts;

        public WeightedAverage(int count)
        {
            this.sums = new float[count];
            this.counts = new int[count];
        }

        public void Add(float index, float value)
        {
            this.Add((int)index, value);
        }
        public void Add(int index, float value)
        {
            this.sums[index] += value;
            this.counts[index]++;
        }
        public float[] GetAverges()
        {
            float[] averages = new float[this.counts.Length];
            for (int i = 0; i < this.counts.Length; i++)
            {
                averages[i] = this.counts[i] == 0 ? 0 : this.sums[i] / this.counts[i];
            }
            return averages;
        }
    }
}
