using System.Linq;

namespace TakeSword
{
    public abstract class ActivityRoutine : IRoutine
    {

        private IRoutine Routine { get; set; }
        protected IAction StoredAction { get; private set; }
        public abstract IActor GetActor();

        public abstract IActivity NextActivity();

        
        public IAction NextAction()
        {
            if (StoredAction != null)
            {
                return StoredAction;
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

        public ActionOutcome IsValid()
        {
            if (StoredAction == null)
            {
                StoredAction = NextAction();
            }
            return StoredAction.IsValid();
        }

        public IRoutine AsRoutine()
        {
            return this;
        }

        public virtual void ReactToAnnouncement(object announcement)
        {
            if (Routine != null)
            {
                Routine.ReactToAnnouncement(announcement);
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
}