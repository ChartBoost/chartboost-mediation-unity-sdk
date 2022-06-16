using System;
using NUnit.Framework;

namespace Helium
{
    public class ProcessEventWithPlacementAndErrorTests
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
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("NoAdFoundErrorCodeTest1", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.AreEqual("An ad was not found.", error.errorDescription);
            }

            // The JSON string
            const string json = "{\"placementName\": \"NoAdFoundErrorCodeTest1\", \"errorCode\": 0, \"errorDescription\": \"An ad was not found.\"}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
        }

        [Test]
        public void NoAdFoundErrorCodeTest2()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("NoAdFoundErrorCodeTest2", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"placementName\": \"NoAdFoundErrorCodeTest2\", \"errorCode\": 0}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
        }

        [Test]
        public void NoBidErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("NoBidErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoBid, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"placementName\": \"NoBidErrorCodeTest\", \"errorCode\": 1}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
        }

        [Test]
        public void NoNetworkErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("NoNetworkErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoNetwork, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"placementName\": \"NoNetworkErrorCodeTest\", \"errorCode\": 2}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
        }

        [Test]
        public void ServerErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("ServerErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.ServerError, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            // The JSON string
            const string json = "{\"placementName\": \"ServerErrorCodeTest\", \"errorCode\": 3}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
        }

        [Test]
        public void MinusOneErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("MinusOneErrorCodeTest", placementName);
                Assert.Null(error);
            }

            // The JSON string
            const string json = "{\"placementName\": \"MinusOneErrorCodeTest\", \"errorCode\": -1}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
        }

        [Test]
        public void UnknownErrorCodeTest()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.AreEqual("UnknownErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.Unknown, error.errorCode);
                Assert.Null(error.errorDescription);
            }

            var jsonsWithError = new[]
            {
                "{\"placementName\": \"UnknownErrorCodeTest\", \"errorCode\": 4}",
                "{\"placementName\": \"UnknownErrorCodeTest\", \"errorCode\": -1234}",
                "{\"placementName\": \"UnknownErrorCodeTest\", \"errorCode\": 1234}",
            };

            foreach (var json in jsonsWithError)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
            }
        }

        [Test]
        public void NoPlacementNameTest()
        {
            // these are JSON but they aren't accepted due to the use case
            var unacceptedJsonStrings = new[]
            {
                "{\"foo\": \"bar\"}",
                "{\"error\": 4}",
                "{\"error\": \"1\"}",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Placement name not provided at root of", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var unacceptedJsonString in unacceptedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndError(unacceptedJsonString, Event);
            }
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.Fail();
            }

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError("", Event);
        }

        [Test]
        public void BlankJsonTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Placement name not provided at root of", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.Fail();
            }

            // The JSON string
            const string json = "{}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndError(json, Event);
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
                "[\"placementName\": \"NotJSONTest\"]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var notJsonString in notJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndError(notJsonString, Event);
            }
        }

        [Test]
        public void UnacceptedJsonTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            var unacceptedJsonStrings = new[]
            {
                "[{\"errorCode\": 1}]",
                "[{\"placementName\": \"UnacceptedJSONTest\"}]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var unacceptedJsonString in unacceptedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndError(unacceptedJsonString, Event);
            }
        }

        [Test]
        public void MalformedJsonTest()
        {
            var malformedJsonStrings = new[]
            {
                "{[\"errorCode\": 2]}",
                "{[\"placementName\": \"MalformedJSONTest\"]}",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumError error)
            {
                Assert.Fail();
            }

            foreach (var malformedJsonString in malformedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndError(malformedJsonString, Event);
            }
        }
    }
}