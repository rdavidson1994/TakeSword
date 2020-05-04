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

    public class StationaryEnemyAI : ActivityRoutine<PhysicalActor>
    {
        public StationaryEnemyAI(PhysicalActor actor)
        {
            Actor = actor;
        }
        public override IActivity<PhysicalActor> NextActivity()
        {
            foreach (GameObject thing in Actor.ItemsInReach())
            {
                if (thing.Is<Player>())
                {
                    return Do<UnarmedStrike>(thing);
                }
            }
            return Do<WaitAction>();
        }
    }

    public class PlayerCharacterAI : ActivityRoutine<PhysicalActor>, IVerbalAI<PhysicalActor>
    {
        private List<FormattableString> messageQueue = new List<FormattableString>();
        private IUserInterface userInterface;
        private string storedInput;
        private List<Verb> verbs = new List<Verb>();
        public PlayerCharacterAI(PhysicalActor actor, IUserInterface userInterface)
        {
            Actor = actor;
            this.userInterface = userInterface;
        }

        public override void RecieveTextMessage(FormattableString text)
        {
            // Save for later, when next we ask the user for input.
            // This way you see "You hit the orc. The orc dies." and not the other way around.
            if (messagesSuspended)
            {
                messageQueue.Add(text);
            }
            else
            {
                userInterface.PrintOutput(text);
            }
        }

        private bool messagesSuspended = true;

        public override void ReactToAnnouncement(ActionAnnouncement announcement)
        {
            // This method prints output IMMEDIATELY, rather than adding to the message queue.
            // This way you see "You hit the orc. The orc dies." and not the other way around.
            base.ReactToAnnouncement(announcement);
            if (announcement.IsSuccessful(out PhysicalAction physicalAction, TargetType.Witness))
            {
                if (physicalAction.Quiet && physicalAction.Actor == this.Actor)
                {
                    // Do nothing - we don't need to know about our own insignificant actions.
                }
                else
                {
                    userInterface.PrintOutput(physicalAction.AnnouncementText(Actor));
                }
            }
            if (
                announcement.Relationship == TargetType.Actor
                && announcement.Outcome is FailedOutcome failure
            )
            {
                userInterface.PrintOutput(failure.Reason);
            }
        }
        public void AddVerbs(params Verb[] verbs)
        {
            this.verbs.AddRange(verbs);
        }

        private string GetUserInput()
        {
            PrintMessages();
            return userInterface.GetInput();
        }

        private void PrintMessages()
        {
            foreach (FormattableString message in messageQueue)
            {
                userInterface.PrintOutput(message);
            }
            messageQueue.Clear();
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
                    input = GetUserInput();
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
                    userInterface.PrintOutput($"{letter}.) {candidate.GetName(Actor)}");
                    letter++;
                }
                //letter now holds the highest valid response
                name = GetUserInput();
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