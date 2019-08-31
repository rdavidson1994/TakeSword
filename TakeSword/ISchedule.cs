namespace TakeSword
{
    public interface ISchedule
    {
        void Add(IEvent event1, long delay);
        bool RunFor(long deltaTime);
        bool RunOnce();
    }
}