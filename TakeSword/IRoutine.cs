using System;

namespace TakeSword
{
    public interface IRoutine<TActor> : IActivity<TActor>
    {
        IAction<TActor> Peek();
        IAction<TActor> NextAction();
        void ReactToAnnouncement(ActionAnnouncement announcement);
        void ViewLocation(ILocation location);
        void ViewInventory();
        void RecieveTextMessage(FormattableString text);
        void ResumeMessages();
        void SuspendMessages();
    }
}