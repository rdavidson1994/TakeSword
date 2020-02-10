using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public abstract class ActivityRoutine : IRoutine
    {
        public virtual FormattableString EmptyReason { get; protected set; } = $"You can't do that right now.";
        private IRoutine Routine { get; set; }
        protected IAction StoredAction { get; private set; }
        public abstract IActor GetActor();
        public abstract IActivity NextActivity();
        public IAction NextAction()
        {
            if (StoredAction != null)
            {
                // Return StoredAction and simultaneously set it to null
                var temp = StoredAction;
                StoredAction = null;
                return temp;
            }
            if (Routine != null)
            {
                IAction action = Routine.NextAction();
                if (action != null)
                {
                    return action;
                }
                else
                {
                    Routine = null;
                }
            }

            IActivity activity = NextActivity();
            if (activity == null)
            {
                return null;
            }
            if (activity is IAction atomicAction)
            {
                return atomicAction;
            }
            else
            {
                Routine = activity.AsRoutine();
                return NextAction();
            }
        }

        //Return the next action, but don't discard it
        public IAction Peek()
        {
            if (StoredAction == null)
            {
                StoredAction = NextAction();
            }
            return StoredAction;
        }

        public ActionOutcome IsValid()
        {
            var previewAction = Peek();
            if (previewAction != null)
            {
                return previewAction.IsValid();
            }
            return new FailedOutcome(EmptyReason);
        }

        public IRoutine AsRoutine()
        {
            return this;
        }

        public virtual void ReactToAnnouncement(ActionAnnouncement announcement)
        {
            if (Routine != null)
            {
                Routine.ReactToAnnouncement(announcement);
            }
        }
    }

    public abstract class SingleActivity : PhysicalRoutine
    {
        private bool done = false;

        public abstract IActivity GetActivity();

        public override IActivity NextActivity()
        {
            if (done)
            {
                return null;
            }
            else
            {
                done = true;
                return GetActivity();
            }
        }
    }


    public class WrapperRoutine : ActivityRoutine
    {
        private bool done;
        private IActor actor;
        private IActivity wrappedActivity;

        public WrapperRoutine(IActivity wrappedActivity)
        {
            actor = wrappedActivity.GetActor();
            done = false;
            this.wrappedActivity = wrappedActivity;
        }
        public override IActor GetActor()
        {
            return actor;
        }

        public override IActivity NextActivity()
        {
            if (done)
            {
                return null;
            }
            else
            {
                done = true;
                return wrappedActivity;
            }
        }
    }



    public abstract class PhysicalRoutine : ActivityRoutine, IPhysicalActivity
    {
        public PhysicalActor Actor { get; set; }

        public override IActor GetActor()
        {
            return Actor;
        }
    }

    public abstract class GeneratorRoutine : PhysicalRoutine
    {
        protected abstract IEnumerable<IActivity> Activities();
        private IEnumerator<IActivity> activityEnumerator;
        public GeneratorRoutine()
        {
            activityEnumerator = Activities().GetEnumerator();
        }

        ~GeneratorRoutine()
        {
            activityEnumerator?.Dispose();
        }

        public override IActivity NextActivity()
        {
            if (activityEnumerator.MoveNext())
            {
                return activityEnumerator.Current;
            }
            return null;
        }
    }

    public class ActOnAll<ActionType> : GeneratorRoutine where ActionType : ITargetedActivity, new()
    {
        protected override IEnumerable<IActivity> Activities()
        {
            foreach (var item in Actor.ItemsInReach().ToList())
            {
                var action = new ActionType
                {
                    Actor = Actor,
                    Target = item
                };
                if (action.IsValid())
                {
                    yield return action;
                }
            }
            yield break;
        }
    }

    public class TakeAll : GeneratorRoutine
    {
        protected override IEnumerable<IActivity> Activities()
        {
            foreach (var item in Actor.ItemsInReach().ToList())
            {
                var action = new Take
                {
                    Actor = Actor,
                    Target = item
                };
                if (action.IsValid())
                {
                    yield return action;
                }
            }
            yield break;
        }
    }

    public class GoDirection : SingleActivity, IDirectionActivity
    {
        public Direction Direction { get; set; }

        public override IActivity GetActivity()
        {
            IEnumerable<Portal> portals = Actor.ItemsInReach()
                .Select(item => item as Portal)
                .Where(portal => portal != null && portal.Direction == Direction);
            Portal foundPortal = portals.FirstOrDefault();
            if (foundPortal == null)
            {
                EmptyReason = $"There is no portal facing {Direction}";
                return null;
            }
            return new Enter
            {
                Actor = Actor,
                Target = portals.FirstOrDefault()
            };
        }
    }
}