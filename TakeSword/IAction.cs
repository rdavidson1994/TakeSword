namespace TakeSword
{
    public interface ISimpleAction<TActor> : ISimpleActivity<TActor>
    {
        // Marker interface
    }
    public interface IAction<TActor> : IActivity<TActor>
    {
        long OnsetTime { get; } //ms
        long CooldownTime { get; } //ms
        ActionOutcome Attempt();
    }
}