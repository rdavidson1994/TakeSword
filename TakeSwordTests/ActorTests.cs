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
        private class FakeActor : IActor<FakeActor>
        {
            public void Act()
            {
                throw new NotImplementedException();
            }

            public void AttemptAction(IAction<FakeActor> action)
            {
                throw new NotImplementedException();
            }
        }
        private class FakeAction : IAction<FakeActor>
        {
            public bool wasAttempted;
            public FakeAction(FakeActor actor)
            {
                Actor = actor;
            }
            public long OnsetTime => 0;

            public long CooldownTime => 0;

            public ActionOutcome Attempt()
            {
                wasAttempted = true;
                return new SuccessfulOutcome();
            }

            public FakeActor Actor { get; set; }

            public ActionOutcome IsValid()
            {
                return new SuccessfulOutcome();
            }

            public IRoutine<FakeActor> AsRoutine()
            {
                throw new NotImplementedException();
            }
        }
        private class FakeRoutine : IRoutine<FakeActor>
        {
            public FakeActor Actor { get; set; }
            public FakeRoutine(FakeActor actor)
            {
                Actor = actor;
            }

            public IRoutine<FakeActor> AsRoutine()
            {
                throw new NotImplementedException();
            }

            public ActionOutcome IsValid()
            {
                throw new NotImplementedException();
            }

            public IAction<FakeActor> NextAction()
            {
                return new FakeAction(Actor);
            }

            public IAction<FakeActor> Peek()
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
