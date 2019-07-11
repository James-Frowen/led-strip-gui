namespace LedStrip.Controllers
{
    public abstract class LEDController
    {
        protected readonly ILedMessageSender messageSender;

        public LEDController(ILedMessageSender messageSender)
        {
            this.messageSender = messageSender;
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
