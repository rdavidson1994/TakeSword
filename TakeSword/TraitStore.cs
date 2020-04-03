using System;
using System.Collections;
using System.Collections.Generic;

namespace TakeSword
{
    public interface ITraitStore
    {
        T Get<T>() where T : Trait;
        FrozenTraitStore Freeze();
    }

    public interface IWritableTraitStore : ITraitStore
    {
        void Add<T>(T newTrait) where T : Trait;
        bool Remove<T>() where T : Trait;
    }
    public class FrozenTraitStore : ITraitStore
    {
        static FrozenTraitStore Create(params Trait[] traits)
        {
            var traitStore = new TraitStore();
            foreach (var trait in traits)
            {
                traitStore.Add(trait);
            }
            return traitStore.Freeze();
        }
        static FrozenTraitStore empty;
        private TraitStore innerStore;
        private FrozenTraitStore(TraitStore traitStore, bool createCopy)
        {
            if (createCopy)
            {
                traitStore = traitStore.Copy();
            }
            innerStore = traitStore;
        }

        public FrozenTraitStore(TraitStore traitStore) : this(traitStore, true) { }

        public TraitStore LiveCopy()
        {
            return innerStore.Copy();
        }

        public FrozenTraitStore Freeze()
        {
            return this;
        }

        public T Get<T>() where T : Trait
        {
            return innerStore.Get<T>();
        }

        public static FrozenTraitStore Empty()
        {
            if (empty == null)
            {
                empty = new FrozenTraitStore(new TraitStore(), false);
            }
            return empty;
        }
    }
    public class TraitStore : IWritableTraitStore, IEnumerable<Trait>
    {
        private Dictionary<Type, Trait> data;
        private List<Type> iterationOrder;
        public TraitStore()
        {
            data = new Dictionary<Type, Trait>();
            iterationOrder = new List<Type>();
        }
        public TraitStore Copy()
        {
            TraitStore copy = new TraitStore();
            foreach (Type type in iterationOrder)
            {
                copy.data.Add(type, data[type].Copy());
            }
            return copy;
        }

        public FrozenTraitStore Freeze()
        {
            return new FrozenTraitStore(this);
        }

        public T Get<T>() where T : Trait
        {
            Type directTraitSubtype = Trait.DirectSubtype<T>();
            bool success = data.TryGetValue(directTraitSubtype, out Trait outTrait);
            if (success)
                return (T)outTrait;
            return null;
        }

        public void Add<T>(T newTrait) where T : Trait
        {
            if (typeof(T) == typeof(Trait))
            {
                throw new NotSupportedException();
            }
            Type directSubtypeOfTrait = Trait.DirectSubtype<T>();
            iterationOrder.Add(directSubtypeOfTrait);
            data[directSubtypeOfTrait] = newTrait;
        }

        public bool Remove<T>() where T : Trait
        {
            Type directSubtypeOfTrait = Trait.DirectSubtype<T>();
            bool removed = data.Remove(directSubtypeOfTrait);
            if (removed)
                iterationOrder.Remove(directSubtypeOfTrait);
            return removed;
        }

        public void Test()
        {
            Add(new Food(nutrition: 40));

            Add(new Weapon(
                skillType: SkillType.Sword,
                damageMultiplier: 3.0,
                damageType: DamageType.Blunt
            ));
        }

        public IEnumerator<Trait> GetEnumerator()
        {
            foreach (Type type in iterationOrder)
            {
                yield return data[type];
            }
            //foreach (var (type, trait) in data)
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class MirrorTraitStore : IWritableTraitStore
    {
        private FrozenTraitStore baseStore;
        private TraitStore liveStore;
        private ITraitStore activeStore;
        public MirrorTraitStore(FrozenTraitStore basis)
        {
            this.baseStore = basis;
            this.activeStore = basis;
        }
        private void CreateLiveStoreIfNeeded()
        {
            if (activeStore == baseStore)
            {
                liveStore = baseStore.LiveCopy();
                activeStore = liveStore;
            }
        }
        public void Add<T>(T newTrait) where T : Trait
        {
            CreateLiveStoreIfNeeded();
            liveStore.Add<T>(newTrait);
        }

        public FrozenTraitStore Freeze()
        {
            // this does not copy if activeStore is a FrozenTraitStore. :)
            return activeStore.Freeze();
        }

        public T Get<T>() where T : Trait
        {
            return activeStore.Get<T>();
        }

        public bool Remove<T>() where T : Trait
        {
            CreateLiveStoreIfNeeded();
            return liveStore.Remove<T>();
        }
    }
}
