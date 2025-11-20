using System.Runtime.CompilerServices;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Displays test progress information on the device screen during test execution.
    /// Useful for debugging and monitoring which tests are running, especially when some tests take longer than others.
    /// </summary>
    public class TestProgressTracker : MonoBehaviour
    {
        private static TestProgressTracker _instance;
        private string _currentTestName = "Initializing...";
        private int _currentTestNumber;
        private int _totalTests;
        private string _testFixtureName = "";
        private float _testStartTime;
        private bool _isVisible = true;

        // UI styling
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _headerStyle;
        private bool _stylesInitialized;

        /// <summary>
        /// Gets or creates the singleton instance of TestProgressTracker.
        /// </summary>
        private static TestProgressTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("TestProgressTracker");
                    _instance = go.AddComponent<TestProgressTracker>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Notifies the progress display that a test is starting.
        /// Automatically extracts the test method name from the caller.
        /// </summary>
        /// <param name="testFixtureName">Name of the test fixture (test class)</param>
        /// <param name="testNumber">Current test number (1-based, matching the Order attribute)</param>
        /// <param name="totalTests">Total number of tests in the fixture</param>
        /// <param name="testMethodName">Auto-populated with the calling method name</param>
        public static void NotifyTestStart(string testFixtureName, int testNumber, int totalTests, [CallerMemberName] string testMethodName = "")
        {
            var instance = Instance;
            instance._testFixtureName = testFixtureName;
            instance._currentTestName = testMethodName;
            instance._currentTestNumber = testNumber;
            instance._totalTests = totalTests;
            instance._testStartTime = Time.realtimeSinceStartup;
            instance._isVisible = true; // Ensure visible when updated
        }

        /// <summary>
        /// Shows the test progress display on screen.
        /// </summary>
        public static void Show()
        {
            Instance._isVisible = true;
        }

        /// <summary>
        /// Hides the test progress display.
        /// </summary>
        public static void Hide()
        {
            Instance._isVisible = false;
        }

        /// <summary>
        /// Clears the test progress display content and hides it.
        /// Call this in OneTimeTearDown to clear the display when a test fixture completes.
        /// </summary>
        public static void Clear()
        {
            var instance = Instance;
            instance._testFixtureName = "";
            instance._currentTestName = "";
            instance._currentTestNumber = 0;
            instance._totalTests = 0;
            instance._isVisible = false;
        }

        private void InitializeStyles()
        {
            if (_stylesInitialized)
                return;

            // Box style with semi-transparent black background
            _boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(15, 15, 15, 15),
                margin = new RectOffset(10, 10, 10, 10),
                normal = { background = MakeTexture(2, 2, new Color(0, 0, 0, 0.85f)) }
            };

            // Header style for test fixture name - larger font for readability
            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };

            // Label style for test information - larger font for readability
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };

            _stylesInitialized = true;
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            var pixels = new Color[width * height];
            for (var i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            var texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private void OnGUI()
        {
            if (!_isVisible)
                return;

            InitializeStyles();

            // Calculate elapsed time
            var elapsedTime = Time.realtimeSinceStartup - _testStartTime;
            var progressPercentage = _totalTests > 0 ? (_currentTestNumber * 100f) / _totalTests : 0;

            // Position at center-top: horizontally centered, vertically offset 25% from top
            var boxWidth = Screen.width * 0.9f;
            var boxHeight = 220f; // Increased height to accommodate larger fonts and prevent cut-off
            var xPosition = (Screen.width - boxWidth) / 2f; // Center horizontally
            var yPosition = Screen.height * 0.25f; // 25% from top
            var rect = new Rect(xPosition, yPosition, boxWidth, boxHeight);

            // Draw background box
            GUI.Box(rect, "", _boxStyle);

            // Draw content with more padding
            var contentRect = new Rect(rect.x + 20, rect.y + 20, rect.width - 40, rect.height - 40);

            GUILayout.BeginArea(contentRect);

            // Test fixture name (header)
            if (!string.IsNullOrEmpty(_testFixtureName))
            {
                GUILayout.Label(_testFixtureName, _headerStyle);
                GUILayout.Space(5);
            }

            // Progress information
            GUILayout.Label($"Progress: {_currentTestNumber}/{_totalTests} ({progressPercentage:F1}%)", _labelStyle);
            GUILayout.Space(5);

            // Current test name
            GUILayout.Label($"Running: {_currentTestName}", _labelStyle);
            GUILayout.Space(5);

            // Elapsed time
            GUILayout.Label($"Elapsed: {elapsedTime:F2}s", _labelStyle);

            GUILayout.EndArea();
        }
    }
}
