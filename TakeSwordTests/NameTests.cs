using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TakeSword;

namespace TakeSwordTests
{
    [TestFixture]
    public class NameTests
    {
        [Test]
        public void PossessiveTest()
        {
            SimpleName ahabName = new SimpleName("Ahab");
            IName legName = ahabName.Possessive("leg");
            Assert.AreEqual(legName.GetName(null), "Ahab's leg");
        }
    }
}
