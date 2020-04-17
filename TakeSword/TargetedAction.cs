using System;
using System.Collections.Generic;
namespace TakeSword
{
    public abstract class TargetedAction : PhysicalAction, ITargetedActivity<PhysicalActor>
    {
        public override FormattableString AnnouncementText(IGameObject viewer)
        {
            return $"{Actor.DisplayName(viewer)} {RelativeName(viewer)} {Target.DisplayName(viewer)}.";
        }
        protected override IEnumerable<(TargetType, GameObject)> Stakeholders()
        {
            yield return (TargetType.Actor, Actor);
            yield return (TargetType.Target, Target);
        }
        public GameObject Target { get; set; }
    }
}