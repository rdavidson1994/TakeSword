namespace TakeSword
{
    public class Enter : TargetedAction
    {
        protected override ActionOutcome Run(bool execute)
        {
            if (CanReach(Target) is FailedOutcome notInReach)
                return notInReach;
            if (Target is Portal portal)
            {
                if (execute)
                {
                    Actor.Move(portal.Opposite.Location);
                    return Succeed();
                }
                else
                {
                    return Succeed();
                }
            }
            return Fail($"{Target} is not a portal.");
        }

        protected override string Name => "enter";
    }
}