using Chartboost.Mediation.Ad.Banner;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class LayoutParamsTests
    {
        [Test]
        public void LayoutParamsInitializesCorrectly()
        {
            var layoutParams = new LayoutParams
            {
                x = TestConstants.Dimensions.TestX,
                y = TestConstants.Dimensions.TestY,
                width = TestConstants.Dimensions.StandardWidth,
                height = TestConstants.Dimensions.StandardHeight
            };

            Assert.AreEqual(TestConstants.Dimensions.TestX, layoutParams.x);
            Assert.AreEqual(TestConstants.Dimensions.TestY, layoutParams.y);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, layoutParams.width);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, layoutParams.height);
        }

        [Test]
        public void LayoutParamsWithZeroValuesInitializesCorrectly()
        {
            var layoutParams = new LayoutParams
            {
                x = 0f,
                y = 0f,
                width = 0f,
                height = 0f
            };

            Assert.AreEqual(0f, layoutParams.x);
            Assert.AreEqual(0f, layoutParams.y);
            Assert.AreEqual(0f, layoutParams.width);
            Assert.AreEqual(0f, layoutParams.height);
        }

        [Test]
        public void LayoutParamsWithNegativeValuesInitializesCorrectly()
        {
            var layoutParams = new LayoutParams
            {
                x = -10f,
                y = -20f,
                width = -30f,
                height = -40f
            };

            Assert.AreEqual(-10f, layoutParams.x);
            Assert.AreEqual(-20f, layoutParams.y);
            Assert.AreEqual(-30f, layoutParams.width);
            Assert.AreEqual(-40f, layoutParams.height);
        }

        [Test]
        public void TopLeftVector2CanBeSetAndRetrieved()
        {
            var layoutParams = new LayoutParams
            {
                topLeft = new Vector2(TestConstants.Dimensions.TestTopLeftX, TestConstants.Dimensions.TestTopLeftY)
            };

            Assert.AreEqual(TestConstants.Dimensions.TestTopLeftX, layoutParams.topLeft.x);
            Assert.AreEqual(TestConstants.Dimensions.TestTopLeftY, layoutParams.topLeft.y);
        }

        [Test]
        public void BottomLeftVector2CanBeSetAndRetrieved()
        {
            var layoutParams = new LayoutParams
            {
                bottomLeft = new Vector2(TestConstants.Dimensions.TestBottomLeftX, TestConstants.Dimensions.TestBottomLeftY)
            };

            Assert.AreEqual(TestConstants.Dimensions.TestBottomLeftX, layoutParams.bottomLeft.x);
            Assert.AreEqual(TestConstants.Dimensions.TestBottomLeftY, layoutParams.bottomLeft.y);
        }

        [Test]
        public void TopRightVector2CanBeSetAndRetrieved()
        {
            var layoutParams = new LayoutParams
            {
                topRight = new Vector2(TestConstants.Dimensions.TestTopRightX, TestConstants.Dimensions.TestTopRightY)
            };

            Assert.AreEqual(TestConstants.Dimensions.TestTopRightX, layoutParams.topRight.x);
            Assert.AreEqual(TestConstants.Dimensions.TestTopRightY, layoutParams.topRight.y);
        }

        [Test]
        public void BottomRightVector2CanBeSetAndRetrieved()
        {
            var layoutParams = new LayoutParams
            {
                bottomRight = new Vector2(TestConstants.Dimensions.TestBottomRightX, TestConstants.Dimensions.TestBottomRightY)
            };

            Assert.AreEqual(TestConstants.Dimensions.TestBottomRightX, layoutParams.bottomRight.x);
            Assert.AreEqual(TestConstants.Dimensions.TestBottomRightY, layoutParams.bottomRight.y);
        }

        [Test]
        public void AllFieldsCanBeSetTogether()
        {
            var layoutParams = new LayoutParams
            {
                x = TestConstants.Dimensions.TestX,
                y = TestConstants.Dimensions.TestY,
                width = TestConstants.Dimensions.StandardWidth,
                height = TestConstants.Dimensions.StandardHeight,
                topLeft = new Vector2(TestConstants.Dimensions.TestTopLeftX, TestConstants.Dimensions.TestTopLeftY),
                bottomLeft = new Vector2(TestConstants.Dimensions.TestBottomLeftX, TestConstants.Dimensions.TestBottomLeftY),
                topRight = new Vector2(TestConstants.Dimensions.TestTopRightX, TestConstants.Dimensions.TestTopRightY),
                bottomRight = new Vector2(TestConstants.Dimensions.TestBottomRightX, TestConstants.Dimensions.TestBottomRightY)
            };

            Assert.AreEqual(TestConstants.Dimensions.TestX, layoutParams.x);
            Assert.AreEqual(TestConstants.Dimensions.TestY, layoutParams.y);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, layoutParams.width);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, layoutParams.height);
            Assert.AreEqual(new Vector2(TestConstants.Dimensions.TestTopLeftX, TestConstants.Dimensions.TestTopLeftY), layoutParams.topLeft);
            Assert.AreEqual(new Vector2(TestConstants.Dimensions.TestBottomLeftX, TestConstants.Dimensions.TestBottomLeftY), layoutParams.bottomLeft);
            Assert.AreEqual(new Vector2(TestConstants.Dimensions.TestTopRightX, TestConstants.Dimensions.TestTopRightY), layoutParams.topRight);
            Assert.AreEqual(new Vector2(TestConstants.Dimensions.TestBottomRightX, TestConstants.Dimensions.TestBottomRightY), layoutParams.bottomRight);
        }

        [Test]
        public void JsonSerializationIncludesPositionAndSize()
        {
            var layoutParams = new LayoutParams
            {
                x = TestConstants.Dimensions.TestX,
                y = TestConstants.Dimensions.TestY,
                width = TestConstants.Dimensions.StandardWidth,
                height = TestConstants.Dimensions.StandardHeight
            };

            var json = JsonConvert.SerializeObject(layoutParams);

            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("\"x\""));
            Assert.IsTrue(json.Contains("\"y\""));
            Assert.IsTrue(json.Contains("\"width\""));
            Assert.IsTrue(json.Contains("\"height\""));
        }

        [Test]
        public void JsonSerializationExcludesVector2Fields()
        {
            var layoutParams = new LayoutParams
            {
                x = TestConstants.Dimensions.TestX,
                y = TestConstants.Dimensions.TestY,
                width = TestConstants.Dimensions.StandardWidth,
                height = TestConstants.Dimensions.StandardHeight,
                topLeft = new Vector2(TestConstants.Dimensions.TestTopLeftX, TestConstants.Dimensions.TestTopLeftY),
                bottomLeft = new Vector2(TestConstants.Dimensions.TestBottomLeftX, TestConstants.Dimensions.TestBottomLeftY),
                topRight = new Vector2(TestConstants.Dimensions.TestTopRightX, TestConstants.Dimensions.TestTopRightY),
                bottomRight = new Vector2(TestConstants.Dimensions.TestBottomRightX, TestConstants.Dimensions.TestBottomRightY)
            };

            var json = JsonConvert.SerializeObject(layoutParams);

            Assert.IsNotNull(json);
            Assert.IsFalse(json.Contains("topLeft"));
            Assert.IsFalse(json.Contains("bottomLeft"));
            Assert.IsFalse(json.Contains("topRight"));
            Assert.IsFalse(json.Contains("bottomRight"));
        }

        [Test]
        public void JsonDeserializationRestoresPositionAndSize()
        {
            const string json = "{\"x\":10.0,\"y\":20.0,\"width\":320.0,\"height\":50.0}";

            var layoutParams = JsonConvert.DeserializeObject<LayoutParams>(json);

            Assert.IsNotNull(layoutParams);
            Assert.AreEqual(TestConstants.Dimensions.TestX, layoutParams.x);
            Assert.AreEqual(TestConstants.Dimensions.TestY, layoutParams.y);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, layoutParams.width);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, layoutParams.height);
        }

        [Test]
        public void DefaultConstructorInitializesWithDefaultValues()
        {
            var layoutParams = new LayoutParams();

            Assert.AreEqual(0f, layoutParams.x);
            Assert.AreEqual(0f, layoutParams.y);
            Assert.AreEqual(0f, layoutParams.width);
            Assert.AreEqual(0f, layoutParams.height);
            Assert.AreEqual(Vector2.zero, layoutParams.topLeft);
            Assert.AreEqual(Vector2.zero, layoutParams.bottomLeft);
            Assert.AreEqual(Vector2.zero, layoutParams.topRight);
            Assert.AreEqual(Vector2.zero, layoutParams.bottomRight);
        }
    }
}
