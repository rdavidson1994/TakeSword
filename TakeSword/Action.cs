using System;
using System.Collections.Generic;

namespace TakeSword
{
    public enum TargetType
    {
        Actor, Target, Tool, Bystander
    }

    public abstract class PhysicalAction : IAction, IPhysicalActivity
    {
        protected abstract string Name { get; }

        protected virtual string RelativeName(IGameObject viewer)
        {
            if (Actor == viewer)
            {
                return Name;
            }
            else
            {
                return Name + "s";
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
                outcome = stakeholder.Allows(new ActionAnnouncement(this, null, type));
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

    public abstract class TargetedAction : PhysicalAction, ITargetedActivity
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

    public abstract class ToolAction : TargetedAction, IToolActivity
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
            if (!Target.Is<ItemTrait>())
                return Fail($"You can only take reasonably sized inanimate objects.");
            return DoesNotHave(Target) && CanReach(Target);
        }

        protected override ActionOutcome Execute()
        {
            Target.Move(Actor);
            return Succeed();
        }

        protected override string Name => "take";
    }

    public class Drop : TargetedAction
    {
        protected override string Name => "drop";
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
        protected override string Name => "hit";
        public override ActionOutcome IsValid()
        {
            return CanReach(Target) && Has(Tool);
        }

        protected override ActionOutcome Execute()
        {
            return Succeed();
        }
    }

    public class WaitAction : PhysicalAction
    {
        public override ActionOutcome IsValid()
        {
            return Succeed();
        }

        protected override ActionOutcome Execute()
        {
            return Succeed();
        }

        protected override string Name => "wait";
    }

    public class Eat : TargetedAction
    {
        protected override string Name => "eat";

        public override ActionOutcome IsValid()
        {
            if (Target.Is<Food>())
            {
                return Has(Target);
            }
            return Fail($"{Target} is not edible.");
        }

        protected override ActionOutcome Execute()
        {
            if (Target.Is(out Food food))
            {
                Target.Vanish();
                Console.WriteLine($"Yum! +{food.Nutrition} nutrition.");
                return Succeed();
            }
            return Fail($"Not edible");
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

    public class Enter : TargetedAction
    {
        private Portal targetAsPortal;
        public override ActionOutcome IsValid()
        {
            ActionOutcome base_ = base.IsValid();
            if (!base_) return base_;
            if (Target is Portal portal)
            {
                targetAsPortal = portal;
                return Succeed();
            }
            return Fail($"{Target} is not a portal.");
        }
        protected override ActionOutcome Execute()
        {
            Actor.Move(targetAsPortal.Opposite.Location);
            return Succeed();
        }

        protected override string Name => "enter";
    }
}