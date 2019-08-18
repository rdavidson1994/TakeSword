using Moq;
using NUnit.Framework;
using TakeSword;

namespace Tests
{
    [TestFixture]
    public class ScheduleTests
    {
        private IEvent RequiredEvent()
        {
            var event1 = new Mock<IEvent>(MockBehavior.Strict);
            event1.Setup(x => x.Happen());
            return event1.Object;
        }

        private IEvent ForbiddenEvent()
        {
            return new Mock<IEvent>(MockBehavior.Strict).Object;
        }

        private Schedule schedule;
        [SetUp]
        public void Setup()
        {
            schedule = new Schedule();
        }

        [Test]
        public void RunForTest()
        {
            /*
            var event1 = new Mock<Event>(MockBehavior.Strict);
            event1.Setup(x => x.Happen());
            var event2 = new Mock<Event>(MockBehavior.Strict);
            event2.Setup(x => x.Happen());
            var event3 = new Mock<Event>(MockBehavior.Strict);
            */

            schedule.Add(ForbiddenEvent(), 300);
            schedule.Add(RequiredEvent(), 100);
            schedule.Add(RequiredEvent(), 200);

            schedule.RunFor(250);
        }

        [Test]
        public void RunOnceTest()
        {
            schedule.Add(RequiredEvent(), 100);
            schedule.Add(ForbiddenEvent(), 200);
            schedule.RunOnce();
        }
    }
}