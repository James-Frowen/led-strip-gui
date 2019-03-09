using System.Collections;
using System.Threading;

namespace LedStripGui
{
    public abstract class ThreadLEDController : LEDController
    {
        private Thread activeThread;
        private bool started;
        private bool stopThread = false;

        public ThreadLEDController(Settings settings) : base(settings)
        {
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
            var sleepTime = 1000 / this.settings.UpdatesPerSecond;
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
