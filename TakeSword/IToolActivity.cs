namespace TakeSword
{
    public interface IToolActivity : IActivity<PhysicalActor>
    {
        GameObject Target { get; set; }
        GameObject Tool { get; set; }
    }
}