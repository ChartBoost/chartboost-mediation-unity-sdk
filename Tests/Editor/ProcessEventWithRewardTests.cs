using System;
using NUnit.Framework;

namespace Helium
{
    public class ProcessEventWithRewardTests
    {
        private Action<string> _unexpectedSystemErrorDidOccurEvent;

        [TearDown]
        public void Teardown()
        {
            if (_unexpectedSystemErrorDidOccurEvent != null)
                HeliumEventProcessor.UnexpectedSystemErrorDidOccur -= _unexpectedSystemErrorDidOccurEvent;
        }

        [Test]
        public void TypicalJsonTest1()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string reward)
            {
                StringAssert.IsMatch("500", reward);
            }

            // The JSON string
            const string json = "{\"reward\": 500}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithReward(json, Event);
        }

        [Test]
        public void TypicalJsonTest2()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string reward)
            {
                StringAssert.IsMatch("trophy:93282", reward);
            }

            // The JSON string
            const string json = "{\"reward\": \"trophy:93282\"}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithReward(json, Event);
        }

        [Test]
        public void TypicalJsonTest3()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string reward)
            {
                StringAssert.IsMatch("500", reward);
            }

            // The JSON string
            const string json = "{\"reward\": \"500\"}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithReward(json, Event);
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string reward)
            {
                Assert.Fail();
            }

            // Process the event
            HeliumEventProcessor.ProcessEventWithReward("", Event);
        }

        [Test]
        public void BlankJsonTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Reward object not included with JSON payload", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string reward)
            {
                Assert.Fail();
            }

            // The JSON string
            const string json = "{}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithReward(json, Event);
        }

        [Test]
        public void NotJsonTest()
        {
            var notJsonStrings = new[]
            {
                " ",
                "x",
                "{",
                "}",
                "\\\\",
                "[\"reward\": \"12345\"]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string reward)
            {
                Assert.Fail();
            }

            foreach (var notJsonString in notJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithReward(notJsonString, Event);
            }
        }

        [Test]
        public void UnacceptedJsonTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            var unacceptedJsonStrings = new[]
            {
                "[{\"reward\": \"trophy:555\"}]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string reward)
            {
                Assert.Fail();
            }

            foreach (var unacceptedJsonString in unacceptedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithReward(unacceptedJsonString, Event);
            }
        }

        [Test]
        public void MalformedJsonTest()
        {
            var malformedJsonStrings = new[]
            {
                "{[\"reward\": 10]}",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string reward)
            {
                Assert.Fail();
            }

            foreach (var malformedJsonString in malformedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithReward(malformedJsonString, Event);
            }
        }
    }
}