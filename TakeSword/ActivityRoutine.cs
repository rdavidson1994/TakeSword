﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public abstract class ActivityRoutine<TActor> : IRoutine<TActor>
    {
        public virtual FormattableString EmptyReason { get; protected set; } = $"You can't do that right now.";
        private IRoutine<TActor> Routine { get; set; }
        protected IAction<TActor> StoredAction { get; private set; }
        public TActor Actor { get; set; }

        public abstract IActivity<TActor> NextActivity();

        public IAction<TActor> NextAction()
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
                IAction<TActor> action = Routine.NextAction();
                if (action != null)
                {
                    return action;
                }
                else
                {
                    Routine = null;
                }
            }

            IActivity<TActor> activity = NextActivity();
            if (activity == null)
            {
                return null;
            }
            if (activity is IAction<TActor> atomicAction)
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
        public IAction<TActor> Peek()
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

        public IRoutine<TActor> AsRoutine()
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

        public virtual void ViewLocation(ILocation location)
        {
            // By default, do nothing
        }

        public virtual void ViewInventory()
        {
            // By default, do nothing
        }

        public virtual void RecieveTextMessage(FormattableString text) { }

        public virtual void ResumeMessages()
        {

        }

        public virtual void SuspendMessages()
        {

        }
    }

    public abstract class SingleActivity<TActor> : ActivityRoutine<TActor>
    {
        private bool done = false;

        public abstract IActivity<TActor> GetActivity();

        public override IActivity<TActor> NextActivity()
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


    public class WrapperRoutine<TActor> : ActivityRoutine<TActor>
    {
        private bool done;
        private IActivity<TActor> wrappedActivity;

        public WrapperRoutine(IActivity<TActor> wrappedActivity)
        {
            Actor = wrappedActivity.Actor;
            done = false;
            this.wrappedActivity = wrappedActivity;
        }

        //public override TActor Actor { get; set; }

        public override IActivity<TActor> NextActivity()
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



    //public abstract class ActivityRoutine<PhysicalActor> : ActivityRoutine, IActivity<PhysicalActor>
    //{
    //    public PhysicalActor Actor { get; set; }

    //    public override IActor Actor => Actor;
    //}

    public abstract class GeneratorRoutine<TActor> : ActivityRoutine<TActor>
    {
        protected abstract IEnumerable<IActivity<TActor>> Activities();
        private IEnumerator<IActivity<TActor>> activityEnumerator;
        public GeneratorRoutine()
        {
            activityEnumerator = Activities().GetEnumerator();
        }

        ~GeneratorRoutine()
        {
            activityEnumerator?.Dispose();
        }

        public override IActivity<TActor> NextActivity()
        {
            if (activityEnumerator.MoveNext())
            {
                return activityEnumerator.Current;
            }
            return null;
        }
    }

    public class ActOnAll<ActionType> : GeneratorRoutine<PhysicalActor> where ActionType : ITargetedActivity<PhysicalActor>, new()
    {
        //public override PhysicalActor Actor { get; set; }

        protected override IEnumerable<IActivity<PhysicalActor>> Activities()
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

    public class GoDirection : SingleActivity<PhysicalActor>, IDirectionActivity
    {
        public Direction Direction { get; set; }
       // public override PhysicalActor Actor { get; set; }

        public override IActivity<PhysicalActor> GetActivity()
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