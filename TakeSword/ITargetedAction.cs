namespace TakeSword
{
    public interface ITargetedActivity : IPhysicalActivity
    {
        GameObject Target { get; set; }
    }
}