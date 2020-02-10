using System;

namespace TakeSword
{
    public class FailedOutcome : ActionOutcome
    {
        public override bool Success()
        {
            return false;
        }

        public FormattableString Reason { get; protected set; }
        public FailedOutcome(FormattableString reason)
        {
            Reason = reason;
        }
    }
}