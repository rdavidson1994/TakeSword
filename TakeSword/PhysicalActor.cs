using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public interface IPhysicalActor : IGameObject, IActor, ILocation
    {
        bool CanReach(IGameObject target);
        bool HasItem(IGameObject item);
    };

    public class PhysicalActor : GameObject, IPhysicalActor
    {
        public PhysicalActor(ILocation location = null, FrozenTraitStore traits = null) : base(location, traits) { }

        protected IEvent ScheduledEvent { get; set; }
        public IRoutine AI { get; set; }

        public void Act()
        {
            IAction action = AI.NextAction();
            IEvent actionEvent = new ActionEvent() {
                Action = action,
                Actor = this,
            };
            ScheduledEvent = actionEvent;
            Schedule.Add(actionEvent, action.OnsetTime);
        }

        public bool HasItem(IGameObject item)
        {
            return Contents.Contains(item);
        }

        public void AttemptAction(IAction action)
        {
            action.Attempt();
            IEvent cooldownEvent = new CooldownEvent()
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
                Location.NearbyObjects(SightRange)).Where((obj)=>obj.HasName(this, name)
            );
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

        protected override void ReactToAnnouncement(object announcement)
        {
            AI.ReactToAnnouncement(announcement);
            base.ReactToAnnouncement(announcement);
        }
    }
    
    public class ActionEvent : IEvent
    {
        public IAction Action { get; set; }
        public IActor Actor { get; set; }

        public void Happen()
        {
            Actor.AttemptAction(Action);
        }
    }

    public class CooldownEvent : IEvent
    {
        public IActor Actor { get; set; }
        public IAction Action { get; set; }
        public void Happen()
        {
            Actor.Act();
        }
    }
}