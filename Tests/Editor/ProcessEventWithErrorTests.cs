using System;
using NUnit.Framework;

namespace Helium
{
    public class ProcessEventWithErrorTests
    {
        private Action<string> _unexpectedSystemErrorDidOccurEvent;

        [TearDown]
        public void Teardown()
        {
            if (_unexpectedSystemErrorDidOccurEvent != null)
                HeliumEventProcessor.UnexpectedSystemErrorDidOccur -= _unexpectedSystemErrorDidOccurEvent;
        }

        [Test]
        public void NoAdFoundErrorCodeTest1()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.AreEqual("An ad was not found.", error.errorDescription);
            }

            // The JSON string
            const string json = "{\"errorCode\": 0, \"errorDescription\": \"An ad was not found.\"}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
        }

        [Test]
        public void NoAdFoundErrorCodeTest2()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"errorCode\": 0}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
        }

        [Test]
        public void NoBidErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoBid, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"errorCode\": 1}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
        }

        [Test]
        public void NoNetworkErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoNetwork, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"errorCode\": 2}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
        }

        [Test]
        public void ServerErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.ServerError, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"errorCode\": 3}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
        }

        [Test]
        public void MinusOneErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.Null(error);
            }

            // The JSON string
            const string json = "{\"errorCode\": -1}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
        }

        [Test]
        public void UnknownErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.Unknown, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            var jsonsWithError = new[]
            {
                "{\"errorCode\": 4}",
                "{\"errorCode\": -1234}",
                "{\"errorCode\": 1234}",
            };

            foreach (var json in jsonsWithError)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithError(json, Event);
            }
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(HeliumError error)
            {
                Assert.Fail();
            }

            // Process the event
            HeliumEventProcessor.ProcessEventWithError("", Event);
        }

        [Test]
        public void BlankJsonTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = _ => { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(HeliumError error)
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
            }

            // The JSON string
            const string json = "{}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithError(json, Event);
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
                "[\"errorCode\": \"3\"]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var notJsonString in notJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithError(notJsonString, Event);
            }
        }

        [Test]
        public void UnacceptedJsonTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            var unacceptedJsonStrings = new[]
            {
                "[{\"errorCode\": 1}]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var unacceptedJsonString in unacceptedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithError(unacceptedJsonString, Event);
            }
        }

        [Test]
        public void MalformedJsonTest()
        {
            var malformedJsonStrings = new[]
            {
                "{[\"errorCode\": 2]}",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var malformedJsonString in malformedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithError(malformedJsonString, Event);
            }
        }
    }
}