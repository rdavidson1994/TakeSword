using System;

namespace TakeSword
{
    public abstract class InstantAction : SimpleAction
    {
        public override long OnsetTime => 0;
        public override long CooldownTime => 0;
    }

    public abstract class DisplayAction : InstantAction
    {
        public override bool Quiet => true;
        protected abstract FormattableString Display();
        protected override ActionOutcome Run(bool execute)
        {
            if (execute)
            {
                Actor.ReceiveTextMessage(Display());
            }
            return Succeed();
        }
    }
}