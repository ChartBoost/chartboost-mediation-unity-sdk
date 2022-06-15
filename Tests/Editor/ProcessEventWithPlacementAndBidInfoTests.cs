using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Helium
{
    public class ProcessEventWithPlacementAndBidInfo
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
            unexpectedSystemErrorDidOccurEvent = (message) => { Assert.Fail(message); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) =>
            {
                StringAssert.IsMatch("TypicalJSONTest1", placementName);
                StringAssert.IsMatch("abcdefg", bidInfo.AuctionId);
                Assert.AreEqual(2.99, bidInfo.Price);
                StringAssert.IsMatch("chair", bidInfo.Seat);
                StringAssert.IsMatch("TypicalJSONTest1", bidInfo.PartnerPlacementName);
            };

            // The JSON string
            string json =
                "{\"placementName\": \"TypicalJSONTest1\", \"info\": {\"auction-id\": \"abcdefg\", \"price\": 2.99, \"seat\": \"chair\", \"placementName\": \"TypicalJSONTest1\"}}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndBidInfo(json, evt);
        }

        [Test]
        public void TypicalJSONTest2()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) => { Assert.Fail(message); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) =>
            {
                StringAssert.IsMatch("TypicalJSONTest2", placementName);
                StringAssert.IsMatch("1234567", bidInfo.AuctionId);
                Assert.AreEqual(33.67, bidInfo.Price);
                Assert.Null(bidInfo.Seat);
                Assert.Null(bidInfo.PartnerPlacementName);
            };

            // The JSON string
            string json =
                "{\"placementName\": \"TypicalJSONTest2\", \"info\": {\"auction-id\": \"1234567\", \"price\": 33.67 }}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndBidInfo(json, evt);
        }

        [Test]
        public void TypicalJSONTest3()
        {
            // Should NOT get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) => { Assert.Fail(message); };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) =>
            {
                StringAssert.IsMatch("TypicalJSONTest3", placementName);
                StringAssert.IsMatch("𝘈Ḇ𝖢𝕯٤ḞԍНǏ𝙅ƘԸⲘ𝙉০Ρ𝗤Ɍ𝓢", bidInfo.AuctionId);
                Assert.AreEqual(91.56, bidInfo.Price);
                Assert.Null(bidInfo.Seat);
                Assert.Null(bidInfo.PartnerPlacementName);
            };

            // The JSON string
            string json =
                "{\"placementName\": \"TypicalJSONTest3\", \"info\": {\"auction-id\": \"𝘈Ḇ𝖢𝕯٤ḞԍНǏ𝙅ƘԸⲘ𝙉০Ρ𝗤Ɍ𝓢\", \"price\": \"91.56\" }}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndBidInfo(json, evt);
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.IsMatch("Non JSON data received when processing event with placement and bid info: ",
                    message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) => { Assert.Fail(); };

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndBidInfo("", evt);
        }

        [Test]
        public void BlankJSONTest()
        {
            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.IsMatch("Placement name not provided at root of: \\{}", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) => { Assert.Fail(); };

            // The JSON string
            string json = "{}";

            // Process the event
            eventProcessor.ProcessEventWithPlacementAndBidInfo(json, evt);
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
                "[\"placementName\": \"NotJSONTest\"]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement and bid info",
                    message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) => { Assert.Fail(); };

            foreach (string notJSONString in notJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndBidInfo(notJSONString, evt);
            }
        }

        [Test]
        public void UnacceptedJSONTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            string[] unacceptedJSONStrings = new string[]
            {
                "[{\"placementName\": \"NotJSONTest\"}]",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement and bid info",
                    message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) => { Assert.Fail(); };

            foreach (string unacceptedJSONString in unacceptedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndBidInfo(unacceptedJSONString, evt);
            }
        }

        [Test]
        public void MalformedJSONTest()
        {
            string[] malformedJSONStrings = new string[]
            {
                "{[\"placementName\": \"NotJSONTest\"]}",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with placement and bid info",
                    message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) => { Assert.Fail(); };

            foreach (string malformedJSONString in malformedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndBidInfo(malformedJSONString, evt);
            }
        }

        [Test]
        public void NoPlacementNameTest()
        {
            // these are JSON but they aren't accepted due to the use case
            string[] unacceptedJSONStrings = new string[]
            {
                "{\"foo\": \"bar\"}",
                "{\"info\": {\"auction-id\": \"abcdefg\", \"price\": 2.99, \"seat\": \"chair\", \"placementName\": \"ProcessEventWithPlacementAndBidInfoTest\"}}",
            };

            // Should get an unexepected system error event
            unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Placement name not provided at root of", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            Action<string, HeliumBidInfo> evt = (placementName, bidInfo) => { Assert.Fail(); };

            foreach (string unacceptedJSONString in unacceptedJSONStrings)
            {
                // Process the event
                eventProcessor.ProcessEventWithPlacementAndBidInfo(unacceptedJSONString, evt);
            }
        }
    }
}