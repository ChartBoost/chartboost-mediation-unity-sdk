using NUnit.Framework;
using UnityEngine;
using System.IO;

namespace Helium
{
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
    public class JsonParseTests
    {
        private enum Acceptance
        {
            Optional,
            Accept,
            Reject
        }

        private enum Expectation
        {
            Pass,
            Skip,
            KnownFailure
        };

        private string[] _testFiles;

        private const string JsonTestLocation = "Packages/com.chartboost.mediation/Tests/TestJSON";

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
        public void ExhaustiveJsonTest()
        {
            Debug.Log($"Running ExhaustiveJSONTest over a total of {_testFiles.Length} files");
            var skippedCount = 0;
            var expectedFails = 0;

            foreach (string testFile in _testFiles)
            {
                var fileName = Path.GetFileName(testFile);
                
                var expectation = GetExpectation(fileName);
                if (expectation == Expectation.Skip)
                {
                    skippedCount++;
                    continue;
                }

                var acceptance = GetAcceptance(fileName);

                // Read in the JSON
                var reader = new StreamReader(testFile);
                var json = reader.ReadToEnd();
                reader.Close();

                var rawResult = HeliumJson.Deserialize(json);
                switch (acceptance)
                {
                    case Acceptance.Optional:
                        if (rawResult != null)
                            Debug.Log($"optional acceptance was accepted: {testFile}");
                        break;
                    case Acceptance.Accept:
                        if (testFile.Contains("lonely_null"))
                        {
                            if (expectation == Expectation.KnownFailure)
                            {
                                Assert.NotNull(rawResult, $"known failure but will have passed: {testFile}");
                                expectedFails++;
                            }
                            else
                                Assert.Null(rawResult, $"expected to not fail: {testFile}");
                        }
                        else
                        {
                            if (expectation == Expectation.KnownFailure)
                            {
                                Assert.Null(rawResult, $"known failure but will have passed: {testFile}");
                                expectedFails++;
                            }
                            else
                                Assert.NotNull(rawResult, $"expected to not fail: {testFile}");
                        }

                        break;
                    case Acceptance.Reject:
                        if (expectation == Expectation.KnownFailure)
                        {
                            Assert.NotNull(rawResult, $"reject known failure but will have passed: {testFile}");
                            expectedFails++;
                        }
                        else
                            Assert.Null(rawResult, $"reject expected to not fail: {testFile}");

                        break;
                }
            }

            Debug.LogWarning($"Skipped files in ExhaustiveJSONTest: {skippedCount}");
            Debug.LogWarning($"Expected failed files in ExhaustiveJSONTest: {expectedFails}");
        }

        private static Acceptance GetAcceptance(string filename)
        {
            if (filename.StartsWith("i_") || filename.Contains("_i_"))
                return Acceptance.Optional;
            if (filename.StartsWith("y_") || filename.Contains("_y_"))
                return Acceptance.Accept;
            if (filename.StartsWith("n_") || filename.Contains("_n_"))
                return Acceptance.Reject;
            return Acceptance.Accept;
        }

        private static Expectation GetExpectation(string filename)
        {
            if (filename.StartsWith("s_"))
                return Expectation.Skip;
            return filename.StartsWith("x_") ? Expectation.KnownFailure : Expectation.Pass;
        }
    }
}
