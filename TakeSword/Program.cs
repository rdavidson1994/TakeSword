using System;

namespace TakeSword
{
    public static class BanditFactory
    {
        static FrozenTraitStore banditTraits = new TraitStore() {

        }.Freeze();

        static FrozenTraitStore swordTraits = new TraitStore()
        {
            new Weapon(
                skillType: SkillType.Sword,
                damageMultiplier: 4.0,
                damageType: DamageType.Slashing
            )
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
            GameObject place2 = new GameObject(schedule);
            Portal door = new Portal(Direction.North, place)
            {
                StringName = "northern door"
            };
            new Portal(door, place2)
            {
                StringName = "southern door"
            };
            GameObject sword = new GameObject(place)
            {
                StringName = "iron sword"
            };
            GameObject sword2 = new GameObject(place)
            {
                StringName = "bronze sword"
            };
            var apple = new GameObject(place)
            {
                StringName = "apple",
                Traits = new TraitStore()
                {
                    new Food(300),
                    new InventoryItem(200)
                }
            };
            //apple.AddTrait(new Food { Nutrition = 300 });
            //apple.AddTrait(new ItemTrait { Weight = 200 });
            sword.AddTrait(new InventoryItem ( weight : 1500 ));
            sword2.Name = new SimpleName("sword");
            sword2.AddTrait(new InventoryItem(weight: 1500));
            PhysicalActor player = new PhysicalActor(place);
            var formatter = new ConsoleOutputFormatter();
            IUserInterface userInterface = new ConsoleUserInterface(formatter);
            PlayerCharacterAI playerAI = new PlayerCharacterAI(player, userInterface);
            formatter.VerbalAI = playerAI;
            playerAI.AddVerbs(
                new TargetVerb<Eat>("eat"),
                new SimpleVerb<ActOnAll<Take>>("take all"),
                new TargetVerb<Take>("take", "get", "pick up"),
                new SimpleVerb<ActOnAll<Drop>>("drop all"),
                new TargetVerb<Drop>("put down", "drop"),
                new ToolVerb<WeaponStrike>("hit", "attack", "strike"),
                new SimpleVerb<WaitAction>("wait","delay"),
                new TargetVerb<Enter>("enter"),
                new DirectionVerb<GoDirection>("go"),
                new DirectionVerb<GoDirection>()
            );
            player.AI = playerAI;
            player.Act();
            schedule.RunFor(1000000);
        }
    }
}
