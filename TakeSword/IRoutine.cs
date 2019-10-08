namespace TakeSword
{
    public interface IRoutine : IActivity
    {
        IAction NextAction();
    }
}