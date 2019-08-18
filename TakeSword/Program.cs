using System;

namespace TakeSword
{
    public interface IEvent
    {
        void Happen();
    }

    public class MessageEvent : IEvent
    {
        private readonly string message;
        public bool Instant { get; set; }
        public MessageEvent(string message)
        {
            this.message = message;
        }
        public void Happen()
        {
            Console.WriteLine(message);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Schedule schedule = new Schedule();
            schedule.Add(new MessageEvent("world"), 200);
            schedule.Add(new MessageEvent("!"), 300);
            schedule.Add(new MessageEvent("hello"), 100);
            schedule.RunOnce();
            schedule.RunOnce();
        }
    }
}
