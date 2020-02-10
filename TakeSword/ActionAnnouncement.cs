using System;
using System.Collections.Generic;
using System.Text;

namespace TakeSword
{
    public enum AnnouncementContext
    {
        Before,
        After
    }
    public class ActionAnnouncement
    {
        public ActionAnnouncement(IActivity activity, ActionOutcome outcome, TargetType relationship)
        {
            Activity = activity;
            Outcome = outcome;
            Relationship = relationship;
        }
        public bool Is<T>(out T action) where T : class
        {
            if (Activity != null && Activity is T tAction)
            {
                action = tAction;
                return true;
            }
            else
            {
                action = null;
                return false;
            }
        }
        public IActivity Activity { get; private set; }
        public ActionOutcome Outcome { get; private set; }
        public TargetType Relationship { get; private set; }
    }

    public interface IReaction
    {
        void Handle(object input);
    }
}
