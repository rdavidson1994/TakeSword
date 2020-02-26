using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public interface IUserInterface
    {
        bool UpdateMap(string mapData);
        bool PrintOutput(FormattableString output);
        string GetInput();
    }

    public class ConsoleUserInterface : IUserInterface
    {
        public IGameOutputFormatter Formatter { get; }

        public ConsoleUserInterface(IGameOutputFormatter formatter)
        {
            Formatter = formatter;
        }
        public bool UpdateMap(string mapData)
        {
            Console.WriteLine(mapData);
            return true;
        }

        public bool PrintOutput(FormattableString output)
        {
            string plainTextOutput = Formatter.FormatString(output);
            Console.WriteLine(plainTextOutput);
            return true;
        }
        public string GetInput()
        {
            return Console.ReadLine();
        }
    }

    public class PlayerCharacterAI : ActivityRoutine<PhysicalActor>, IVerbalAI<PhysicalActor>
    {
        private IUserInterface userInterface;
        private string storedInput;
        private List<Verb> verbs = new List<Verb>();
        public PlayerCharacterAI(PhysicalActor actor, IUserInterface userInterface)
        {
            Actor = actor;
            this.userInterface = userInterface;
        }

        public override void ReactToAnnouncement(ActionAnnouncement announcement)
        {
            if (announcement.Is(out PhysicalAction physicalAction, TargetType.Bystander))
            {
                userInterface.PrintOutput(physicalAction.AnnouncementText(Actor));
            }
            if (announcement.Is(out PhysicalAction failedPhysicalAction, TargetType.Bystander, successful: false))
            {
                //userInterface.PrintOutput();
            }
        }
        public void AddVerbs(params Verb[] verbs)
        {
            this.verbs.AddRange(verbs);
        }

        public override IActivity<PhysicalActor> NextActivity()
        {
            while (true)
            {
                string input;
                if (storedInput != null)
                {
                    input = storedInput;
                    storedInput = null;
                }
                else
                {
                    input = userInterface.GetInput();
                }

                FailedOutcome backupOutcome = null;
                foreach (Verb verb in verbs)
                {
                    var activity = verb.Interpret(this, input);
                    if (activity != null)
                    {
                        if (activity.IsValid() is FailedOutcome failure)
                        {
                            if (backupOutcome == null)
                            {
                                backupOutcome = failure;
                            }
                        }
                        else
                        {
                            return activity;
                        }
                    }
                }
                if (storedInput != null)
                {
                    continue;
                }
                if (backupOutcome != null)
                {
                    userInterface.PrintOutput(backupOutcome.Reason);
                    continue;
                }
                userInterface.PrintOutput($"I don't understand the verb of that sentence.");
            }
        }

        public GameObject ChooseObject(string name, IEnumerable<GameObject> objects)
        {
            var candidates = objects.ToList();
            while (candidates.Count > 1)
            {
                userInterface.PrintOutput($"There are multiple things called '{name}'. Which did you mean?");
                char letter = 'a';
                foreach (GameObject candidate in candidates)
                {
                    Console.WriteLine($"{letter}.) {candidate.GetName(Actor)}");
                    letter++;
                }
                //letter now holds the highest valid response
                name = userInterface.GetInput();
                if (name.Length == 1)
                {
                    char letterChoice = char.ToUpper(name[0]);
                    if ('A' <= letterChoice && letterChoice < letter)
                    {
                        int index = letterChoice - 'A';
                        return candidates[index];
                    }
                }
                candidates = candidates.Where(x => x.HasName(Actor, name)).ToList();
            }
            if (candidates.Count == 0)
            {
                //If the players response doesn't match any option,
                //It may be an unrelated command - save it for later evaluation
                storedInput = name;
                return null;
            }
            return candidates[0];
        }

        public IEnumerable<GameObject> ObjectsWithName(string name) => Actor.TargetsByName(name);
    }
}