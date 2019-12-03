namespace TakeSword
{
    public interface IRoutine : IActivity
    {
        IAction NextAction();
        void ReactToAnnouncement(object announcement);
    }
}