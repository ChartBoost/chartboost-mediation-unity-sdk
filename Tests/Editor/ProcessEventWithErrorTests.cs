using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Helium
{
    public class ProcessEventWithErrorTests
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
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.AreEqual("An ad was not found.", error.errorDescription);
            };

            // The JSON string
            string json = "{\"errorCode\": 0, \"errorDescription\": \"An ad was not found.\"}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
        }

        [Test]
        public void NoAdFoundErrorCodeTest2()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"errorCode\": 0}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
        }

        [Test]
        public void NoBidErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoBid, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"errorCode\": 1}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
        }

        [Test]
        public void NoNetworkErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoNetwork, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"errorCode\": 2}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
        }

        [Test]
        public void ServerErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.ServerError, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            // The JSON string
            string json = "{\"errorCode\": 3}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
        }

        [Test]
        public void MinusOneErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<HeliumError> evt = (error) => { Assert.Null(error); };

            // The JSON string
            string json = "{\"errorCode\": -1}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
        }

        [Test]
        public void UnknownErrorCodeTest()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.Unknown, error.errorCode);
                Assert.Null(error.errorDescription);
            };

            string[] jsonsWithError = new string[]
            {
                "{\"errorCode\": 4}",
                "{\"errorCode\": -1234}",
                "{\"errorCode\": 1234}",
            };

            foreach (string json in jsonsWithError)
            {
                // Process the event
                eventProcessor.ProcessEventWithError(json, evt);
            }
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<HeliumError> evt = (error) => { Assert.Fail(); };

            // Process the event
            eventProcessor.ProcessEventWithError("", evt);
        }

        [Test]
        public void BlankJSONTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) => { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<HeliumError> evt = (error) =>
            {
                Assert.NotNull(error);
                Assert.AreEqual(HeliumErrorCode.NoAdFound, error.errorCode);
            };

            // The JSON string
            string json = "{}";

            // Process the event
            eventProcessor.ProcessEventWithError(json, evt);
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
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<HeliumError> evt = (error) => { Assert.Fail(); };

            foreach (string notJSONString in notJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithError(notJSONString, evt);
            }
        }

        [Test]
        public void UnacceptedJSONTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            string[] unacceptedJSONStrings = new string[]
            {
                "[{\"errorCode\": 1}]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<HeliumError> evt = (error) => { Assert.Fail(); };

            foreach (string unacceptedJSONString in unacceptedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithError(unacceptedJSONString, evt);
            }
        }

        [Test]
        public void MalformedJSONTest()
        {
            string[] malformedJSONStrings = new string[]
            {
                "{[\"errorCode\": 2]}",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with error", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<HeliumError> evt = (error) => { Assert.Fail(); };

            foreach (string malformedJSONString in malformedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithError(malformedJSONString, evt);
            }
        }
    }
}