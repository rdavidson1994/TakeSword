using System;
using System.Collections.Generic;

namespace TakeSword
{
    public enum TargetType
    {
        Actor, Target, Tool, Bystander
    }
    public abstract class ActionOutcome
    {
        public abstract bool Success();
        public static implicit operator bool(ActionOutcome outcome)
        {
            return outcome.Success();
        }
        public static bool operator false(ActionOutcome actionOutcome)
        {
            return !actionOutcome;
        }
        public static bool operator true(ActionOutcome actionOutcome)
        {
            return actionOutcome;
        }
        public static ActionOutcome operator |(ActionOutcome first, ActionOutcome second)
        {
            if (first.Success())
                return first;
            return second;
        }

        public static ActionOutcome operator &(ActionOutcome first, ActionOutcome second)
        {
            if (!first.Success())
                return first;
            return second;
        }
    }

    public class SuccessfulOutcome : ActionOutcome
    {
        public override bool Success()
        {
            return true;
        }
    }

    public class FailedOutcome : ActionOutcome
    {
        public override bool Success()
        {
            return false;
        }

        public FormattableString Reason { get; protected set; }
        public FailedOutcome(FormattableString reason)
        {
            Reason = reason;
        }
    }

    public abstract class PhysicalAction : IAction
    {
        protected abstract string Name();
        protected virtual string RelativeName(IGameObject viewer)
        {
            if (Actor == viewer)
            {
                return Name();
            }
            else
            {
                return Name() + "s";
            }
        }
        public virtual FormattableString AnnouncementText(IGameObject viewer)
        {
            return $"{Actor.DisplayName(viewer)} {RelativeName(viewer)}.";
        }
        public long OnsetTime { get; set; } = 750;
        public long CooldownTime { get; set; } = 250;
        public PhysicalActor Actor { get; set; }
        public IActor GetActor()
        {
            return Actor;
        }
        protected ActionOutcome Succeed()
        {
            return new SuccessfulOutcome();
        }
        protected ActionOutcome Fail(FormattableString reason)
        {
            return new FailedOutcome(reason);
        }
        public string NameOf(GameObject gameObject)
        {
            return gameObject.DisplayName(Actor);
        }
        public IRoutine AsRoutine()
        {
            return new WrapperRoutine(this);
        }
        private void Announce(ActionOutcome outcome)
        {
            foreach (var (type, stakeholder) in Stakeholders())
            {
                var announcement = new ActionAnnouncement(this, outcome, type);
                stakeholder.HandleAnnouncement(announcement);
            }
        }

        protected ActionOutcome Has(GameObject gameObject)
        {
            if (!Actor.HasItem(gameObject))
            {
                return Fail($"You don't have {NameOf(gameObject)}");
            }
            return Succeed();
        }

        protected ActionOutcome CanReach(GameObject gameObject)
        {
            if (!Actor.CanReach(gameObject))
                return Fail($"You cannot reach {NameOf(gameObject)}");
            return Succeed();
        }

        public ActionOutcome Attempt()
        {
            ActionOutcome outcome = IsValid();
            if (!outcome)
            {
                Announce(outcome);
                return outcome;
            }

            foreach (var (type, stakeholder) in Stakeholders())
            {
                outcome = stakeholder.Allows((dynamic)this, type);
                if (!outcome)
                {
                    Announce(outcome);
                    return outcome;
                }
            }
            outcome = Execute();
            Announce(outcome);
            return outcome;

        }
        protected virtual IEnumerable<(TargetType, GameObject)> Stakeholders()
        {
            yield return (TargetType.Actor, Actor);
        }
        public abstract ActionOutcome IsValid();
        protected abstract ActionOutcome Execute();

    }

    public abstract class TargetedAction : PhysicalAction
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

        public override ActionOutcome IsValid()
        {
            return CanReach(Target);
        }
    }

    public abstract class ToolAction : TargetedAction
    {
        public GameObject Tool { get; set; }
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

    public class Take : TargetedAction
    {
        private ActionOutcome DoesNotHave(GameObject gameObject)
        {
            if (Actor.HasItem(gameObject))
                return Fail($"You already have {NameOf(Target)}");
            return Succeed();
        }
        public override ActionOutcome IsValid()
        {
            return DoesNotHave(Target) && CanReach(Target);
        }

        protected override ActionOutcome Execute()
        {
            Target.Move(Actor);
            return Succeed();
        }

        protected override string Name() => "take";
    }

    public class Drop : TargetedAction
    {
        protected override string Name() => "drop";
        public override ActionOutcome IsValid()
        {
            if (!Actor.HasItem(Target))
            {
                return Fail($"You don't have {NameOf(Target)}");
            }
            return Succeed();
        }

        protected override ActionOutcome Execute()
        {
            Target.Move(Actor.Location);
            return Succeed();
        }
    }

    public class WeaponStrike : ToolAction
    {
        protected override string Name() => "hit";
        public override ActionOutcome IsValid()
        {
            return CanReach(Target) && Has(Tool);
        }

        protected override ActionOutcome Execute()
        {
            return Succeed();
        }
    }



    public class AutoFailAction : IAction
    {
        private IActor actor;
        private FailedOutcome staticFailedOutcome;

        public long OnsetTime => 0;

        public long CooldownTime => 0;
        
        public AutoFailAction(IActor actor, FailedOutcome staticFailedOutcome)
        {
            this.actor = actor;
            this.staticFailedOutcome = staticFailedOutcome;
        }

        public IRoutine AsRoutine()
        {
            return new WrapperRoutine(this);
        }

        public ActionOutcome Attempt()
        {
            return staticFailedOutcome;
        }

        public IActor GetActor()
        {
            return actor;
        }

        public ActionOutcome IsValid()
        {
            return staticFailedOutcome;
        }
    }
}