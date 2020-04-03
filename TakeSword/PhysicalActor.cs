using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public class PhysicalActor : GameObject, IActor<PhysicalActor>
    {
        public PhysicalActor(ILocation location = null, FrozenTraitStore traits = null) : base(location, traits) { }
        protected IEvent ScheduledEvent { get; set; }
        public IRoutine<PhysicalActor> AI { get; set; }
        protected Dictionary<SkillType, double> skillValues;

        public void Act()
        {
            IAction<PhysicalActor> action = AI.NextAction();
            IEvent actionEvent = new ActionEvent<PhysicalActor>() {
                Action = action,
                Actor = this,
            };
            ScheduledEvent = actionEvent;
            Schedule.Add(actionEvent, action.OnsetTime);
        }

        public int MeleeDamage(GameObject weapon)
        {
            double damage = new Random().Next(10, 50);
            if (weapon.Is(out Weapon actualWeapon))
            {
                damage *= actualWeapon.DamageMultiplier;
            }
            if (Is(out PhysicalStats stats))
            {
                damage *= stats.Strength;
            }
            return Convert.ToInt32(damage);
        }

        public bool HasItem(IGameObject item)
        {
            return Contents.Contains(item);
        }

        public void AttemptAction(IAction<PhysicalActor> action)
        {
            action.Attempt();
            IEvent cooldownEvent = new CooldownEvent<PhysicalActor>()
            {
                Actor = this,
            };
            ScheduledEvent = cooldownEvent;
            Schedule.Add(cooldownEvent, action.CooldownTime);
        }
        public int SightRange { get; set; } = 100000;
        public int Reach { get; set; } = 2000;

        public IEnumerable<GameObject> TargetsByName(string name)
        {
            return Enumerable.Concat(
                contents,
                Location.NearbyObjects(SightRange)
            ).Where(obj=>obj.HasName(this, name));
        }

        public IEnumerable<GameObject> ItemsInReach()
        {
            return Enumerable.Concat(
                contents,
                Location.NearbyObjects(Reach) 
            ).Where(obj => obj != this);
        }

        public GameObject ObjectByName(string name)
        {
            var candidates = Enumerable.Concat(contents, Location.NearbyObjects(SightRange));
            foreach (GameObject thing in candidates)
            {
                if (thing.Name.Matches(this, name))
                {
                    return thing;
                }
            }
            return null;
        }

        public bool CanReach(IGameObject gameObject)
        {
            return Location.NearbyObjects(Reach).Contains(gameObject) || HasItem(gameObject);
        }

        public ActionOutcome Take(GameObject target)
        {
            target.Move(this);
            return new SuccessfulOutcome();
        }

        protected override void ReactToAnnouncement(ActionAnnouncement announcement)
        {
            if (AI != null)
            {
                AI.ReactToAnnouncement(announcement);
            }
            base.ReactToAnnouncement(announcement);
        }
    }
    
    public class ActionEvent<T> : IEvent
    {
        public IAction<T> Action { get; set; }
        public IActor<T> Actor { get; set; }

        public void Happen()
        {
            Actor.AttemptAction(Action);
        }
    }

    public class CooldownEvent<T> : IEvent
    {
        public IActor<T> Actor { get; set; }
        public IAction<T> Action { get; set; }
        public void Happen()
        {
            Actor.Act();
        }
    }
}