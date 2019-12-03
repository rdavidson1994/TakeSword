using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TakeSword;

namespace TakeSwordTests
{
    

    [TestFixture]
    public class RequirementManagerTests
    {
        private Dictionary<string, string> lookup;
        private FakeVerbalAI fakeVerbalAI;
        private RequirementManager manager;

        public class FakeVerbalAI : IVerbalAI
        {
            public IRoutine AsRoutine()
            {
                return this;
            }

            public void ChooseValue(Hypothetical hypothetical)
            {
                if (hypothetical is Hypothetical<string> hString)
                {
                    foreach (string str in hString.PossibleValues)
                    {
                        if (str.StartsWith("fancy")) {
                            hString.Value = str;
                        }
                    }
                }
            }

            public PhysicalActor GetActor()
            {
                throw new System.NotImplementedException();
            }

            public void GetPossibilities(Hypothetical hypothetical)
            {
                if (hypothetical is Hypothetical<string> hString)
                {
                    if (hypothetical.Name == "fork")
                        hString.PossibleValues = new List<string> { "fancy fork", "grungy fork", "rusty fork" };
                }
            }

            public ActionOutcome IsValid()
            {
                throw new System.NotImplementedException();
            }

            public IAction NextAction()
            {
                throw new System.NotImplementedException();
            }

            public void ReactToAnnouncement(object announcement)
            {
                throw new NotImplementedException();
            }

            IActor IActivity.GetActor()
            {
                throw new System.NotImplementedException();
            }
        }

        [SetUp]
        public void Init()
        {
            lookup = new Dictionary<string, string>
            {
                {"UTENSIL","fork" },
                {"FOOD", "spam" }
            };
            fakeVerbalAI = new FakeVerbalAI();
            manager = new RequirementManager(lookup, fakeVerbalAI);
        }

        [Test]
        public void RequirementsWork()
        {
            Hypothetical<string> output = manager.Require<string>("UTENSIL");
            Assert.IsNull(output.Value);
            Assert.AreEqual(3, output.PossibleValues.Count);
        }

        [Test]
        public void FulfillmentWorks()
        {
            Hypothetical<string> output = manager.Require<string>("UTENSIL");
            Assert.IsTrue(manager.Fulfill());
            Assert.AreEqual("fancy fork", output.Value);
        }

        [Test]
        public void BadRequirementsRaiseException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                manager.Require<string>("NONSENSE");
            });
        }
    }
}
