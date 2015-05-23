using CodeVision.Model;
using NUnit.Framework;

namespace CodeVision.Tests
{
    [TestFixture]
    public class HitTests
    {
        [TestCase("c:\\jenkins\\workspace", "c:\\jenkins\\workspace\\Notifier\\Account.cs", "Notifier\\Account.cs" )]
        [TestCase("c:\\jenkins\\workspace\\", "c:\\jenkins\\workspace\\Notifier\\Account.cs", "Notifier\\Account.cs")]
        [TestCase("c:\\jenkins\\bogus\\", "c:\\jenkins\\workspace\\Notifier\\Account.cs", "Account.cs")]
        [TestCase("c:\\jenkins\\workspace\\", "c:\\jenkins\\workspace\\Account.cs", "Account.cs")]
        [TestCase("c:\\jenkins\\workspace\\", "c:\\Account.cs", "Account.cs")]
        public void Hit_FriendlyName(string rootPath, string fullPath, string expected)
        {
            var hit = new Hit(rootPath, fullPath);
            Assert.That(hit.FriendlyFileName, Is.EqualTo(expected));
        }
    }
}
