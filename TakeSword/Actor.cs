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
        public PhysicalActor(ILocation location) : base(location) { }

        public PhysicalActor() : base() { }

        protected IEvent ScheduledEvent { get; set; }
        public IRoutine AI { get; protected set; }

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

        public int Reach { get; set; } = 2000;
        public bool CanReach(IGameObject gameObject)
        {
            return Location.NearbyObjects(Reach).Contains(gameObject);
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