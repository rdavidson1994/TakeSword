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

    public class PlayerCharacterAI : ActivityRoutine, IVerbalAI
    {
        protected PhysicalActor Actor { get; set; }

        private IUserInterface userInterface;
        private string storedInput;

        public PlayerCharacterAI(PhysicalActor actor, IUserInterface userInterface)
        {
            Actor = actor;
            this.userInterface = userInterface;
        }

        PhysicalActor IVerbalAI.GetActor()
        {
            return Actor;
        }

        public override void ReactToAnnouncement(object announcement)
        {
            if (
                announcement is ActionAnnouncement actionAnnouncement
                && actionAnnouncement.Activity is PhysicalAction physicalAction
                )
            {
                userInterface.PrintOutput(physicalAction.AnnouncementText(Actor));
            }
        }

        private List<Verb> verbs = new List<Verb>();
        public void AddVerbs(params Verb[] verbs)
        {
            this.verbs.AddRange(verbs);
        }

        public override IActivity NextActivity()
        {
            //userInterface.PrintOutput("Player's turn!");
            while (true)
            {
                //userInterface.PrintOutput("Seeking action.");
                bool failedWithReason = false;
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
                foreach (Verb verb in verbs)
                {
                    IActivity activity = verb.BuildActivity(this, input);
                    if (activity != null)
                    {
                        //userInterface.PrintOutput("Got an action.");
                        if (activity.IsValid() is FailedOutcome failure)
                        {
                            userInterface.PrintOutput(failure.Reason);
                            failedWithReason = true;
                            break;
                        }
                        else
                        {
                            return activity;
                        }
                    }
                }
                if (storedInput == null && !failedWithReason)
                {
                    userInterface.PrintOutput($"I don't understand that.");
                }
            }
        }

        public void GetPossibilities(Hypothetical hypothetical)
        {
            if (hypothetical is Hypothetical<GameObject> hGameObject)
            {
                hGameObject.PossibleValues = new List<GameObject>(Actor.TargetsByName(hGameObject.Name));
            }
            else if (hypothetical is Hypothetical<Direction> hDirection)
            {

                Direction direction = DirectionConverter.FromString(hDirection.Name);
                if (direction != Direction.None)
                {
                    hDirection.PossibleValues = new List<Direction> { direction };
                }
            }
        }

        public void ChooseValue(Hypothetical hypothetical)
        {
            if (hypothetical is Hypothetical<GameObject> hypotheticalObject)
            {
                List<GameObject> candidates = new List<GameObject>(hypotheticalObject.PossibleValues);
                string input = hypotheticalObject.Name;
                while (candidates.Count > 1)
                {
                    userInterface.PrintOutput($"There are multiple matches for {input}. Which did you mean?");
                    char letter = 'a';
                    foreach (GameObject candidate in candidates)
                    {
                        Console.WriteLine($"{letter}.) {candidate.GetName(Actor)}");
                        letter++;
                    }
                    //letter now holds the highest valid response
                    input = userInterface.GetInput();
                    if (input.Length == 1)
                    {
                        char letterChoice = char.ToUpper(input[0]);
                        if ('A' <= letterChoice && letterChoice < letter)
                        {
                            int index = letterChoice - 'A';
                            hypotheticalObject.Value = candidates[index];
                            return;
                        }
                    }
                    candidates = candidates.Where(x => x.HasName(Actor, input)).ToList();
                }
                if (candidates.Count == 0)
                {
                    //If the players response doesn't match any option,
                    //It may be an unrelated command - save it for later evaluation
                    storedInput = input;
                    //Don't set hypotheticalObject.Value, since we could not determine it
                }
                else
                {
                    hypotheticalObject.Value = candidates.FirstOrDefault();
                }
            }
        }

        public override IActor GetActor()
        {
            return Actor;
        }
    }
}