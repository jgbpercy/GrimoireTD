using System.Collections.Generic;
using System.Linq;
using System;
using NUnit.Framework;

namespace GrimoireTD.Tests.MutableRecalculatorListTests
{
    public class MutableRecalculatorListTests
    {
        private class TestEA : EventArgs
        {
            public readonly int NewValue;

            public TestEA(int newValue)
            {
                NewValue = newValue;
            }
        }

        private class TestIntWrapper : INotifyOnChange<TestEA>
        {
            private int _intValue;

            public TestIntWrapper(int initValue)
            {
                _intValue = initValue;
            }

            public int IntValue
            {
                get
                {
                    return _intValue;
                }
                set
                {
                    _intValue = value;

                    OnChange?.Invoke(this, new TestEA(value));
                }
            }

            public event EventHandler<TestEA> OnChange;
        }

        private string TestValueFunction(List<TestIntWrapper> list)
        {
            return list.Select(x => x.IntValue).Sum().ToString();
        }

        [Test]
        public void ListCtor_Always_CalculatesValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                new TestIntWrapper(7),
            }, TestValueFunction);

            Assert.AreEqual("10", subject.Value);
        }

        [Test]
        public void EmptyCtor_Always_CalculatesValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(TestValueFunction);

            Assert.AreEqual("0", subject.Value);
        }

        [Test]
        public void Add_Always_UpdatesValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { new TestIntWrapper(1) }, TestValueFunction);

            subject.Add(new TestIntWrapper(4));

            Assert.AreEqual("5", subject.Value);
        }

        [Test]
        public void Add_Always_ReturnsUpdatedValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { new TestIntWrapper(1) }, TestValueFunction);

            var result = subject.Add(new TestIntWrapper(4));

            Assert.AreEqual("5", result);
        }

        [Test]
        public void Add_Always_FiresOnChangeEvent()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { new TestIntWrapper(1) }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.Add(new TestIntWrapper(4));

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "5");
        }

        [Test]
        public void AddRange_Always_UpdatesValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { new TestIntWrapper(1) }, TestValueFunction);

            subject.AddRange(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                new TestIntWrapper(5),
            });

            Assert.AreEqual("9", subject.Value);
        }

        [Test]
        public void AddRange_Always_ReturnsUpdatedValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { new TestIntWrapper(1) }, TestValueFunction);

            var result = subject.AddRange(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                new TestIntWrapper(5),
            });

            Assert.AreEqual("9", result);
        }

        [Test]
        public void AddRange_Always_FiresOnChangeEventOnce()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { new TestIntWrapper(1) }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.AddRange(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                new TestIntWrapper(5),
            });

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "9");
        }

        [Test]
        public void Remove_ItemPresent_UpdatesValue()
        {
            var itemToRemove = new TestIntWrapper(9);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                itemToRemove,
                new TestIntWrapper(1),
            }, TestValueFunction);

            subject.Remove(itemToRemove);

            Assert.AreEqual("4", subject.Value);
        }

        [Test]
        public void Remove_ItemPresent_ReturnsUpdatedValue()
        {
            var itemToRemove = new TestIntWrapper(9);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                itemToRemove,
                new TestIntWrapper(1),
            }, TestValueFunction);

            var result = subject.Remove(itemToRemove);

            Assert.AreEqual("4", result);
        }

        [Test]
        public void Remove_ItemPresent_FiresOnChangeEvent()
        {
            var itemToRemove = new TestIntWrapper(9);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> { 
                new TestIntWrapper(3),
                itemToRemove,
                new TestIntWrapper(1),
            }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.Remove(itemToRemove);

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "4");
        }

        [Test]
        public void Remove_ItemNotPresent_ThrowsKeyNotFoundException()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(3),
                new TestIntWrapper(9),
                new TestIntWrapper(1),
            }, TestValueFunction);

            Assert.Throws<KeyNotFoundException>(() => subject.Remove(new TestIntWrapper(2)));
        }

        [Test]
        public void Replace_RemovedItemPresent_UpdatesValue()
        {
            var itemToReplace = new TestIntWrapper(6);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(4),
                itemToReplace,
                new TestIntWrapper(7)
            }, TestValueFunction);

            subject.Replace(
                itemToReplace,
                new TestIntWrapper(3)
            );

            Assert.AreEqual("14", subject.Value);
        }

        [Test]
        public void Replace_RemovedItemPresent_ReturnsUpdatedValue()
        {
            var itemToReplace = new TestIntWrapper(6);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(4),
                itemToReplace,
                new TestIntWrapper(7)
            }, TestValueFunction);

            var result = subject.Replace(
                itemToReplace,
                new TestIntWrapper(3)
            );

            Assert.AreEqual("14", result);
        }

        [Test]
        public void Replace_RemovedItemPresent_FiresOnChangeEvent()
        {
            var itemToReplace = new TestIntWrapper(6);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(4),
                itemToReplace,
                new TestIntWrapper(7)
            }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            subject.Replace(
                itemToReplace,
                new TestIntWrapper(3)
            );

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "14");
        }

        [Test]
        public void Replace_RemovedItemNotPresent_ThrowsKeyNotFoundException()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper> {
                new TestIntWrapper(4),
                new TestIntWrapper(6),
                new TestIntWrapper(7)
            }, TestValueFunction);

            Assert.Throws<KeyNotFoundException>(() => subject.Replace(new TestIntWrapper(2), new TestIntWrapper(3)));
        }

        [Test]
        public void OnStartingItemChange_Always_RecalculatesValue()
        {
            var itemToChange = new TestIntWrapper(3);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper>
            {
                itemToChange,
                new TestIntWrapper(2),
            }, TestValueFunction);

            itemToChange.IntValue = 6;

            Assert.AreEqual("8", subject.Value);
        }

        [Test]
        public void OnStartingItemChange_Always_FiresOnChangeEvent()
        {
            var itemToChange = new TestIntWrapper(3);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(new List<TestIntWrapper>
            {
                itemToChange,
                new TestIntWrapper(2),
            }, TestValueFunction);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            itemToChange.IntValue = 6;

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "8");
        }

        [Test]
        public void OnAddedItemChange_Always_RecalculatesValue()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(
                new List<TestIntWrapper> { new TestIntWrapper(2), }, 
                TestValueFunction
            );

            var itemToChange = new TestIntWrapper(4);

            subject.Add(itemToChange);

            itemToChange.IntValue = 7;

            Assert.AreEqual("9", subject.Value);
        }

        [Test]
        public void OnAddedItemChange_Always_FiresOnChangeEvent()
        {
            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(
                new List<TestIntWrapper> { new TestIntWrapper(2), },
                TestValueFunction
            );

            var itemToChange = new TestIntWrapper(4);

            subject.Add(itemToChange);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            itemToChange.IntValue = 7;

            eventTester.AssertFired(1);
            eventTester.AssertResult(subject, args => args.NewValue == "9");
        }

        [Test]
        public void OnRemovedItemChange_Never_ChangesValue()
        {
            var itemToRemove = new TestIntWrapper(10);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(
                new List<TestIntWrapper>
                {
                    new TestIntWrapper(3),
                    itemToRemove
                },
                TestValueFunction
            );

            subject.Remove(itemToRemove);

            itemToRemove.IntValue = 5;

            Assert.AreEqual("3", subject.Value);
        }

        [Test]
        public void OnRemovedItemChange_Never_FiresOnChangeEvent()
        {
            var itemToRemove = new TestIntWrapper(10);

            var subject = new MutableRecalculatorList<TestIntWrapper, string, TestEA>(
                new List<TestIntWrapper>
                {
                    new TestIntWrapper(3),
                    itemToRemove
                },
                TestValueFunction
            );

            subject.Remove(itemToRemove);

            var eventTester = new EventTester<EAOnRecalculatorListChange<string>>();
            subject.OnChange += eventTester.Handler;

            itemToRemove.IntValue = 5;

            eventTester.AssertFired(false);
        }
    }
}