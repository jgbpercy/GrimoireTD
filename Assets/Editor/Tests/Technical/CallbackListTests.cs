using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace GrimoireTD.Tests.CallbackListTests
{
    public class CallbackListTests
    {
        [Test]
        public void Count_Always_ReturnsCorrectNumberOfElements()
        {
            var subject = new CallbackList<int> { 5, 6, 7 };

            Assert.AreEqual(3, subject.Count);
        }

        [Test]
        public void Indexer_Always_ReturnsElementAtIndex()
        {
            var subject = new CallbackList<string> { "one", "two", "three" };

            Assert.AreEqual("one", subject[0]);
            Assert.AreEqual("three", subject[2]);
        }

        [Test]
        public void Add_Always_AddsItemToList()
        {
            var subject = new CallbackList<int>();

            subject.Add(3);

            Assert.AreEqual(3, subject[0]);
        }

        [Test]
        public void Add_Always_FiresOnAddEvent()
        {
            var subject = new CallbackList<int>();

            var eventTester = new EventTester<EAOnCallbackListAdd<int>>();
            subject.OnAdd += eventTester.Handler;

            subject.Add(6);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.AddedItem == 6);
        }

        [Test]
        public void Contains_ListContainsItem_ReturnsTrue()
        {
            var subject = new CallbackList<int> { 3, 4 };

            Assert.True(subject.Contains(4));
        }

        [Test]
        public void Contains_ListDoesNotContainItem_ReturnsFalse()
        {
            var subject = new CallbackList<int> { 5, 6 };

            Assert.False(subject.Contains(1));
        }
    
        [Test]
        public void TryRemove_ListContainsItem_RemovesItem()
        {
            var subject = new CallbackList<string> { "blah", "thing " };

            subject.TryRemove("blah");

            Assert.False(subject.Contains("blah"));
        }

        [Test]
        public void TryRemove_ListContainsItem_FiresOnRemoveEvent()
        {
            var subject = new CallbackList<string> { "blah", "doobery" };

            var eventTester = new EventTester<EAOnCallbackListRemove<string>>();
            subject.OnRemove += eventTester.Handler;

            subject.TryRemove("blah");

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, arg => arg.RemovedItem == "blah");
        }

        [Test]
        public void TryRemove_ListContainsItem_ReturnsTrue()
        {
            var subject = new CallbackList<string> { "blah", "thing" };

            var result = subject.TryRemove("blah");

            Assert.True(result);
        }

        [Test]
        public void TryRemove_ListDoesNotContainItem_DoesNotFireOnRemoveEvent()
        {
            var subject = new CallbackList<string> { "thing" };

            var eventTester = new EventTester<EAOnCallbackListRemove<string>>();
            subject.OnRemove += eventTester.Handler;

            subject.TryRemove("not thing");

            eventTester.AssertFired(false);
        }

        [Test]
        public void TryRemove_ListDoesNotContainItem_ReturnsFalse()
        {
            var subject = new CallbackList<string> { "blah", };

            var result = subject.TryRemove("not blah");

            Assert.False(result);
        }

        private class TestStringEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Substring(0, 1) == y.Substring(0, 1);
            }

            public int GetHashCode(string obj) => obj.GetHashCode();
        }

        private TestStringEqualityComparer testStringEqualityComparer = new TestStringEqualityComparer();

        [Test]
        public void TryRemoveWithEqualityComparer_ListContainsItem_RemovesItem()
        {
            var subject = new CallbackList<string> { "blah" };

            subject.TryRemove("beep", testStringEqualityComparer);

            Assert.False(subject.Contains("blah"));
        }

        [Test]
        public void TryRemoveWithEqualityComparer_ListContainsItem_FiresOnRemoveEvent()
        {
            var subject = new CallbackList<string> { "blah" };

            var eventTester = new EventTester<EAOnCallbackListRemove<string>>();
            subject.OnRemove += eventTester.Handler;

            subject.TryRemove("beep", testStringEqualityComparer);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, arg => arg.RemovedItem == "blah");
        }

        [Test]
        public void TryRemoveWithEqualityComparer_ListContainsItem_ReturnsTrue()
        {
            var subject = new CallbackList<string> { "blah" };

            var result = subject.TryRemove("bleep", testStringEqualityComparer);

            Assert.True(result);
        }

        [Test]
        public void TryRemoveWithEqualityComparer_ListDoesNotContainItem_DoesNotFireOnRemoveEvent()
        {
            var subject = new CallbackList<string> { "blah" };

            var eventTester = new EventTester<EAOnCallbackListRemove<string>>();
            subject.OnRemove += eventTester.Handler;

            subject.TryRemove("not beep", testStringEqualityComparer);

            eventTester.AssertFired(false);
        }

        [Test]
        public void TryRemoveWithEqualityComparer_ListDoesNotContainItem_ReturnsFalse()
        {
            var subject = new CallbackList<string> { "blah" };

            var result = subject.TryRemove("not bleep", testStringEqualityComparer);

            Assert.False(result);
        }

        [Test]
        public void Clear_Always_RemovesAllItemsFromList()
        {
            var subject = new CallbackList<int> { 3, 5, 6 };

            subject.Clear();

            Assert.AreEqual(0, subject.Count);
        }

        [Test]
        public void Clear_Always_FiresOnRemoveForEachItemInList()
        {
            var subject = new CallbackList<int> { 1, 2, 4 };

            var eventTester = new EventTester<EAOnCallbackListRemove<int>>();
            subject.OnRemove += eventTester.Handler;

            subject.Clear();

            eventTester.AssertFired(3);
            eventTester.AssertResults(argList =>
            {
                return 
                    argList.Any(x => x.RemovedItem == 1) &&
                    argList.Any(x => x.RemovedItem == 2) &&
                    argList.Any(x => x.RemovedItem == 4);
            });
        }
    }
}