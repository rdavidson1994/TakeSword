using System;
using System.Collections.Generic;

namespace TakeSword
{
    public class RequirementManager
    {
        private Dictionary<string, string> lookupTable;
        private IVerbalAI source;

        private FailedOutcome FailureReport { get; set; }

        private event EventHandler Fulfilled;

        private bool Valid { get; set; } = true;

        public AutoFailAction AutoFail()
        {
            if (FailureReport == null)
            {
                return null;
            }
            return new AutoFailAction(source.GetActor(), FailureReport);
        }

        public RequirementManager(Dictionary<string, string> lookupTable, IVerbalAI source)
        {
            this.lookupTable = lookupTable;
            this.source = source;
        }

        public Hypothetical<T> Require<T>(string token)
        {
            if (!Valid)
            {
                return null;
            }
            bool success = lookupTable.TryGetValue(token, out string name);
            if (!success)
            {
                throw new InvalidOperationException($"Token {token} not provided in lookup table.");
            }
            var hypothetical = new Hypothetical<T>
            {
                Name = name
            };
            source.GetPossibilities(hypothetical);
            if (hypothetical.PossibleValues.Count <= 0)
            {
                Valid = false;
                FailureReport = new FailedOutcome($"There is no \"{hypothetical.Name}\".");
                return null;
            }
            void onFulfilled(object obj, EventArgs eventArgs)
            {
                if (Valid)
                {
                    source.ChooseValue(hypothetical);
                    if (hypothetical.Value == null)
                    {
                        Valid = false;
                    }
                }
            }
            Fulfilled += onFulfilled;
            return hypothetical;
        }

        public bool Fulfill()
        {
            if (Valid)
            {
                Fulfilled(this, new EventArgs());
                return Valid;
            }
            return false;
        }
    }
}