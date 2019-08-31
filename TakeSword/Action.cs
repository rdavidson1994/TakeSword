using System.Linq;

namespace TakeSword
{
    public interface IActionOutcome
    {
        IAction Action { get; }
        bool Success();
    }

    public class SuccessfulOutcome : IActionOutcome
    {
        public IAction Action { get; protected set; }
        public SuccessfulOutcome(IAction action)
        {
            this.Action = action;
        }
        public bool Success()
        {
            return true;
        }
    }

    public class FailedOutcome : IActionOutcome
    {
        public IAction Action { get; protected set; }
        public bool Success()
        {
            return false;
        }

        public string Reason { get; protected set; }
        public FailedOutcome(string reason, IAction action)
        {
            Reason = reason;
            Action = action;
        }
    }

    public abstract class PhysicalAction : IAction
    {
        public long OnsetTime { get; set; } = 750;
        public long CooldownTime { get; set; } = 250;
        public IPhysicalActor Actor { get; protected set; }
        public PhysicalAction(IPhysicalActor actor)
        {
            this.Actor = actor;
        }
        public IActor GetActor()
        {
            return Actor;
        }
        protected IActionOutcome Succeed()
        {
            return new SuccessfulOutcome(this);
        }
        protected IActionOutcome Fail(string reason)
        {
            return new FailedOutcome(reason, this);
        }

        public IActionOutcome Attempt()
        {
            return Do(false);
        }

        public IActionOutcome IsValid()
        {
            return Do(true);
        }

        public string NameOf(GameObject gameObject)
        {
            return gameObject.GetName(Actor);
        }
        protected abstract IActionOutcome Do(bool isPreflight);
    }

    public abstract class TargetedAction : PhysicalAction
    {
        public IGameObject Target { get; protected set; }
        public TargetedAction(IPhysicalActor actor, IGameObject target) : base(actor)
        {
            Target = target;
        }
    }

    public abstract class ToolAction : TargetedAction
    {
        public IGameObject Tool { get; protected set; }
        public ToolAction(PhysicalActor actor, GameObject target, GameObject tool) : base(actor, target)
        {
            Tool = tool;
        }

    }

    public class Take : TargetedAction
    {
        public Take(PhysicalActor actor, GameObject target) : base(actor, target) { }
        protected override IActionOutcome Do(bool isPreflight)
        {
            if (!Actor.CanReach(Target))
            {
                return Fail($"You cannot reach {Target.GetName(Actor)}");
            }
            if (Actor.HasItem(Target))
            {
                return Fail($"You already have {Target.GetName(Actor)}");
            }
            if (isPreflight)
            {
                return Succeed();
            }
            Target.Move(Actor);  
            return Succeed();
        }
    }

    public class Drop : TargetedAction
    {
        public Drop(PhysicalActor actor, GameObject target) : base(actor, target) { }

        protected override IActionOutcome Do(bool isPreflight)
        {
            if (!Actor.HasItem(Target))
            {
                return Fail($"You don't have {Target.GetName(Actor)}");
            }
            if (isPreflight)
            {
                return Succeed();
            }
            Target.Move(Actor.Location);
            return Succeed();
        }
    }
}