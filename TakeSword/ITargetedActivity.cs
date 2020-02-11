namespace TakeSword
{
    public interface ITargetedActivity : IActivity<PhysicalActor>
    {
        GameObject Target { get; set; }
    }

    public interface IDirectionActivity : IActivity<PhysicalActor>
    {
        Direction Direction { get; set; }
    }
}