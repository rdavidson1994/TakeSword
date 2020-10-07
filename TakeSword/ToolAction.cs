using SmartAnalyzers.CSharpExtensions.Annotations;
using System;
using System.Collections.Generic;
namespace TakeSword
{
    public abstract class ToolAction : PhysicalAction, IToolActivity<PhysicalActor>
    {
        [InitRequired]
        public GameObject Tool { get; set; }
        [InitRequired]
        public GameObject Target { get; set; }

        public override FormattableString AnnouncementText(IGameObject viewer)
        {
            return $"{Actor.DisplayName(viewer)} {RelativeName(viewer)} {Target.DisplayName(viewer)} with {Tool.DisplayName(viewer)}.";
        }
        protected override IEnumerable<(TargetType, GameObject)> Stakeholders()
        {
            yield return (TargetType.Actor, Actor);
            yield return (TargetType.Target, Target);
            yield return (TargetType.Tool, Tool);
        }

        protected ActionOutcome HasTool()
        {
            if (!Actor.HasItem(Tool))
            {
                return Fail($"You don't have {NameOf(Tool)}");
            }
            return Succeed();
        }
    }
}