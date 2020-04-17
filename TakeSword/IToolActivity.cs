namespace TakeSword
{
    public interface IToolActivity<TActor> : IActivity<TActor>
    {
        GameObject Target { get; set; }
        GameObject Tool { get; set; }
    }
}