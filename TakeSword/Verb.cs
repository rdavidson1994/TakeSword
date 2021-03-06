﻿using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public abstract class Verb
    {
        protected IParser Parser { get; set; }
        public Verb(string pattern, params string[] synonyms)
        {
            Parser = new ParserBuilder()
                .AddMacro("VERB", synonyms)
                .CreateParser(pattern);
        }


        public IActivity<PhysicalActor>? Interpret(IVerbalAI<PhysicalActor> ai, string input)
        {
            var lookupTable = Parser.Match(input);
            if (lookupTable == null)
            {
                return null;
            }
            return BuildActivity(ai, lookupTable);
        }

        protected abstract IActivity<PhysicalActor>? BuildActivity(IVerbalAI<PhysicalActor> ai, Dictionary<string, string> lookup);
    }

    public class SimpleVerb<TAction> : Verb where TAction : ISimpleActivity<PhysicalActor>, new()
    {
        public SimpleVerb(params string[] synonyms) : base("VERB", synonyms)
        {
        }

        protected override IActivity<PhysicalActor> BuildActivity(IVerbalAI<PhysicalActor> ai, Dictionary<string, string> lookup)
        {
            return new TAction { Actor = ai.Actor };
        }
    }

    public class TargetVerb<TAction> : Verb where TAction: ITargetedActivity<PhysicalActor>, new()
    {
        public TargetVerb(params string[] synonyms) : base("VERB TARGET", synonyms) { }

        protected override IActivity<PhysicalActor>? BuildActivity(IVerbalAI<PhysicalActor> ai, Dictionary<string, string> lookup)
        {
            List<GameObject> targets = ai.ObjectsWithName(lookup["TARGET"]).ToList();
            if (targets.Count == 0)
            {
                return new AutoFailAction<PhysicalActor>(
                    ai.Actor,
                    new FailedOutcome($"There is no {lookup["TARGET"]} here")
                );
            }
            List<TAction>? activities = null;
            List<TAction>? validActivities = null;
            while (validActivities == null || validActivities.Count > 1)
            {
                IEnumerable<TAction> query =
                    from target in targets
                    select new TAction
                    {
                        Actor = ai.Actor,
                        Target = target,
                    };
                activities = new List<TAction>(query);
                validActivities = activities.Where(x => x.IsValid()).ToList();
                IEnumerable<GameObject> validTargets = from act in validActivities select act.Target;
                var uniqueTargets = new HashSet<GameObject>(validTargets);
                if (uniqueTargets.Count > 1)
                {
                    GameObject? definiteTarget = ai.ChooseObject(lookup["TARGET"], uniqueTargets);
                    if (definiteTarget == null)
                    {
                        targets = new List<GameObject>(); // empty
                    }
                    else
                    {
                        targets = new List<GameObject>() { definiteTarget };
                    }
                }
            }
            // activities cannot be null here, because the above loop only exits if it is non-null
            if (activities!.Count == 0)
            {
                return null;
            }
            else if (validActivities.Count == 0)
            {
                return activities[0];
            }
            else
            {
                return validActivities[0];
            }
        }


    }

    public class ToolVerb<TAction> : Verb where TAction: IToolActivity<PhysicalActor>, new()
    {
        public ToolVerb(params string[] synonyms) : base("VERB TARGET with TOOL", synonyms) { }

        protected override IActivity<PhysicalActor>? BuildActivity(IVerbalAI<PhysicalActor> ai, Dictionary<string, string> lookup)
        {
            List<GameObject> targets = ai.ObjectsWithName(lookup["TARGET"]).ToList();
            if (targets.Count == 0)
                return new AutoFailAction<PhysicalActor>(
                    ai.Actor,
                    new FailedOutcome($"There is no {lookup["TARGET"]} here.")
                );

            List<GameObject> tools = ai.ObjectsWithName(lookup["TOOL"]).ToList();
            if (tools.Count == 0)
                return new AutoFailAction<PhysicalActor>(
                    ai.Actor,
                    new FailedOutcome($"There is no {lookup["TOOL"]} here.")
                );
            List<TAction>? activities = null;
            List<TAction>? validActivities = null;
            while (validActivities == null || validActivities.Count > 1)
            {
                IEnumerable<TAction> query =
                    from target in targets
                    from tool in tools
                    select new TAction
                    {
                        Actor = ai.Actor,
                        Target = target,
                        Tool = tool,
                    };
                activities = new List<TAction>(query);
                validActivities = activities.Where(x => x.IsValid()).ToList();
                IEnumerable<GameObject> validTargets = from act in validActivities select act.Target;
                var uniqueTargets = new HashSet<GameObject>(validTargets);
                if (uniqueTargets.Count > 1)
                {
                    GameObject? definiteTarget = ai.ChooseObject(lookup["TARGET"], uniqueTargets);
                    if (definiteTarget == null)
                    {
                        targets = new List<GameObject>(); // empty
                    }
                    else
                    {
                        targets = new List<GameObject>() { definiteTarget };
                    }
                }
                else
                {
                    IEnumerable<GameObject> validTools = from act in validActivities select act.Tool;
                    var uniqueTools = new HashSet<GameObject>(validTools);
                    if (uniqueTools.Count > 1)
                    {
                        GameObject? definiteTool = ai.ChooseObject(lookup["TOOL"], uniqueTools);
                        if (definiteTool == null)
                        {
                            tools = new List<GameObject>(); // empty
                        }
                        else
                        {
                            tools = new List<GameObject>() { definiteTool };
                        }
                    }
                }
            }
            // activities cannot be null here, because the above loop only exits if it is non-null
            if (activities!.Count == 0)
            {
                return null;
            }
            else if (validActivities.Count == 0)
            {
                return activities[0];
            }
            else
            {
                return validActivities[0];
            }
        }
    }

    public class DirectionVerb<ActionType> : Verb where ActionType : IDirectionActivity, new()
    {
        //public static DirectionVerb<ActionType> DirectionWithImplicitVerb()
        public DirectionVerb() : base("DIRECTION") { }
        public DirectionVerb(params string[] synonyms) : base("VERB DIRECTION", synonyms) { }

        protected override IActivity<PhysicalActor>? BuildActivity(IVerbalAI<PhysicalActor> ai, Dictionary<string, string> lookup)
        {
            var direction = DirectionConverter.FromString(lookup["DIRECTION"]);
            if (direction == Direction.None)
            {
                return null;
                //return new AutoFailAction(
                //    ai.GetActor(),
                //    new FailedOutcome($"{lookup["DIRECTION"]} is not a valid direction")
                //);
            }
            return new ActionType()
            {
                Actor = ai.Actor,
                Direction = direction
            };
        }
    }
}