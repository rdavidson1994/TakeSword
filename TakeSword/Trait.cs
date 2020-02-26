using System;
using System.Collections.Generic;

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

    public enum SkillType
    {
        MiscWeapon,
        Sword,
        Mace,
        // ...more tbd
    }

    public class PhysicalStats : Trait
    {
        public float Strength { get; set; }

        public float Agility { get; set; }

        public float Toughness { get; set; }

        public float Dexterity { get; set; }
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
        public SkillType SkillType { get; set; }
        public double DamageMultiplier { get; set; }
        public DamageType DamageType { get; set; }
    }
}