using System.Collections.Generic;

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

        public IActivity BuildActivity(IVerbalAI ai, string input)
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

    public class SimpleVerb<ActionType> : Verb where ActionType : PhysicalAction, new()
    {
        public SimpleVerb(params string[] synonyms) : base("VERB", synonyms)
        {
        }

        protected override IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup)
        {
            return new ActionType { Actor = ai.GetActor() };
        }
    }

    public class TargetVerb<ActionType> : Verb where ActionType: TargetedAction, new()
    {
        public TargetVerb(params string[] synonyms) : base("VERB TARGET", synonyms) { }

        protected override IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup)
        {
            var manager = new RequirementManager(lookup, ai);
            var target = manager.Require<GameObject>("TARGET");
            if (manager.Fulfill())
            {
                return new ActionType
                {
                    Actor = ai.GetActor(),
                    Target = target.Value,
                };
            }
            return manager.AutoFail();
        }
    }

    public class ToolVerb<ActionType> : Verb where ActionType: ToolAction, new()
    {
        public ToolVerb(params string[] synonyms) : base("VERB TARGET with TOOL", synonyms) { }

        protected override IActivity BuildActivity(IVerbalAI ai, Dictionary<string, string> lookup)
        {
            var manager = new RequirementManager(lookup, ai);
            var target = manager.Require<GameObject>("TARGET");
            var tool = manager.Require<GameObject>("TOOL");
            if (manager.Fulfill())
            {
                return new ActionType
                {
                    Actor = ai.GetActor(),
                    Target = target.Value,
                    Tool = tool.Value,
                };
            }
            return manager.AutoFail();
        }
    }


    public static class Test
    {
        public static void DoTest()
        {
            PhysicalActor player = new PhysicalActor();
            PlayerCharacterAI playerAI = new PlayerCharacterAI(player, new ConsoleUserInterface(new ConsoleOutputFormatter()));
            playerAI.AddVerbs(new Verb[]
            {
                new TargetVerb<Take>("take", "get", "pick up"),
                new TargetVerb<Drop>("put down", "drop"),
                new ToolVerb<WeaponStrike>("hit", "attack", "strike"),
            });
            player.AI = playerAI;
        }
    }
}