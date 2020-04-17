using System;
using System.Collections.Generic;
namespace TakeSword
{
    public abstract class PhysicalAction : IAction<PhysicalActor>
    {
        public virtual bool Quiet { get; set; }
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
        public virtual long OnsetTime { get; set; } = 750;
        public virtual long CooldownTime { get; set; } = 250;
        public PhysicalActor Actor { get; set; }
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
        public IRoutine<PhysicalActor> AsRoutine()
        {
            return new WrapperRoutine<PhysicalActor>(this);
        }
        private void Announce(ActionOutcome outcome)
        {
            foreach (var (type, stakeholder) in Stakeholders())
            {
                var announcement = new ActionAnnouncement(this, outcome, type);
                stakeholder.HandleAnnouncement(announcement);
            }
            var locationAnnouncement = new ActionAnnouncement(this, outcome, TargetType.Scene);
            Actor.Location.HandleAnnouncement(locationAnnouncement);
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
            Actor.SuspendMessages();
            outcome = Execute();
            Announce(outcome);
            Actor.ResumeMessages();
            return outcome;

        }
        protected virtual IEnumerable<(TargetType, GameObject)> Stakeholders()
        {
            yield return (TargetType.Actor, Actor);
        }
        protected abstract ActionOutcome Run(bool execute=true);
        public ActionOutcome IsValid()
        {
            return Run(execute: false);
        }
        protected ActionOutcome Execute()
        {
            return Run();
        }

    }
}