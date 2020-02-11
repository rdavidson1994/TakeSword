namespace TakeSword
{
    public interface IActor
    {
        void Act();
        void AttemptAction(IAction<IActor> action);
    }
}