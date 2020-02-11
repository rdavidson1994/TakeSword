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
        private class FakeAction : IAction<IActor>
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
                wasAttempted = true;
                return new SuccessfulOutcome();
            }

            public IActor Actor => actor;

            public ActionOutcome IsValid()
            {
                return new SuccessfulOutcome();
            }

            public IRoutine<IActor> AsRoutine()
            {
                throw new NotImplementedException();
            }
        }
        private class FakeRoutine : IRoutine<IActor>
        {
            private IActor actor;
            public FakeRoutine(IActor actor)
            {
                this.actor = actor;
            }

            public IRoutine<IActor> AsRoutine()
            {
                throw new NotImplementedException();
            }

            public IActor Actor => actor;

            public ActionOutcome IsValid()
            {
                throw new NotImplementedException();
            }

            public IAction<IActor> NextAction()
            {
                return new FakeAction(actor);
            }

            public IAction<IActor> Peek()
            {
                throw new NotImplementedException();
            }

            public void ReactToAnnouncement(ActionAnnouncement announcement)
            {
                throw new NotImplementedException();
            }
        }
    }
}
