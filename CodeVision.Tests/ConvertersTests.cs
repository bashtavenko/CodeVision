using System.Collections.Generic;
using System.Linq;
using CodeVision.Dependencies.Database;
using CodeVision.Web.Common;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class ConvertersTests
    {
        [Test]
        public void Converters_FromCommaSeparatedListToEnumArray_Basic()
        {
            DatabaseObjectType[] result = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>("1, 2, 3");
            Assert.That(result.Length, Is.EqualTo(3));

            result = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>("1");
            Assert.That(result.Length, Is.EqualTo(1));
        }

        [Test]
        public void Converters_FromCommaSeparatedListToEnumArray_Invalid()
        {
            DatabaseObjectType[] result = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>("1, 2, -30");
            Assert.That(result.Length, Is.EqualTo(2));

            result = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>("1, 2, abc");
            Assert.That(result.Length, Is.EqualTo(2));

            result = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>(null);
            Assert.That(result.Length, Is.EqualTo(0));

            result = Converters.FromCommaSeparatedListToEnumArray<DatabaseObjectType>("");
            Assert.That(result.Length, Is.EqualTo(0));
        }
    }
}
