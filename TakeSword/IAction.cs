namespace TakeSword
{
    public interface IAction : IActivity
    {
        long OnsetTime { get; } //ms
        long CooldownTime { get; } //ms
        ActionOutcome Attempt();
    }
}