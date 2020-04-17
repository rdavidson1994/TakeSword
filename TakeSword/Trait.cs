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
        public PhysicalStats(float strength, float agility, float toughness, float dexterity)
        {
            Strength = strength;
            Agility = agility;
            Toughness = toughness;
            Dexterity = dexterity;
        }

        public float Strength { get; }

        public float Agility { get; }

        public float Toughness { get; }

        public float Dexterity { get; }
    }

    public class InventoryItem : Trait
    {
        public InventoryItem(int weight)
        {
            Weight = weight;
        }

        // in grams
        public int Weight { get; }
    }

    public class Player : Trait { }

    public class Food : Trait
    {
        public Food(int nutrition)
        {
            Nutrition = nutrition;
        }

        public int Nutrition { get; }
    }

    public class Weapon : Trait
    {
        public Weapon(SkillType skillType, double damageMultiplier, DamageType damageType)
        {
            SkillType = skillType;
            DamageMultiplier = damageMultiplier;
            DamageType = damageType;
        }

        public SkillType SkillType { get; }
        public double DamageMultiplier { get; }
        public DamageType DamageType { get; }
    }
}