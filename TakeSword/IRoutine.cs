namespace TakeSword
{
    public interface IRoutine : IActivity
    {
        IAction Peek();
        IAction NextAction();
        void ReactToAnnouncement(ActionAnnouncement announcement);
    }
}