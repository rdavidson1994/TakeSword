namespace TakeSword
{
    public interface ISchedule
    {
        void Add(IEvent event1, long delay);
        void RunFor(long deltaTime);
        void RunOnce();
    }
}