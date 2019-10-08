using System;

namespace TakeSword
{
    public static class TraitConverter
    {
        public static Type DirectTraitType<T>() where T : Trait
        {
            Type prevType = typeof(T);
            Type nextType = prevType;
            while (nextType != typeof(Trait))
            {
                prevType = nextType;
                nextType = prevType.BaseType;
            }
            return prevType;
        }
    }
    public class Trait
    {
        public static Type DirectSubtype<T>() where T : Trait
        {
            Type prevType = typeof(T);
            Type nextType = prevType;
            while (nextType != typeof(Trait))
            {
                prevType = nextType;
                nextType = prevType.BaseType;
            }
            return prevType;
        }
        public Trait Copy()
        {
            return (Trait)MemberwiseClone();
        }
    }

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
        public double DamageMultiplier { get; set; }
        public DamageType DamageType { get; set; }
    }
}