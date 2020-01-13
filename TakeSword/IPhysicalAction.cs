namespace TakeSword
{
    public interface IPhysicalActivity : IActivity
    {
        PhysicalActor Actor { get; set; }
    }
}