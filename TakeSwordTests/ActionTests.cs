using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TakeSword;

namespace TakeSwordTests
{
    [TestFixture]
    public class ActionTests
    {
        // The actual action code can make arbitrary changes to the game state;
        // Since they have no business logic, no unit tests are appropriate.
        // Instead we use integration tests.
        [Test]
        public void TakeTest()
        {
            GameObject place = new GameObject();
            GameObject prop = new GameObject(place);
            PhysicalActor actor = new PhysicalActor(place);
            IActionOutcome outcome = new Take(actor, prop).Attempt();
            Assert.IsTrue(outcome.Success());
            Assert.AreEqual(actor, prop.Location);
        }

        [Test]
        public void DropTest()
        {
            GameObject place = new GameObject();
            PhysicalActor actor = new PhysicalActor(place);
            GameObject prop = new GameObject(actor);
            IActionOutcome outcome = new Drop(actor, prop).Attempt();
            Assert.IsTrue(outcome.Success());
            Assert.AreEqual(place, prop.Location);
        }
    }
}
