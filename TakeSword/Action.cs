using System;
namespace TakeSword
{
    public enum TargetType
    {
        None,
        Actor,
        Target,
        Tool,
        Bystander,
        Scene,
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
            if (!Target.Is<InventoryItem>())
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
            if (Target.Is(out Food _))
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

    public class AutoFailAction<TActor> : IAction<TActor>
    {
        private FailedOutcome staticFailedOutcome;

        public long OnsetTime => 0;

        public long CooldownTime => 0;
        
        public AutoFailAction(TActor actor, FailedOutcome staticFailedOutcome)
        {
            this.Actor = actor;
            this.staticFailedOutcome = staticFailedOutcome;
        }

        public IRoutine<TActor> AsRoutine()
        {
            return new WrapperRoutine<TActor>(this);
        }

        public ActionOutcome Attempt()
        {
            return staticFailedOutcome;
        }

        public TActor Actor { get; set; }

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