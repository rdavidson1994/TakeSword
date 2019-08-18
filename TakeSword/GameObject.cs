using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeSword
{
    public class GameObject : ILocation
    {
        protected readonly HashSet<GameObject> contents;
        public IReadOnlyCollection<GameObject> Contents { get { return contents; } }
        public ILocation Location { get; protected set; }
        public virtual IReadOnlyCollection<Trait> ClassTraits { get; } = new HashSet<Trait>();
        public ISchedule Schedule { get; protected set; }
        private HashSet<Trait> instanceTraits;

        public GameObject(ISchedule schedule, ILocation location)
        {
            Location = location;
            Location.BeEntered(this);
            contents = new HashSet<GameObject>();
            Schedule = schedule;
        }

        public GameObject(GameObject location) : this(location.Schedule, location) { }

        public GameObject(ISchedule schedule) : this(schedule, new OffscreenLocation()) { }


        public TraitType GetTrait<TraitType>() where TraitType: Trait
        {
            IReadOnlyCollection<Trait> candidates;
            if (instanceTraits != null)
            {
                candidates = instanceTraits;
            }
            else
            {
                candidates = ClassTraits;
            }
            foreach (Trait trait in candidates)
            {
                if (trait is TraitType foundTrait)
                {
                    return foundTrait;
                }
            }
            return null;
        }

        public bool AddTrait(Trait trait)
        {
            if (instanceTraits == null)
            {
                instanceTraits = new HashSet<Trait>(ClassTraits);
            }
            return instanceTraits.Add(trait);
        }

        public void Move(ILocation location)
        {
            Location.BeExited(this);
            Location = location;
            Location.BeEntered(this);
        }

        public bool RemoveTrait(Trait trait)
        {
            if (instanceTraits == null)
            {
                instanceTraits = new HashSet<Trait>(ClassTraits);
            }
            return instanceTraits.Remove(trait);
        }

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
    }

    public class Item : GameObject
    {

        public override IReadOnlyCollection<Trait> ClassTraits { get; } = new HashSet<Trait>() { new ItemTrait() };

        public Item(GameObject location) : base(location)
        {
        }

        public Item(ISchedule schedule) : base(schedule)
        {
        }

        public Item(ISchedule schedule, ILocation location) : base(schedule, location)
        {
        }
    }
}
