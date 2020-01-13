namespace TakeSword
{
    public interface IToolActivity : ITargetedActivity
    {
        GameObject Tool { get; set; }
    }
}