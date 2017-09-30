using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GrimoireTD.Tests
{
    public class EventTester<T> where T : EventArgs
    {
        public int EventFiredCount { get; private set; } = 0;

        public bool EventFired
        {
            get
            {
                return EventFiredCount > 0;
            }
        }

        private List<object> senderResults = new List<object>();

        public object SenderResult
        {
            get
            {
                if (senderResults.Count == 0)
                {
                    return null;
                }
                else
                {
                    return senderResults[senderResults.Count - 1];
                }
            }
        }

        public IReadOnlyList<object> SenderResults
        {
            get
            {
                return senderResults;
            }
        }

        private List<T> argResults = new List<T>();

        public T ArgsResult
        {
            get
            {
                if (argResults.Count == 0)
                {
                    return null;
                }
                else
                {
                    return argResults[argResults.Count - 1];
                }
            }
        }

        public IReadOnlyList<T> ArgResults
        {
            get
            {
                return argResults;
            }
        }

        public EventHandler<T> Handler { get; }

        public EventTester()
        {
            Handler = new EventHandler<T>((sender, args) =>
            {
                EventFiredCount += 1;
                senderResults.Add(sender);
                argResults.Add(args);
            });
        }

        public void AssertFired(bool eventFired)
        {
            Assert.AreEqual(eventFired, EventFired);
        }

        public void AssertResult(object senderResult, Func<T, bool> argTests)
        {
            Assert.True(EventFired);
            Assert.AreEqual(senderResult, SenderResult);
            Assert.True(argTests(ArgsResult));
        }

        public void AssertFired(int numberOfTimes)
        {
            Assert.AreEqual(numberOfTimes, EventFiredCount);
        }

        public void AssertResults(Func<IEnumerable<T>, bool> argTests)
        {
            Assert.True(EventFired);
            Assert.True(argTests(argResults));
        }

        public void AssertResults(IList<object> senderResults, Func<IEnumerable<T>, bool> argTests)
        {
            AssertResults(argTests);

            Assert.AreEqual(senderResults.Count, this.senderResults.Count);

            for (int i = 0; i < senderResults.Count; i++)
            {
                Assert.AreEqual(senderResults[i], this.senderResults[i]);
            }
        }
    }
}