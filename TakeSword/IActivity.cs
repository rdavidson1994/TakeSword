namespace TakeSword
{
    public interface IActivity<TActor>
    {
        TActor Actor { get; set; }

        IRoutine<TActor> AsRoutine();
        ActionOutcome IsValid();
    }
}