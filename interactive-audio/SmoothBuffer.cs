using System.Linq;

namespace interactive_audio
{
    public class SmoothBuffer
    {
        private float[] buffer;
        private readonly float smoothness;
        private readonly bool increaseInstanty;

        public SmoothBuffer(int bandCount, float smoothness, bool increaseInstanty)
        {
            this.buffer = new float[bandCount];
            this.smoothness = smoothness;
            this.increaseInstanty = increaseInstanty;
        }

        public float[] GetBuffer()
        {
            return this.buffer.ToArray();
        }
        public void AddToBuffer(float[] newValues)
        {
            this.ApplyBuffer(newValues);
        }
        public float[] ApplyBuffer(float[] newValues)
        {
            for (int i = 0; i < this.buffer.Length; i++)
            {
                if (this.increaseInstanty && this.buffer[i] < newValues[i])
                {
                    this.buffer[i] = newValues[i];
                }
                else
                {
                    this.buffer[i] = this.lerp(this.buffer[i], newValues[i], this.smoothness);
                }
            }
            // return copy of Array
            return this.buffer.ToArray();
        }

        private float lerp(float A, float B, float V)
        {
            return A * (1 - V) + B * V;
        }
    }
}
