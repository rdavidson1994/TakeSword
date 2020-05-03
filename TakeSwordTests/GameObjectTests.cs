using System;
using TakeSword;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Linq;

namespace TakeSwordTests
{
    class ScheduleStub : ISchedule
    {
        public void Add(IEvent event1, long delay)
        {
            throw new NotImplementedException();
        }

        public bool RunFor(long deltaTime)
        {
            throw new NotImplementedException();
        }

        public bool RunOnce()
        {
            throw new NotImplementedException();
        }
    }

    class TraitStub : Trait {
        public int traitProperty = 0;
    }
    class LocationSpy : ILocation
    {
        public bool wasEntered;
        public bool wasExited;

        public ISchedule Schedule => new ScheduleStub();

        public bool BeEntered(GameObject gameObject)
        {
            wasEntered = true;
            return true;
        }

        public bool BeExited(GameObject gameObject)
        {
            wasExited = true;
            return true;
        }

        public FormattableString DescriptionForInhabitant(GameObject viewer)
        {
            throw new NotImplementedException();
        }

        public void HandleAnnouncement(ActionAnnouncement announcement)
        {
            throw new NotImplementedException();
        }

        public void HandleTextMessage(FormattableString message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GameObject> NearbyObjects(long range)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class GameObjectTests
    {
        [Test]
        public void MoveTest()
        {
            var start = new LocationSpy();
            var finish = new LocationSpy();
            GameObject gameObject = new GameObject(start);
            gameObject.Move(finish);
            Assert.IsTrue(start.wasEntered);
            Assert.IsTrue(start.wasExited);
            Assert.IsTrue(finish.wasEntered);
            Assert.IsFalse(finish.wasExited);
        }
        [Test]
        public void AddTraitTest()
        {
            TraitStub trait = new TraitStub();
            ISchedule schedule = new ScheduleStub();
            GameObject gameObject = new GameObject();
            gameObject.AddTrait(trait);
            TraitStub retrievedTrait = gameObject.As<TraitStub>();
            Assert.AreEqual(trait, retrievedTrait);
        }
        [Test]
        public void NullTraitTest()
        {
            TraitStub trait = new TraitStub();
            ISchedule schedule = new ScheduleStub();
            GameObject gameObject = new GameObject();
            TraitStub retrievedTrait = gameObject.As<TraitStub>();
            Assert.IsNull(retrievedTrait);
        }
        [Test]
        public void RemoveTrait()
        {
            FrozenTraitStore initialTraits = new TraitStore()
            {
                new TraitStub
                {
                    traitProperty = 10
                }
            }
            .Freeze();

            GameObject item = new GameObject(traits: initialTraits);
            GameObject item2 = new GameObject(traits: initialTraits);
            item.RemoveTrait<TraitStub>();
            Assert.IsNull(item.As<TraitStub>());
            Assert.AreEqual(10, item2.As<TraitStub>().traitProperty);
        }

        [Test]
        public void ObjectAsLocation()
        {
            GameObject gameObject = new GameObject();
            GameObject inner = new GameObject(gameObject);
            Assert.IsTrue(gameObject.Contents.Contains(inner));
            Assert.AreEqual(inner.Location, gameObject);
        }
    }
}
