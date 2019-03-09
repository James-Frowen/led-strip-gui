namespace LedStripGui
{
    public abstract class LEDController
    {
        protected readonly int count = ArduinoCodes.LED_COUNT;
        protected readonly Settings settings;

        public LEDController(Settings settings)
        {
            this.settings = settings;
            this.count = settings.ledCount;
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
