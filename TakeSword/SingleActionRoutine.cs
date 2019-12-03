namespace TakeSword
{
    public class SingleActionRoutine : IRoutine
    {
        private readonly IAction action;
        private bool done = false;
        public SingleActionRoutine(IAction action)
        {
            this.action = action;
        }
        public IRoutine AsRoutine() => this;

        public IActor GetActor() => action.GetActor();

        public ActionOutcome IsValid() => action.IsValid();

        public IAction NextAction()
        {
            if (done)
            {
                return null;
            }
            else
            {
                done = true;
                return action;
            }
        }

        public void ReactToAnnouncement(object announcement) { }
    }
}