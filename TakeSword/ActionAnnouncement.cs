using System;
using System.Collections.Generic;
using System.Text;

namespace TakeSword
{
    public class ActionAnnouncement
    {
        public ActionAnnouncement(IActivity activity, ActionOutcome outcome, TargetType relationship)
        {
            Activity = activity;
            Outcome = outcome;
            Relationship = relationship;
        }
        public IActivity Activity { get; private set; }
        public ActionOutcome Outcome { get; private set; }
        public TargetType Relationship { get; private set; }
    }
}
