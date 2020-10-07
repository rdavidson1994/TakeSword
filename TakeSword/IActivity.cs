using SmartAnalyzers.CSharpExtensions.Annotations;

namespace TakeSword
{
    public interface IActivity<TActor>
    {
        TActor Actor { get; set; }
        IRoutine<TActor> AsRoutine();
        ActionOutcome IsValid();
    }

    public interface ISimpleActivity<TActor> : IActivity<TActor>
    {
        // Marker interface - indicating that no targets are required for valid new() usage
    }
}