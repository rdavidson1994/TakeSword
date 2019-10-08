using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
        bool RemoveTrait<T>() where T : Trait;
    }
    public class FrozenTraitStore : ITraitStore
    {
        static FrozenTraitStore empty;
        private LiveTraitStore innerStore;
        private FrozenTraitStore(LiveTraitStore traitStore, bool createCopy)
        {
            if (createCopy)
            {
                traitStore = traitStore.Copy();
            }
            innerStore = traitStore;
        }

        public FrozenTraitStore(LiveTraitStore traitStore) : this(traitStore, true) { }

        public LiveTraitStore LiveCopy()
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
                empty = new FrozenTraitStore(new LiveTraitStore(), false);
            }
            return empty;
        }
    }
    public class LiveTraitStore : IWritableTraitStore, IEnumerable<Trait>
    {
        private Dictionary<Type, Trait> data;
        private List<Type> iterationOrder;
        public LiveTraitStore()
        {
            data = new Dictionary<Type, Trait>();
            iterationOrder = new List<Type>();
        }
        public LiveTraitStore Copy()
        {
            LiveTraitStore copy = new LiveTraitStore();
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
            Type directSubtypeOfTrait = Trait.DirectSubtype<T>();
            iterationOrder.Add(directSubtypeOfTrait);
            data.Add(directSubtypeOfTrait, newTrait);
        }

        public bool RemoveTrait<T>() where T : Trait
        {
            Type directSubtypeOfTrait = Trait.DirectSubtype<T>();
            bool removed = data.Remove(directSubtypeOfTrait);
            if (removed)
                iterationOrder.Remove(directSubtypeOfTrait);
            return removed;
        }

        public void Test()
        {
            Add(new Food()
            {
                Nutrition = 40,
            });

            Add(new Weapon()
            {
                DamageMultiplier = 3.0,
                DamageType = DamageType.Blunt
            });
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
        private LiveTraitStore liveStore;
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

        public bool RemoveTrait<T>() where T : Trait
        {
            CreateLiveStoreIfNeeded();
            return liveStore.RemoveTrait<T>();
        }
    }
}
