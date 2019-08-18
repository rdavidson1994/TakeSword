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

        public void RunOnce()
        {
            var tuple = events.First.Value;
            IEvent nextEvent = tuple.Item1;
            long nextTime = tuple.Item2;
            nextEvent.Happen();
            events.RemoveFirst(); // remove nextEvent
            currentTime = nextTime;
        }

        public void RunFor(long deltaTime)
        {
            long endTime = currentTime + deltaTime;
            bool running = true;
            while (running)
            {
                var (nextEvent, nextTime) = events.First?.Value;
                if (nextEvent != null && nextTime <= endTime)
                {
                    nextEvent.Happen();
                    events.RemoveFirst();
                }
                else
                {
                    currentTime = endTime;
                    running = false;
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
