using System;
using System.Collections.Generic;

namespace TakeSword
{
    public class Schedule : ISchedule
    {
        private long currentTime;
        private LinkedList<Tuple<IEvent, long>> events;

        public Schedule()
        {
            currentTime = 0;
            events = new LinkedList<Tuple<IEvent, long>>();
        }

        public bool RunOnce()
        {
            if (events.Count == 0)
            {
                return false;
            }
            var tuple = events.First.Value;
            IEvent nextEvent = tuple.Item1;
            currentTime = tuple.Item2;
            nextEvent.Happen();
            events.RemoveFirst(); // remove nextEvent
            return true;
        }

        public bool RunFor(long deltaTime)
        {
            long endTime = currentTime + deltaTime;
            while (true)
            {
                if (events.Count == 0)
                {
                    return currentTime == endTime;
                }
                var (nextEvent, nextTime) = events.First.Value;
                if (nextTime <= endTime)
                {
                    currentTime = nextTime;
                    nextEvent.Happen();
                    events.RemoveFirst();
                }
                else
                {
                    currentTime = endTime;
                    return true;
                }
            }
        }

        public void Add(IEvent event1, long delay)
        {
            bool last = true;
            long time = currentTime + delay;
            var entry = Tuple.Create(event1, time);

            LinkedListNode<Tuple<IEvent, long>> node = events.First;
            while (node != null)
            {
                if (time < node.Value.Item2)
                {
                    events.AddBefore(node, entry);
                    last = false;
                    break;
                }
                node = node.Next;
            }

            if (last)
            {
                events.AddLast(entry);
            }
        }
    }
}
