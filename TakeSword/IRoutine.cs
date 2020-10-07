using System;

namespace TakeSword
{
    public interface IRoutine<TActor> : IActivity<TActor>
    {
        IAction<TActor>? Peek();
        IAction<TActor>? NextAction();
        void ReactToAnnouncement(ActionAnnouncement announcement);
        void RecieveTextMessage(FormattableString text);
        void Die();
    }
    // // Todo: Consider something like this?
    //public interface IAIRoutine<TActor> : IActivity<TActor>
    //{
    //    IAction<TActor> Peek();
    //    IAction<TActor> NextAction();
    //}
}