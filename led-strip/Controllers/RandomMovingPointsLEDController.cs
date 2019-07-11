using System.Collections;

namespace LedStrip.Controllers
{
    public class RandomMovingPointsLEDController : ThreadLEDController
    {
        // a few Random points than move along the string, with lerp between them

        public RandomMovingPointsLEDController(ILedMessageSender messageSender, int updatesPerSecond) : base(messageSender, updatesPerSecond)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator loop()
        {
            throw new System.NotImplementedException();
        }
    }
}
