using SmartAnalyzers.CSharpExtensions.Annotations;
using System;
using System.Collections.Generic;
namespace TakeSword
{
    public abstract class TargetedAction : PhysicalAction, ITargetedActivity<PhysicalActor>
    {
        public override FormattableString AnnouncementText(IGameObject viewer)
        {
            // For example, if Actor=Dog, Action=Bite, and Target=Man, this returns
            //        The dog...                  bites...                    the man.
            return $"{Actor.DisplayName(viewer)} {this.RelativeName(viewer)} {Target.DisplayName(viewer)}.";
        }
        protected override IEnumerable<(TargetType, GameObject)> Stakeholders()
        {
            yield return (TargetType.Actor, Actor);
            yield return (TargetType.Target, Target);
        }

        [InitRequired]
        public GameObject Target { get; set; }
    }
}