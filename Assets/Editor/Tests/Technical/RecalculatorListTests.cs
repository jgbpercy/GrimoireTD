using System.Collections.Generic;
using System.Linq;
using System;
using NUnit.Framework;

namespace GrimoireTD.Tests.RecalculatorListTests
{
    public class RecalculatorListTests
    {
        private string TestValueFunction(List<int> list)
        {
            return list.Sum().ToString();
        }

        protected virtual RecalculatorList<TItem, TValue> ConstructSubject<TItem, TValue>(List<TItem> initialList, Func<List<TItem>, TValue> valueFunction)
        {
            return new RecalculatorList<TItem, TValue>(initialList, valueFunction);
        }

        [Test]
        public void ListCtor_Always_CalculatesValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 3, 7, 12 }, TestValueFunction);

            Assert.AreEqual("22", subject.Value);
        }

        [Test]
        public void EmptyCtor_Always_CalculatesValue()
        {
            var subject = new RecalculatorList<int, string>(TestValueFunction);

            Assert.AreEqual("0", subject.Value);
        }

        [Test]
        public void Add_Always_UpdatesValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 1 }, TestValueFunction);

            subject.Add(4);

            Assert.AreEqual("5", subject.Value);
        }

        [Test]
        public void Add_Always_ReturnsUpdatedValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 1 }, TestValueFunction);

            var result = subject.Add(4);

            Assert.AreEqual("5", result);
        }

        [Test]
        public void Add_Always_FiresOnChangeEvent()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 1 }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.Add(4);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "5");
        }

        [Test]
        public void AddRange_Always_UpdatesValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 1 }, TestValueFunction);

            subject.AddRange(new List<int> { 3, 5 });

            Assert.AreEqual("9", subject.Value);
        }

        [Test]
        public void AddRange_Always_ReturnsUpdatedValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 1 }, TestValueFunction);

            var result = subject.AddRange(new List<int> { 3, 5 });

            Assert.AreEqual("9", result);
        }

        [Test]
        public void AddRange_Always_FiresOnChangeEventOnce()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 1 }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.AddRange(new List<int> { 3, 5 });

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "9");
        }

        [Test]
        public void Remove_ItemPresent_UpdatesValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 3, 9, 1 }, TestValueFunction);

            subject.Remove(9);

            Assert.AreEqual("4", subject.Value);
        }

        [Test]
        public void Remove_ItemPresent_ReturnsUpdatedValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 3, 9, 1 }, TestValueFunction);

            var result = subject.Remove(9);

            Assert.AreEqual("4", result);
        }

        [Test]
        public void Remove_ItemPresent_FiresOnChangeEvent()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 3, 9, 1 }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.Remove(9);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "4");
        }

        [Test]
        public void Remove_ItemNotPresent_ThrowsKeyNotFoundException()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 3, 9, 1 }, TestValueFunction);

            Assert.Throws<KeyNotFoundException>(() => subject.Remove(2));
        }

        [Test]
        public void Replace_RemovedItemPresent_UpdatesValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 4, 6, 7 }, TestValueFunction);

            subject.Replace(6, 3);

            Assert.AreEqual("14", subject.Value);
        }

        [Test]
        public void Replace_RemovedItemPresent_ReturnsUpdatedValue()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 4, 6, 7 }, TestValueFunction);

            var result = subject.Replace(6, 3);

            Assert.AreEqual("14", result);
        }

        [Test]
        public void Replace_RemovedItemPresent_FiresOnChangeEvent()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 4, 6, 7 }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.Replace(6, 3);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "14");
        }

        [Test]
        public void Replace_RemovedItemNotPresent_ThrowsKeyNotFoundException()
        {
            var subject = new RecalculatorList<int, string>(new List<int> { 4, 6, 7 }, TestValueFunction);

            Assert.Throws<KeyNotFoundException>(() => subject.Replace(2, 3));
        }
    }
}