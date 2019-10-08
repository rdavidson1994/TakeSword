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

            public ActionOutcome Attempt()
            {
                return new SuccessfulOutcome(this);
            }

            public IActor GetActor()
            {
                return actor;
            }

            public ActionOutcome IsValid()
            {
                return new SuccessfulOutcome(this);
            }

            public IRoutine AsRoutine()
            {
                throw new NotImplementedException();
            }
        }
        private class FakeRoutine : IRoutine
        {
            private IActor actor;
            public FakeRoutine(IActor actor)
            {
                this.actor = actor;
            }

            public IRoutine AsRoutine()
            {
                throw new NotImplementedException();
            }

            public IActor GetActor()
            {
                return actor;
            }

            public ActionOutcome IsValid()
            {
                throw new NotImplementedException();
            }

            public IAction NextAction()
            {
                return new FakeAction(actor);
            }
        }
    }
}
