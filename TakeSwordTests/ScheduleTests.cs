using NUnit.Framework;
using System.Collections.Generic;
using TakeSword;

namespace TakeSwordTests
{
    [TestFixture]
    public class ScheduleTests
    {
        private class RaceEvent : IEvent
        {
            private List<IEvent> eventList;
            public RaceEvent(List<IEvent> eventList)
            {
                this.eventList = eventList;
            }
            public void Happen()
            {
                eventList.Add(this);
            }
        }
        private class EventSpy : IEvent
        {
            public bool happened = false;
            public void Happen()
            {
                happened = true;
            }
        }

        [Test]
        public void RunOnceTest()
        {
            Schedule schedule = new Schedule();
            EventSpy requiredEvent = new EventSpy();
            EventSpy forbiddenEvent = new EventSpy();
            schedule.Add(requiredEvent, 100);
            schedule.Add(forbiddenEvent, 200);
            schedule.RunOnce();
            Assert.IsTrue(requiredEvent.happened);
            Assert.IsFalse(forbiddenEvent.happened);
        }
        [Test]
        public void TiesFirstInFirstOut()
        {
            Schedule schedule = new Schedule();
            List<IEvent> outcome = new List<IEvent>();
            RaceEvent event0 = new RaceEvent(outcome);
            RaceEvent event1 = new RaceEvent(outcome);
            RaceEvent event2 = new RaceEvent(outcome);
            schedule.Add(event2, 100);
            schedule.Add(event0, 0);
            schedule.Add(event1, 0);
            Assert.IsTrue(schedule.RunFor(100));
            Assert.AreEqual(outcome[0], event0);
            Assert.AreEqual(outcome[1], event1);
            Assert.AreEqual(outcome[2], event2);
        }
        [Test]
        public void RunForTest()
        {
            Schedule schedule = new Schedule();
            EventSpy requiredEvent = new EventSpy();
            EventSpy requiredEvent2 = new EventSpy();
            EventSpy forbiddenEvent = new EventSpy();
            schedule.Add(forbiddenEvent, 300);
            schedule.Add(requiredEvent, 100);
            schedule.Add(requiredEvent2, 200);
            Assert.IsTrue(schedule.RunFor(250));
            Assert.IsTrue(requiredEvent.happened);
            Assert.IsTrue(requiredEvent2.happened);
            Assert.IsFalse(forbiddenEvent.happened);
        }
    }
}