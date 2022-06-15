using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Helium
{
    public class ProcessEventWithRewardTests
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
        public void TypicalJSONTest1()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string> evt = (reward) => { StringAssert.IsMatch("500", reward); };

            // The JSON string
            string json = "{\"reward\": 500}";

            // Process the event
            eventProcessor.ProcessEventWithReward(json, evt);
        }

        [Test]
        public void TypicalJSONTest2()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string> evt = (reward) => { StringAssert.IsMatch("trophy:93282", reward); };

            // The JSON string
            string json = "{\"reward\": \"trophy:93282\"}";

            // Process the event
            eventProcessor.ProcessEventWithReward(json, evt);
        }

        [Test]
        public void TypicalJSONTest3()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = delegate { Assert.Fail(); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string> evt = (reward) => { StringAssert.IsMatch("500", reward); };

            // The JSON string
            string json = "{\"reward\": \"500\"}";

            // Process the event
            eventProcessor.ProcessEventWithReward(json, evt);
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string> evt = (reward) => { Assert.Fail(); };

            // Process the event
            eventProcessor.ProcessEventWithReward("", evt);
        }

        [Test]
        public void BlankJSONTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Reward object not included with JSON payload", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string> evt = (reward) => { Assert.Fail(); };

            // The JSON string
            string json = "{}";

            // Process the event
            eventProcessor.ProcessEventWithReward(json, evt);
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
                "[\"reward\": \"12345\"]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string> evt = (reward) => { Assert.Fail(); };

            foreach (string notJSONString in notJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithReward(notJSONString, evt);
            }
        }

        [Test]
        public void UnacceptedJSONTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            string[] unacceptedJSONStrings = new string[]
            {
                "[{\"reward\": \"trophy:555\"}]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string> evt = (reward) => { Assert.Fail(); };

            foreach (string unacceptedJSONString in unacceptedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithReward(unacceptedJSONString, evt);
            }
        }

        [Test]
        public void MalformedJSONTest()
        {
            string[] malformedJSONStrings = new string[]
            {
                "{[\"reward\": 10]}",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with reward", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string> evt = (reward) => { Assert.Fail(); };

            foreach (string malformedJSONString in malformedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithReward(malformedJSONString, evt);
            }
        }
    }
}