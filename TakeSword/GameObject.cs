using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TakeSword
{
    public class GameObject : ILocation, IGameObject
    {
        protected readonly HashSet<GameObject> contents;
        public IName Name { get; set; }
        public string StringName
        {
            set
            {
                Name = new Name(value);
            }
        }
        public IReadOnlyCollection<GameObject> Contents { get { return contents; } }
        public ILocation Location { get; protected set; }
        protected IWritableTraitStore traits;
        public ITraitStore Traits
        {
            set
            {
                traits = new MirrorTraitStore(value.Freeze());
            }
        }
        public ISchedule Schedule { get; protected set; }
        public GameObject(ILocation? location = null, FrozenTraitStore? traits = null)
        {
            if (location == null)
                location = new OffscreenLocation();
            if (traits == null)
                traits = FrozenTraitStore.Empty();
            this.traits = new MirrorTraitStore(traits);
            Location = location;
            Location.BeEntered(this);
            Schedule = Location.Schedule;
            contents = new HashSet<GameObject>();
            Name = new SimpleName("nameless object");
        }

        public GameObject(ISchedule schedule, FrozenTraitStore? traits = null)
        {
            if (traits == null)
                traits = FrozenTraitStore.Empty();
            this.traits = new MirrorTraitStore(traits);
            Location = new OffscreenLocation();
            Schedule = schedule;
            contents = new HashSet<GameObject>();
            Name = new SimpleName("nameless object");
        }

        public bool Is<TraitType>([NotNullWhen(true)]out TraitType? trait) where TraitType : Trait
        {
            trait = this.As<TraitType>();
            return (trait != null);
        }

        public virtual FormattableString ShortDescription(PhysicalActor viewer)
        {
            return $"{this}";
        }

        public bool Is<TraitType>() where TraitType : Trait
        {
            return Is(out TraitType _);
        }

        public TraitType? As<TraitType>() where TraitType : Trait
        {
            return traits.Get<TraitType>();
        }

        public ActionOutcome Is<TraitType>(FormattableString reason) where TraitType : Trait
        {
            if (Is(out TraitType _))
            {
                return new SuccessfulOutcome();
            }
            else
            {
                return new FailedOutcome(reason);
            }
        }

        public string DisplayName(IGameObject? viewer)
        {
            if (viewer == this)
            {
                return "you";
            }
            return Name.NameWithArticle(viewer);
        }

        public string GetName(IGameObject viewer)
        {
            if (Name != null)
            {
                return Name.GetName(viewer);
            }
            else
            {
                return "unnamed";
            }
        }

        public virtual ActionOutcome Allows(ActionAnnouncement announcement)
        {
            return new SuccessfulOutcome();
        }

        public bool AddTrait<T>(T trait) where T : Trait
        {
            traits.Add(trait);
            return true;
        }

        public void Move(ILocation location)
        {
            Location.BeExited(this);
            Location = location;
            Schedule = Location.Schedule;
            Location.BeEntered(this);
        }

        public void Vanish()
        {
            Move(new OffscreenLocation());
        }

        public virtual void TakeDamage(int amount, DamageType type, BodyPartKind bodyPart)
        {
            // do nothing
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

        protected void BroadcastAnnouncement(ActionAnnouncement announcement)
        {
            ActionAnnouncement announcementForBystanders = new ActionAnnouncement(
                content: announcement.Content,
                outcome: announcement.Outcome,
                relationship: TargetType.Witness
            );
            foreach (GameObject gameObject in contents)
            {
                gameObject.HandleAnnouncement(announcementForBystanders);
            }
        }

        public virtual bool HasName(IGameObject viewer, string name)
        {
            if (Name == null)
            {
                return false;
            }
            return Name.Matches(viewer, name);
        }

        protected virtual void ReactToAnnouncement(ActionAnnouncement announcement) { }

        public IEnumerable<GameObject> NearbyObjects(long range)
        {
            return contents;
        }

        public bool RemoveTrait<T>() where T : Trait
        {
            return traits.Remove<T>();
        }

        public virtual FormattableString DescriptionForInhabitant(GameObject viewer)
        {
            return $"You are inside {this}";
        }

        public void HandleAnnouncement(ActionAnnouncement announcement)
        {
            ReactToAnnouncement(announcement);
            if (announcement.Relationship == TargetType.Scene)
            {
                var bystanderAnnouncement = new ActionAnnouncement(
                    content: announcement.Content,
                    outcome: announcement.Outcome,
                    relationship: TargetType.Witness
                );
                BroadcastAnnouncement(bystanderAnnouncement);
            }
        }

        public void HandleTextMessage(FormattableString message)
        {
            ReceiveTextMessage(message);
            foreach (GameObject innerObject in Contents)
            {
                innerObject.ReceiveTextMessage(message);
            }
        }

        public virtual void ReceiveTextMessage(FormattableString message) { }
    }
}
