namespace TakeSword
{
    public interface IAction<TActor> : IActivity<TActor>
    {
        long OnsetTime { get; } //ms
        long CooldownTime { get; } //ms
        ActionOutcome Attempt();
    }
}