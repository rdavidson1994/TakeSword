namespace TakeSword
{
    public interface ITargetedActivity : IPhysicalActivity
    {
        GameObject Target { get; set; }
    }

    public interface IDirectionActivity : IPhysicalActivity
    {
        Direction Direction { get; set; }
    }
}