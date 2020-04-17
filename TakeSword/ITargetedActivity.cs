namespace TakeSword
{
    public interface ITargetedActivity<TActor> : IActivity<TActor>
    {
        GameObject Target { get; set; }
    }

    public interface IDirectionActivity : IActivity<PhysicalActor>
    {
        Direction Direction { get; set; }
    }
}