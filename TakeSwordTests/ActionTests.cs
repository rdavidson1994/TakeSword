using NUnit.Framework;
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
        [Test]
        public void TakeTest()
        {
            GameObject place = new GameObject();
            GameObject prop = new GameObject(place);
            prop.AddTrait(new ItemTrait { Weight = 1 });
            PhysicalActor actor = new PhysicalActor(place);
            Take take = new Take { Actor = actor, Target = prop };
            ActionOutcome outcome = take.Attempt();
            Assert.IsTrue(outcome.Success());
            Assert.AreEqual(actor, prop.Location);
        }

        [Test]
        public void DropTest()
        {
            GameObject place = new GameObject();
            PhysicalActor actor = new PhysicalActor(place);
            GameObject prop = new GameObject(actor);
            ActionOutcome outcome = new Drop { Actor = actor, Target = prop }.Attempt();
            Assert.IsTrue(outcome.Success());
            Assert.AreEqual(place, prop.Location);
        }
    }
}
