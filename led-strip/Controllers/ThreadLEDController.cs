using System.Collections;
using System.Threading;

namespace LedStrip.Controllers
{
    public abstract class ThreadLEDController : LEDController
    {
        public const int DEFAULT_UPDATES_PER_SECOND = 10;
        private Thread activeThread;
        private bool started;
        private bool stopThread = false;

        public int UpdatesPerSecond { get; set; }

        public ThreadLEDController(ILedMessageSender messageSender, int updatesPerSecond) : base(messageSender)
        {
            this.UpdatesPerSecond = updatesPerSecond;
        }

        private void thread()
        {
            while (true)
            {
                var l = this.loop();

                while (l.MoveNext())
                {
                    this.sleep();
                    if (this.stopThread)
                    {
                        return;
                    }
                }
            }
        }
        protected void sleep()
        {
            var sleepTime = 1000 / this.UpdatesPerSecond;
            Thread.Sleep(sleepTime);
        }

        protected abstract IEnumerator loop();

        public sealed override void Start()
        {
            if (this.started) { return; }

            this.activeThread = new Thread(this.thread);
            this.activeThread.Start();
            this.started = true;
        }

        public sealed override void Stop()
        {
            this.stopThread = true;
        }
    }
}
