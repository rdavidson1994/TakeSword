namespace TakeSword
{
    public class Trait { }

    public class ItemTrait : Trait
    {
        // in grams
        public int Weight { get; set; }
    }

    public class Food : Trait
    {
        public int Nutrition { get; set; }
    }

    public class Weapon : Trait
    {
        public int DamageMultiplier { get; set; }
        public DamageType DamageType { get; set; }
    }
}