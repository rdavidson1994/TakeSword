namespace TakeSword
{
    public abstract class ActionOutcome
    {
        public abstract bool Success();
        public static implicit operator bool(ActionOutcome outcome)
        {
            return outcome.Success();
        }
        public static bool operator false(ActionOutcome actionOutcome)
        {
            return !actionOutcome;
        }
        public static bool operator true(ActionOutcome actionOutcome)
        {
            return actionOutcome;
        }
        public static ActionOutcome operator |(ActionOutcome first, ActionOutcome second)
        {
            if (first.Success())
                return first;
            return second;
        }

        public static ActionOutcome operator &(ActionOutcome first, ActionOutcome second)
        {
            if (!first.Success())
                return first;
            return second;
        }
    }
}