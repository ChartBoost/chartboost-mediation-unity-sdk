using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;

/// <summary>
/// This suite of tests is intended to directly validate the parsing of JSON
/// (or alleged JSON) using whatever parser is provided by the SDK.
///
/// Today, the parser is `JsonTools.HeliumJSON` in the file `JsonTools.cs`.
///
/// The suite uses the JSON text files in the directory Tests/TestJSON
/// which were borrowed from this Github repository: https://github.com/nst/JSONTestSuite.
/// Each file is prepended with single character, meaning:
///
/// i => the parser is free to accept or reject the content
/// y => the parser must accept the content
/// n => the parser must reject the content
///
/// Since the parser being used by the SDK may or may not be a completely exhaustive
/// implementation, there may also be files that are prepended with additional
/// characters, meaning:
///
/// s => skipped because the parser is not implemented to deal with this content
/// x => will be a KNOWN failure and someday we need to make it pass
/// 
/// </summary>

namespace Helium
{
    public class JsonParseTests
    {
        enum Acceptance
        {
            Optional,
            Accept,
            Reject
        }

        enum Expectation
        {
            Pass,
            Skip,
            KnownFailure
        };

        private string[] _testFiles;

        private const string JsonTestLocation = "Packages/com.chartboost.helium/Tests/TestJSON";

        [SetUp]
        public void Setup()
        {
            _testFiles = Directory.GetFiles(JsonTestLocation, "*.json");
        }

        [TearDown]
        public void TearDown()
        {
            _testFiles = null;
        }

        [Test]
        public void ExhaustiveJSONTest()
        {
            Debug.Log(string.Format("Running ExhaustiveJSONTest over a total of {0} files", _testFiles.Length));
            int skippedCount = 0;
            int expectedFails = 0;

            foreach (string testFile in _testFiles)
            {
                var fileName = Path.GetFileName(testFile);
                
                Expectation expectation = GetExpectation(fileName);
                if (expectation == Expectation.Skip)
                {
                    skippedCount++;
                    continue;
                }

                Acceptance acceptance = GetAcceptance(fileName);

                // Read in the JSON
                string path = testFile;
                StreamReader reader = new StreamReader(path);
                string json = reader.ReadToEnd();
                reader.Close();

                object rawResult = HeliumJSON.Deserialize(json);
                switch (acceptance)
                {
                    case Acceptance.Optional:
                        if (rawResult != null)
                            Debug.Log(string.Format("optional acceptance was accepted: {0}", testFile));
                        break;
                    case Acceptance.Accept:
                        if (testFile.Contains("lonely_null"))
                        {
                            if (expectation == Expectation.KnownFailure)
                            {
                                Assert.NotNull(rawResult,
                                    string.Format("known failure but will have passed: {0}", testFile));
                                expectedFails++;
                            }
                            else
                                Assert.Null(rawResult, string.Format("expected to not fail: {0}", testFile));
                        }
                        else
                        {
                            if (expectation == Expectation.KnownFailure)
                            {
                                Assert.Null(rawResult,
                                    string.Format("known failure but will have passed: {0}", testFile));
                                expectedFails++;
                            }
                            else
                                Assert.NotNull(rawResult, string.Format("expected to not fail: {0}", testFile));
                        }

                        break;
                    case Acceptance.Reject:
                        if (expectation == Expectation.KnownFailure)
                        {
                            Assert.NotNull(rawResult,
                                string.Format("reject known failure but will have passed: {0}", testFile));
                            expectedFails++;
                        }
                        else
                            Assert.Null(rawResult, string.Format("reject expected to not fail: {0}", testFile));

                        break;
                }
            }

            Debug.LogWarning(string.Format("Skipped files in ExhaustiveJSONTest: {0}", skippedCount));
            Debug.LogWarning(string.Format("Expected failed files in ExhaustiveJSONTest: {0}", expectedFails));
        }

        Acceptance GetAcceptance(string filename)
        {
            if (filename.StartsWith("i_") || filename.Contains("_i_"))
                return Acceptance.Optional;
            if (filename.StartsWith("y_") || filename.Contains("_y_"))
                return Acceptance.Accept;
            if (filename.StartsWith("n_") || filename.Contains("_n_"))
                return Acceptance.Reject;
            return Acceptance.Accept;
        }

        Expectation GetExpectation(string filename)
        {
            if (filename.StartsWith("s_"))
                return Expectation.Skip;
            if (filename.StartsWith("x_"))
                return Expectation.KnownFailure;
            return Expectation.Pass;
        }
    }
}
