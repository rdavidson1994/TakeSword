﻿using SmartAnalyzers.CSharpExtensions.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace TakeSword
{
    public enum TargetType
    {
        None,
        Actor,
        Target,
        Tool,
        Witness,
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

        protected override ActionOutcome Run(bool execute)
        {
            ActionOutcome prereqs =
                Target.Is<InventoryItem>($"You can only take reasonably sized inanimate objects.")
                && CanReach(Target)
                && DoesNotHave(Target);
            if (!prereqs)
            {
                return prereqs;
            }

            if (execute)
            {
                Target.Move(Actor);
            }
            return Succeed();
        }

        protected override string Name => "take";
    }

    public class Drop : TargetedAction
    {
        protected override string Name => "drop";
        protected override ActionOutcome Run(bool execute)
        {
            if (!Actor.HasItem(Target))
            {
                return Fail($"You don't have {NameOf(Target)}");
            }
            if (execute)
            {
                Target.Move(Actor.Location);
            }
            return Succeed();
        }
    }

    public enum BodyPartKind
    {
        Any,
        LeftArm,
        LeftLeg,
        RightArm,
        RightLeg,
        Torso,
        Head
    }

    public class UnarmedStrike : TargetedAction
    {
        protected override string Name => "hit";

        protected override ActionOutcome Run(bool execute = true)
        {
            var prereqs = CanReach(Target);
            if (!prereqs) return prereqs;
            if (execute)
            {
                Target.TakeDamage(Actor.MeleeDamage(Actor), DamageType.Blunt, BodyPartKind.Any);
            }
            return Succeed();
        }
    }

    public class BestWeaponStrike : SingleActivity<PhysicalActor>, ITargetedActivity<PhysicalActor>
    {
        [InitRequired]
        public GameObject Target { get; set; }

        public override IActivity<PhysicalActor> GetActivity()
        {
            GameObject? bestWeapon = null;
            double bestMultiplier = 1.0;
            foreach (GameObject item in Actor.Contents)
            {
                if (item.Is(out Weapon? weapon))
                {
                    if (weapon.DamageMultiplier > bestMultiplier)
                    {
                        bestWeapon = item;
                        bestMultiplier = weapon.DamageMultiplier;
                    }
                }
            }
            if (bestWeapon is null)
            {
                return Do<UnarmedStrike>(Target);
            }
            else
            {
                return Do<WeaponStrike>(Target, bestWeapon);
            }
        }
    }

    public class WeaponStrike : ToolAction
    {
        protected override string Name => "hit";

        protected override ActionOutcome Run(bool execute)
        {
            ActionOutcome prereqs = Has(Tool) && CanReach(Target);
            if (!prereqs) return prereqs;
            if (execute)
            {
                DamageType type = DamageType.Blunt;
                if (Tool.Is(out Weapon? weapon))
                {
                    type = weapon.DamageType;
                }
                Target.TakeDamage(Actor.MeleeDamage(Tool), type, BodyPartKind.Any);
            }
            return Succeed();
        }
    }

    public class DisplayInventory : DisplayAction
    {
        protected override string Name => "examine your possessions";

        protected override FormattableString Display()
        {
            var formatBuilder = new FormatBuilder();
            bool anything = false;
            formatBuilder.AddLine($"Inventory:");
            foreach (GameObject item in Actor.NearbyObjects(Actor.Reach))
            {
                anything = true;
                formatBuilder.AddLine(item.ShortDescription(Actor));
            }
            if (!anything)
            {
                formatBuilder.AddLine($"You have no possessions");
            }
            return formatBuilder.Build();
        }
    }

    public class Look : DisplayAction
    {
        protected override string Name => "look";

        protected override FormattableString Display()
        {
            var formatBuilder = new FormatBuilder();
            formatBuilder.AddLine(Actor.Location.DescriptionForInhabitant(Actor));
            formatBuilder.AddLine($"Nearby items:");
            foreach (GameObject item in Actor.Location.NearbyObjects(Actor.SightRange))
            {
                formatBuilder.AddLine(item.ShortDescription(Actor));
            }
            return formatBuilder.Build();
        }
    }

    public class WaitAction : SimpleAction
    {
        protected override string Name => "wait";

        protected override ActionOutcome Run(bool execute)
        {
            return Succeed();
        }
    }

    public class Eat : TargetedAction
    {
        protected override string Name => "eat";
        protected override ActionOutcome Run(bool execute)
        {
            if (!Target.Is(out Food? food))
            {
                return Fail($"{Target} isn't edible.");
            }
            if (Has(Target) is FailedOutcome notHoldingFood) {
                return notHoldingFood;
            }

            if (execute)
            {
                Target.Vanish();
                Actor.ReceiveTextMessage($"Yum! +{food.Nutrition} nutrition.");
            }
            return Succeed();
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
}