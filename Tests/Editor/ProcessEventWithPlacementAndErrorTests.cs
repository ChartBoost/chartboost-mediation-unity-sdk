using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Helium
{
    public class ProcessEventWithPlacementAndErrorTests
    {
        HeliumEventProcessor eventProcessor;
        Action<string> unexpectedSystemErrorDidOccurEvent;

        [SetUp]
        public void Setup()
        {
            eventProcessor = new HeliumEventProcessor();
        }

        [TearDown]
        public void Teardown()
        {
            if (unexpectedSystemErrorDidOccurEvent != null)
                HeliumEventProcessor.UnexpectedSystemErrorDidOccur -= unexpectedSystemErrorDidOccurEvent;
        }

        [Test]
        public void NoAdFoundErrorCodeTest1()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("NoAdFoundErrorCodeTest1", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.AreEqual("An ad was not found.", error.errorDescription);
            };

            // The JSON string
            string json =
                "{\"placementName\": \"NoAdFoundErrorCodeTest1\", \"errorCode\": 0, \"errorDescription\": \"An ad was not found.\"}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void NoAdFoundErrorCodeTest2()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("NoAdFoundErrorCodeTest2", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"placementName\": \"NoAdFoundErrorCodeTest2\", \"errorCode\": 0}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void NoBidErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("NoBidErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoBid, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"placementName\": \"NoBidErrorCodeTest\", \"errorCode\": 1}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void NoNetworkErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("NoNetworkErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoNetwork, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"placementName\": \"NoNetworkErrorCodeTest\", \"errorCode\": 2}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void ServerErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("ServerErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.ServerError, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"placementName\": \"ServerErrorCodeTest\", \"errorCode\": 3}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void MinusOneErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("MinusOneErrorCodeTest", placementName);
                Assert.Null(error);
            };

            // The JSON string
            string json = "{\"placementName\": \"MinusOneErrorCodeTest\", \"errorCode\": -1}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void UnknownErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumError> evt = (placementName, error) =>
            {
                Assert.AreEqual("UnknownErrorCodeTest", placementName);
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.Unknown, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            string[] jsonsWithError = new string[]
            {
                "{\"placementName\": \"UnknownErrorCodeTest\", \"errorCode\": 4}",
                "{\"placementName\": \"UnknownErrorCodeTest\", \"errorCode\": -1234}",
                "{\"placementName\": \"UnknownErrorCodeTest\", \"errorCode\": 1234}",
            };

            foreach (string json in jsonsWithError)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndError(json, evt);
            }
        }

        [Test]
        public void NoPlacementNameTest()
        {
            // these are JSON but they aren't accepted due to the use case
            string[] unacceptedJSONStrings = new string[]
            {
                "{\"foo\": \"bar\"}",
                "{\"error\": 4}",
                "{\"error\": \"1\"}",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Placement name not provided at root of", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumError> evt = (placementName, error) => { Assert.Fail(); };

            foreach (string unacceptedJSONString in unacceptedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndError(unacceptedJSONString, evt);
            }
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumError> evt = (placementName, error) => { Assert.Fail(); };

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError("", evt);
        }

        [Test]
        public void BlankJSONTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Placement name not provided at root of", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumError> evt = (placementName, error) => { Assert.Fail(); };

            // The JSON string
            string json = "{}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndError(json, evt);
        }

        [Test]
        public void NotJSONTest()
        {
            string[] notJSONStrings = new string[]
            {
                " ",
                "x",
                "{",
                "}",
                "\\\\",
                "[\"errorCode\": \"3\"]",
                "[\"placementName\": \"NotJSONTest\"]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumError> evt = (placementName, error) => { Assert.Fail(); };

            foreach (string notJSONString in notJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndError(notJSONString, evt);
            }
        }

        [Test]
        public void UnacceptedJSONTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            string[] unacceptedJSONStrings = new string[]
            {
                "[{\"errorCode\": 1}]",
                "[{\"placementName\": \"UnacceptedJSONTest\"}]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumError> evt = (placementName, error) => { Assert.Fail(); };

            foreach (string unacceptedJSONString in unacceptedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndError(unacceptedJSONString, evt);
            }
        }

        [Test]
        public void MalformedJSONTest()
        {
            string[] malformedJSONStrings = new string[]
            {
                "{[\"errorCode\": 2]}",
                "{[\"placementName\": \"MalformedJSONTest\"]}",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with placement", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumError> evt = (placementName, error) => { Assert.Fail(); };

            foreach (string malformedJSONString in malformedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndError(malformedJSONString, evt);
            }
        }
    }
}