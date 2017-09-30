using System;
using NUnit.Framework;

namespace GrimoireTD.Tests
{
    public class EventTester<T> where T : EventArgs
    {
        public bool EventFired { get; private set; } = false;

        public object SenderResult { get; private set; } = null;

        public T ArgsResult { get; private set; } = null;

        public EventHandler<T> Handler { get; }

        public EventTester()
        {
            Handler = new EventHandler<T>((sender, args) =>
            {
                EventFired = true;
                SenderResult = sender;
                ArgsResult = args;
            });
        }

        public void AssertFired(bool eventFired)
        {
            Assert.AreEqual(eventFired, EventFired);
        }

        public void AssertResults(object senderResult, Func<T, bool> argTests)
        {
            Assert.True(EventFired);
            Assert.AreEqual(senderResult, SenderResult);
            Assert.True(argTests(ArgsResult));
        }
    }
}