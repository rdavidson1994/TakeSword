using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public class GameObject : ILocation, IGameObject
    {
        protected readonly HashSet<GameObject> contents;
        public IName Name { get; set; }
        public IReadOnlyCollection<GameObject> Contents { get { return contents; } }
        public ILocation Location { get; protected set; }
        public virtual IReadOnlyCollection<Trait> InitialTraits { get; set; } = new HashSet<Trait>();
        protected IWritableTraitStore Traits;
        public ISchedule Schedule { get; protected set; }
        private HashSet<Trait> instanceTraits;

        public GameObject(ILocation location=null, FrozenTraitStore traits=null)
        {
            if (location == null)
                location = new OffscreenLocation();
            if (traits == null)
                traits = FrozenTraitStore.Empty();
            Traits = new MirrorTraitStore(traits);
            Location = location;
            Location.BeEntered(this);
            Schedule = Location.Schedule;
            contents = new HashSet<GameObject>();
        }

        public TraitType As<TraitType>() where TraitType: Trait
        {
            return Traits.Get<TraitType>();
        }

        public string GetName(IGameObject viewer)
        {
            if (Name != null)
            {
                return Name.GetText(viewer);
            }
            else
            {
                return "unnamed";
            }
        }

        public bool AddTrait<T>(T trait) where T : Trait
        {
            Traits.Add(trait);
            return true;
        }

        public void Move(ILocation location)
        {
            Location.BeExited(this);
            Location = location;
            Schedule = Location.Schedule;
            Location.BeEntered(this);
        }

        //public bool RemoveTrait(Trait trait)
        //{
        //    if (instanceTraits == null)
        //    {
        //        instanceTraits = new HashSet<Trait>(InitialTraits);
        //    }
        //    return instanceTraits.Remove(trait);
        //}

        public bool BeEntered(GameObject gameObject)
        {
            if (contents.Contains(gameObject))
            {
                return false;
            }
            contents.Add(gameObject);
            return true;
        }

        public bool BeExited(GameObject gameObject) => contents.Remove(gameObject);

        protected void BroadcastAnnouncement(object announcement)
        {
            foreach (GameObject gameObject in contents)
            {
                gameObject.HandleAnnouncement(announcement);
            }
        }

        protected virtual void ReactToAnnouncement(object announcement)
        {
            // do nothing
        }

        public void HandleAnnouncement(object announcement)
        {
            ReactToAnnouncement(announcement);
            BroadcastAnnouncement(announcement);
        }

        public IEnumerable<GameObject> NearbyObjects(long range)
        {
            return contents;
        }

        public bool RemoveTrait<T>() where T : Trait
        {
            return Traits.RemoveTrait<T>();
        }
    }
}
