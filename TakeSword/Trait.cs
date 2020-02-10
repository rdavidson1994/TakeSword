using System;

namespace TakeSword
{
    public class Trait
    {
        public static Type DirectSubtype<T>() where T : Trait
        {
            Type currentType = typeof(T);
            if (currentType == typeof(Trait))
            {
                throw new NotSupportedException();
            }
            Type nextType = currentType;
            while (nextType != typeof(Trait))
            {
                currentType = nextType;
                nextType = currentType.BaseType;
            }
            return currentType;
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