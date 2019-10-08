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
            GameObject sword = new GameObject(traits: swordTraits, location: bandit);
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
            schedule.Add(new MessageEvent("world"), 200);
            schedule.Add(new MessageEvent("!"), 300);
            schedule.Add(new MessageEvent("hello"), 100);
            schedule.RunOnce();
            schedule.RunOnce();
        }
    }
}
