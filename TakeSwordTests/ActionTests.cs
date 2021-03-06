﻿using NUnit.Framework;
using System;
using Moq;
using System.Collections.Generic;
using System.Text;
using TakeSword;

namespace TakeSwordTests
{
    [TestFixture]
    public class ActionTests
    {
        public class FakeBody : IBody
        {
            public bool Alive => throw new NotImplementedException();

            public bool NeedsUpdate => throw new NotImplementedException();

            public void TakeDamage(int amount, DamageType damageType, BodyPartKind bodyPart)
            {
                throw new NotImplementedException();
            }

            public void Update()
            {
                throw new NotImplementedException();
            }

            public void Update(int deltaTime)
            {
                throw new NotImplementedException();
            }
        }
        [Test]
        public void TakeTest()
        {
            GameObject place = new GameObject();
            GameObject prop = new GameObject(place);
            prop.AddTrait(new InventoryItem(weight: 1));
            PhysicalActor actor = new PhysicalActor(new FakeBody(), place);
            Take take = new Take { Actor = actor, Target = prop };
            ActionOutcome outcome = take.Attempt();
            Assert.IsTrue(outcome.Success());
            Assert.AreEqual(actor, prop.Location);
        }

        [Test]
        public void DropTest()
        {
            GameObject place = new GameObject();
            PhysicalActor actor = new PhysicalActor(new FakeBody(), place);
            GameObject prop = new GameObject(actor);
            ActionOutcome outcome = new Drop { Actor = actor, Target = prop }.Attempt();
            Assert.IsTrue(outcome.Success());
            Assert.AreEqual(place, prop.Location);
        }
    }
}
