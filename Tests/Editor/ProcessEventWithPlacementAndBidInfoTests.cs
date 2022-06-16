using System;
using NUnit.Framework;

namespace Helium
{
    public class ProcessEventWithPlacementAndBidInfo
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
            _unexpectedSystemErrorDidOccurEvent = Assert.Fail;
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                StringAssert.IsMatch("TypicalJSONTest1", placementName);
                StringAssert.IsMatch("abcdefg", bidInfo.AuctionId);
                Assert.AreEqual(2.99, bidInfo.Price);
                StringAssert.IsMatch("chair", bidInfo.Seat);
                StringAssert.IsMatch("TypicalJSONTest1", bidInfo.PartnerPlacementName);
            }

            // The JSON string
            const string json = "{\"placementName\": \"TypicalJSONTest1\", \"info\": {\"auction-id\": \"abcdefg\", \"price\": 2.99, \"seat\": \"chair\", \"placementName\": \"TypicalJSONTest1\"}}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(json, Event);
        }

        [Test]
        public void TypicalJsonTest2()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = Assert.Fail;
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                StringAssert.IsMatch("TypicalJSONTest2", placementName);
                StringAssert.IsMatch("1234567", bidInfo.AuctionId);
                Assert.AreEqual(33.67, bidInfo.Price);
                Assert.Null(bidInfo.Seat);
                Assert.Null(bidInfo.PartnerPlacementName);
            }

            // The JSON string
            const string json = "{\"placementName\": \"TypicalJSONTest2\", \"info\": {\"auction-id\": \"1234567\", \"price\": 33.67 }}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(json, Event);
        }

        [Test]
        public void TypicalJsonTest3()
        {
            // Should NOT get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = Assert.Fail;
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                StringAssert.IsMatch("TypicalJSONTest3", placementName);
                StringAssert.IsMatch("𝘈Ḇ𝖢𝕯٤ḞԍНǏ𝙅ƘԸⲘ𝙉০Ρ𝗤Ɍ𝓢", bidInfo.AuctionId);
                Assert.AreEqual(91.56, bidInfo.Price);
                Assert.Null(bidInfo.Seat);
                Assert.Null(bidInfo.PartnerPlacementName);
            }

            // The JSON string
            const string json = "{\"placementName\": \"TypicalJSONTest3\", \"info\": {\"auction-id\": \"𝘈Ḇ𝖢𝕯٤ḞԍНǏ𝙅ƘԸⲘ𝙉০Ρ𝗤Ɍ𝓢\", \"price\": \"91.56\" }}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(json, Event);
        }

        [Test]
        public void BlankStringTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.IsMatch("Non JSON data received when processing event with placement and bid info: ",
                    message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                Assert.Fail();
            }

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo("", Event);
        }

        [Test]
        public void BlankJsonTest()
        {
            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.IsMatch("Placement name not provided at root of: \\{}", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                Assert.Fail();
            }

            // The JSON string
            const string json = "{}";

            // Process the event
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(json, Event);
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
                "[\"placementName\": \"NotJSONTest\"]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement and bid info", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                Assert.Fail();
            }

            foreach (var notJsonString in notJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(notJsonString, Event);
            }
        }

        [Test]
        public void UnacceptedJsonTest()
        {
            // these are JSON but they aren't accepted due to the use case or implementation by design
            var unacceptedJsonStrings = new[]
            {
                "[{\"placementName\": \"NotJSONTest\"}]",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Non JSON data received when processing event with placement and bid info", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                Assert.Fail();
            }

            foreach (var unacceptedJsonString in unacceptedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(unacceptedJsonString, Event);
            }
        }

        [Test]
        public void MalformedJsonTest()
        {
            var malformedJsonStrings = new[]
            {
                "{[\"placementName\": \"NotJSONTest\"]}",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Malformed data received when processing event with placement and bid info", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                Assert.Fail();
            }

            foreach (var malformedJsonString in malformedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(malformedJsonString, Event);
            }
        }

        [Test]
        public void NoPlacementNameTest()
        {
            // these are JSON but they aren't accepted due to the use case
            var unacceptedJsonStrings = new[]
            {
                "{\"foo\": \"bar\"}",
                "{\"info\": {\"auction-id\": \"abcdefg\", \"price\": 2.99, \"seat\": \"chair\", \"placementName\": \"ProcessEventWithPlacementAndBidInfoTest\"}}",
            };

            // Should get an unexpected system error event
            _unexpectedSystemErrorDidOccurEvent = (message) =>
            {
                StringAssert.StartsWith("Placement name not provided at root of", message);
            };
            HeliumEventProcessor.UnexpectedSystemErrorDidOccur += _unexpectedSystemErrorDidOccurEvent;

            // Should NOT get an expected system event
            void Event(string placementName, HeliumBidInfo bidInfo)
            {
                Assert.Fail();
            }

            foreach (var unacceptedJsonString in unacceptedJsonStrings)
            {
                // Process the event
                HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(unacceptedJsonString, Event);
            }
        }
    }
}