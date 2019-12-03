using System;

namespace TakeSword
{
    public static class BanditFactory
    {
        static FrozenTraitStore banditTraits = new LiveTraitStore() {

        }.Freeze();

        static FrozenTraitStore swordTraits = new LiveTraitStore()
        {
            new Weapon()
            {
                DamageType = DamageType.Slashing,
                DamageMultiplier = 4.0
            }
        }.Freeze();

        public static PhysicalActor MakeBandit()
        {
            PhysicalActor bandit = new PhysicalActor(traits: banditTraits);
            new GameObject(traits: swordTraits, location: bandit);
            return bandit;
        }
    }
    public class MessageEvent : IEvent
    {
        private readonly string message;
        public bool Instant { get; set; }
        public MessageEvent(string message)
        {
            this.message = message;
        }
        public void Happen()
        {
            Console.WriteLine(message);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Schedule schedule = new Schedule();
            GameObject place = new GameObject(schedule);
            GameObject sword = new GameObject(place);
            GameObject sword2 = new GameObject(place);
            GameObject apple = new GameObject(place);
            apple.Name = new SimpleName("apple");
            sword.Name = new SimpleName("sword");
            sword2.Name = new SimpleName("sword");
            PhysicalActor player = new PhysicalActor(place);
            IUserInterface userInterface = new ConsoleUserInterface(new ConsoleOutputFormatter());
            PlayerCharacterAI playerAI = new PlayerCharacterAI(player, userInterface);
            playerAI.AddVerbs(
                new TargetVerb<Take>("take", "get", "pick up"),
                new TargetVerb<Drop>("put down", "drop"),
                new ToolVerb<WeaponStrike>("hit", "attack", "strike")
            );
            player.AI = playerAI;
            player.Act();
            schedule.RunFor(1000000);
        }
    }
}
