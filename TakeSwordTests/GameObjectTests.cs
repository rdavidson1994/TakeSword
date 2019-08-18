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

        public void RunFor(long deltaTime)
        {
            throw new NotImplementedException();
        }

        public void RunOnce()
        {
            throw new NotImplementedException();
        }
    }

    class TraitStub : Trait { }
    class LocationSpy : ILocation
    {
        public bool wasEntered;
        public bool wasExited;
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

        public void HandleAnnouncement(object announcement)
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
            ISchedule schedule = new ScheduleStub();
            GameObject gameObject = new GameObject(schedule, start);
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
            GameObject gameObject = new GameObject(schedule);
            gameObject.AddTrait(trait);
            TraitStub retrievedTrait = gameObject.GetTrait<TraitStub>();
            Assert.AreEqual(trait, retrievedTrait);
        }
        [Test]
        public void NullTraitTest()
        {
            TraitStub trait = new TraitStub();
            ISchedule schedule = new ScheduleStub();
            GameObject gameObject = new GameObject(schedule);
            TraitStub retrievedTrait = gameObject.GetTrait<TraitStub>();
            Assert.IsNull(retrievedTrait);
        }
        [Test]
        public void RemoveTrait()
        {
            ISchedule schedule = new ScheduleStub();
            Item item = new Item(schedule);
            Item item2 = new Item(schedule);
            Trait trait = item.GetTrait<ItemTrait>();
            item.RemoveTrait(trait);
            Assert.IsNull(item.GetTrait<ItemTrait>());
            Assert.IsNotNull(item2.GetTrait<ItemTrait>());
        }

        [Test]
        public void ObjectAsLocation()
        {
            ISchedule schedule = new ScheduleStub();
            GameObject gameObject = new GameObject(schedule);
            GameObject inner = new GameObject(gameObject);
            Assert.IsTrue(gameObject.Contents.Contains(inner));
            Assert.AreEqual(inner.Location, gameObject);
        }
    }
}
