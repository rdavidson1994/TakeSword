namespace TakeSword
{
    public abstract class ActivityRoutine : IRoutine
    {

        public IActor Actor { get; protected set; }
        private IRoutine Routine { get; set; }
        protected IAction StoredAction { get; private set; }
        public ActivityRoutine(IActor actor)
        {
            Actor = actor;
        }
        public IActor GetActor()
        {
            return Actor;
        }

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
            if (activity is IRoutine routine)
            {
                Routine = routine;
                return NextAction();
            }
            else if (activity is IAction action)
            {
                return action;
            }
            else
            {
                throw new System.Exception("Unsupported activity type");
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
    }
}