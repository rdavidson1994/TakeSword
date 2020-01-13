using System.Collections.Generic;
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


        public IActivity Interpret(IVerbalAI ai, string input)
        {
            var lookupTable = Parser.Match(input);
            if (lookupTable == null)
            {
                return null;
            }
            return BuildActivity(ai, lookupTable);
        }

        protected abstract IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup);
    }

    public class SimpleVerb<ActionType> : Verb where ActionType : IPhysicalActivity, new()
    {
        public SimpleVerb(params string[] synonyms) : base("VERB", synonyms)
        {
        }

        protected override IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup)
        {
            return new ActionType { Actor = ai.GetActor() };
        }
    }

    public class TargetVerb<ActionType> : Verb where ActionType: ITargetedActivity, new()
    {
        public TargetVerb(params string[] synonyms) : base("VERB TARGET", synonyms) { }

        protected override IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup)
        {
            IEnumerable<GameObject> targets = ai.ObjectsWithName(lookup["TARGET"]);
            List<ActionType> activities = null;
            List<ActionType> validActivities = null;
            while (validActivities == null || validActivities.Count > 1)
            {
                IEnumerable<ActionType> query =
                    from target in targets
                    select new ActionType
                    {
                        Actor = ai.GetActor(),
                        Target = target,
                    };
                activities = new List<ActionType>(query);
                validActivities = activities.Where(x => x.IsValid()).ToList();
                IEnumerable<GameObject> validTargets = from act in validActivities select act.Target;
                var uniqueTargets = new HashSet<GameObject>(validTargets);
                if (uniqueTargets.Count > 1)
                {
                    GameObject definiteTarget = ai.ChooseObject(lookup["TARGET"], uniqueTargets);
                    targets = new List<GameObject>() { definiteTarget };
                }
            }
            if (activities.Count == 0)
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

    public class ToolVerb<ActionType> : Verb where ActionType: IToolActivity, new()
    {
        public ToolVerb(params string[] synonyms) : base("VERB TARGET with TOOL", synonyms) { }

        protected override IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup)
        {
            IEnumerable<GameObject> targets = ai.ObjectsWithName(lookup["TARGET"]);
            IEnumerable<GameObject> tools = ai.ObjectsWithName(lookup["TOOL"]);
            List<ActionType> activities = null;
            List<ActionType> validActivities = null;
            while (validActivities == null || validActivities.Count > 1)
            {
                IEnumerable<ActionType> query =
                    from target in targets
                    from tool in tools
                    select new ActionType
                    {
                        Actor = ai.GetActor(),
                        Target = target,
                        Tool = tool,
                    };
                activities = new List<ActionType>(query);
                validActivities = activities.Where(x => x.IsValid()).ToList();
                IEnumerable<GameObject> validTargets = from act in validActivities select act.Target;
                var uniqueTargets = new HashSet<GameObject>(validTargets);
                if (uniqueTargets.Count > 1)
                {
                    GameObject definiteTarget = ai.ChooseObject(lookup["TARGET"], uniqueTargets);
                    targets = new List<GameObject>() { definiteTarget };
                }
                else
                {
                    IEnumerable<GameObject> validTools = from act in validActivities select act.Tool;
                    var uniqueTools = new HashSet<GameObject>(validTargets);
                    if (uniqueTools.Count > 1)
                    {
                        GameObject definiteTool = ai.ChooseObject(lookup["TOOL"], uniqueTools);
                        tools = new List<GameObject>() { definiteTool };
                    }
                }
            }
            if (activities.Count == 0)
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
}