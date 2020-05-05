using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public interface IBody
    {
        bool Alive { get; }
        bool NeedsUpdate { get; }
        void TakeDamage(int amount, DamageType damageType, BodyPartKind bodyPart);
        void Update(int deltaTime);
    }

    public class BasicBody : IBody
    {
        private int HP = 100;
        public bool Alive => HP > 0;

        public bool NeedsUpdate => false;

        public void TakeDamage(int amount, DamageType damageType, BodyPartKind bodyPart)
        {
            HP -= amount;
        }

        public void Update(int deltaTime)
        {
            // Do nothing
        }
    }
    public class PhysicalActor : GameObject, IActor<PhysicalActor>
    {
        const int BODY_UPDATE_DELTA_TIME = 1000;
        bool updatingBody  = false;
        public PhysicalActor(IBody body, ILocation location = null, FrozenTraitStore traits = null)
            : base(location, traits)
        {
            this.body = body;
        }
        private IBody body;
        public bool Alive { get; set; } = true;
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

        private void UpdateBody()
        {
            body.Update(BODY_UPDATE_DELTA_TIME);
            if (body.NeedsUpdate)
            {
                Schedule.Add(new CallbackEvent(UpdateBody), BODY_UPDATE_DELTA_TIME);
            }
            else
            {
                updatingBody = false;
            }
        }

        private class BodyUpdateEvent : IEvent
        {
            private PhysicalActor actor;
            public BodyUpdateEvent(PhysicalActor actor)
            {
                this.actor = actor;
            }
            public void Happen()
            {
                actor.UpdateBody();
            }
        }

        public override void TakeDamage(int amount, DamageType type, BodyPartKind bodyPart)
        {
            body.TakeDamage(amount, type, bodyPart);
            AI?.RecieveTextMessage($"You take {amount} {type.ToString().ToLowerInvariant()} damage");
            if (!updatingBody && body.NeedsUpdate) {
                UpdateBody();
            }
            if (!body.Alive)
            {
                AI?.Die();
                Die();
            }
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

        public void CreateCorpse()
        {
            var corpse = new GameObject(Location);
            // // Todo: Better name concatenation
            // corpse.Name = this.Name.PlusSuffix("'s corpse"); // or similar
            corpse.Name = this.Name.Possessive("corpse");
        }

        public void Die()
        {
            Alive = false;
            Location.HandleTextMessage($"{this} has died.");
            CreateCorpse();
            Vanish();
        }

        public override void ReceiveTextMessage(FormattableString text)
        {
            if (AI != null)
            {
                AI.RecieveTextMessage(text);
            }
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