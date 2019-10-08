namespace TakeSword
{
    public interface IActivity
    {
        IActor GetActor();

        IRoutine AsRoutine();
        ActionOutcome IsValid();
    }
}