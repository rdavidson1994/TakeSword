using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using TakeSword;

namespace TakeSwordTests
{
    [TestFixture]
    class ParserBuilderTests
    {
        [Test]
        public void LiteralsWork()
        {
            IParser parser = new ParserBuilder().CreateParser("someLiteralText");
            var match = parser.Match("someLiteralText");
            Assert.IsNotNull(match);
            Assert.IsEmpty(match);
        }
        [Test]
        public void InvalidMacroFails()
        {
            ParserBuilder builder = new ParserBuilder();
            Assert.Catch<InvalidOperationException>(() => {
                builder.AddMacro("NOTaLLCAPS", "anystring");
            });
        }
        [Test]
        public void MarcosWork()
        {
            var parser = new ParserBuilder()
                 .AddMacro("MYMACRO", "val1", "val2")
                 .CreateParser("MYMACRO some literal text");
            var goodMatch = parser.Match("val1 some literal text");
            Assert.IsNotNull(goodMatch);
            Assert.IsEmpty(goodMatch);
            Assert.IsNotNull(parser.Match("val2 some literal text"));
            Assert.IsNull(parser.Match("val3 some literal text"));
            Assert.IsNull(parser.Match("val1 some bad text"));
        }
        [Test]
        public void NamedGroupsWork()
        {
            IParser parser = new ParserBuilder().CreateParser("CAPTURE then literal");
            var lookupTable = parser.Match("some text then literal");
            Assert.AreEqual("some text", lookupTable["CAPTURE"]);
            Assert.AreEqual(lookupTable.Count, 1);
        }

        [Test]
        public void MacrosWithNamedGroups()
        {
            IParser parser = new ParserBuilder()
                .AddMacro("VERB", "get", "pick up" )
                .CreateParser("VERB the TARGET");
            Assert.IsNull(parser.Match("get the"));
            Assert.IsNotNull(parser.Match("get the stick"));
            Assert.IsNull(parser.Match("get stick"));
            Assert.IsNotNull(parser.Match("pick up the sword"));
        }

    }
}
