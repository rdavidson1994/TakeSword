namespace TakeSword
{
    // Usually, you'll implement this as class Dude : IActor<Dude>
    // This signals that Dudes can do actions that require Actors of type Dude.
    public interface IActor<T>
    {
        void Act();
        void AttemptAction(IAction<T> action);
    }
}