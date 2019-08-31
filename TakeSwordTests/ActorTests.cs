using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TakeSword;

namespace TakeSwordTests
{
    [TestFixture]
    class ActorTests
    {
        private class FakeAction : IAction
        {
            private IActor actor;
            public bool wasAttempted;
            public FakeAction(IActor actor)
            {
                this.actor = actor;
            }
            public long OnsetTime => 0;

            public long CooldownTime => 0;

            public IActionOutcome Attempt()
            {
                return new SuccessfulOutcome(this);
            }

            public IActor GetActor()
            {
                return actor;
            }

            public IActionOutcome IsValid()
            {
                return new SuccessfulOutcome(this);
            }
        }
        private class FakeRoutine : IRoutine
        {
            private IActor actor;
            public FakeRoutine(IActor actor)
            {
                this.actor = actor;
            }
            public IActor GetActor()
            {
                return actor;
            }

            public IAction NextAction()
            {
                return new FakeAction(actor);
            }
        }
    }
}
