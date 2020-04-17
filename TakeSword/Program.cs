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
            PhysicalActor bandit = new PhysicalActor(
                body: new BasicBody(),
                traits: banditTraits
            );
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
            FrozenTraitStore swordTraits = new TraitStore
            {
                new Weapon(
                    SkillType.Sword,
                    damageMultiplier: 2.0,
                    DamageType.Slashing
                ),
                new InventoryItem(weight: 1500)
            }.Freeze();
            GameObject sword = new GameObject(place, swordTraits)
            {
                StringName = "iron sword"
            };
            GameObject sword2 = new GameObject(place, swordTraits)
            {
                StringName = "bronze sword"
            };
            var apple = new GameObject(place)
            {
                StringName = "apple",
                Traits = new TraitStore()
                {
                    new Food(nutrition: 300),
                    new InventoryItem(weight: 200)
                }
            };
            //apple.AddTrait(new Food { Nutrition = 300 });
            //apple.AddTrait(new ItemTrait { Weight = 200 });
            sword.AddTrait(new InventoryItem ( weight : 1500 ));
            sword2.AddTrait(new InventoryItem(weight: 1500));
            PhysicalActor player = new PhysicalActor(new BasicBody(), place)
            {
                StringName = "myself"
            };
            player.AddTrait(new Player());
            var formatter = new ConsoleOutputFormatter();
            IUserInterface userInterface = new ConsoleUserInterface(formatter);
            PlayerCharacterAI playerAI = new PlayerCharacterAI(player, userInterface);
            formatter.VerbalAI = playerAI;
            playerAI.AddVerbs(
                new SimpleVerb<DisplayInventory>("inventory","i"),
                new SimpleVerb<Look>("look"),
                new TargetVerb<Eat>("eat"),
                new SimpleVerb<ActOnAll<Take>>("take all"),
                new TargetVerb<Take>("take", "get", "pick up"),
                new SimpleVerb<ActOnAll<Drop>>("drop all"),
                new TargetVerb<Drop>("put down", "drop"),
                new ToolVerb<WeaponStrike>("hit", "attack", "strike"),
                new TargetVerb<UnarmedStrike>("hit", "attack", "strike"),
                new SimpleVerb<WaitAction>("wait", "delay"),
                new TargetVerb<Enter>("enter"),
                new DirectionVerb<GoDirection>("go"),
                new DirectionVerb<GoDirection>() // To handle inputs like "north"
            );
            player.AI = playerAI;
            player.Act();

            PhysicalActor enemy = new PhysicalActor(new BasicBody(), place2)
            {
                StringName = "enemy"
            };
            enemy.AI = new StationaryEnemyAI(enemy);
            enemy.Act();
            schedule.RunFor(1000000);
        }
    }
}
