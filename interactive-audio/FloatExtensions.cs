namespace interactive_audio
{
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
}
