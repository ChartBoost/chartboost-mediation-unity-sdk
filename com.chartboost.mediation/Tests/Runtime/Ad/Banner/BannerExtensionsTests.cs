using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using NUnit.Framework;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class BannerExtensionsTests
    {
        private GameObject _testGameObject;
        private RectTransform _rectTransform;

        [SetUp]
        public void SetUp()
        {
            _testGameObject = new GameObject(nameof(BannerExtensionsTests));
            _rectTransform = _testGameObject.AddComponent<RectTransform>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
                Object.DestroyImmediate(_testGameObject);
        }

        [Test]
        public void LayoutParamsReturnsCorrectPositionAndSize()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams);
            Assert.Greater(layoutParams.width, TestConstants.Dimensions.Zero);
            Assert.Greater(layoutParams.height, TestConstants.Dimensions.Zero);
        }

        [Test]
        public void LayoutParamsReturnsCorrectCorners()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams);
            Assert.AreEqual(layoutParams.topLeft.x, layoutParams.x);
            Assert.AreEqual(layoutParams.topLeft.y, layoutParams.y);
        }

        [Test]
        public void LayoutParamsCalculatesWidthCorrectly()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();
            var expectedWidth = (int)(layoutParams.topRight.x - layoutParams.bottomLeft.x);

            Assert.AreEqual(expectedWidth, layoutParams.width);
        }

        [Test]
        public void LayoutParamsCalculatesHeightCorrectly()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();
            var expectedHeight = (int)(layoutParams.topLeft.y - layoutParams.bottomLeft.y);

            Assert.AreEqual(expectedHeight, layoutParams.height);
        }

        [Test]
        public void LayoutParamsBottomLeftCornerIsCorrect()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams.bottomLeft);
            Assert.Less(layoutParams.bottomLeft.y, layoutParams.topLeft.y);
        }

        [Test]
        public void LayoutParamsTopRightCornerIsCorrect()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams.topRight);
            Assert.Greater(layoutParams.topRight.x, layoutParams.topLeft.x);
        }

        [Test]
        public void LayoutParamsBottomRightCornerIsCorrect()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams.bottomRight);
            Assert.Greater(layoutParams.bottomRight.x, layoutParams.bottomLeft.x);
            Assert.Less(layoutParams.bottomRight.y, layoutParams.topRight.y);
        }

        [Test]
        public void IsEqualReturnsTrueForIdenticalLayoutParams()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            Assert.IsTrue(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void IsEqualReturnsTrueForLayoutParamsWithinTolerance()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX + TestConstants.Tolerance.Within,
                y = TestConstants.Dimensions.TestCoordY + TestConstants.Tolerance.Within,
                width = TestConstants.Dimensions.TestRectWidth + TestConstants.Tolerance.Within,
                height = TestConstants.Dimensions.TestRectHeight + TestConstants.Tolerance.Within
            };

            Assert.IsTrue(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void IsEqualReturnsFalseForDifferentX()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX + TestConstants.Tolerance.Outside,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            Assert.IsFalse(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void IsEqualReturnsFalseForDifferentY()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY + TestConstants.Tolerance.Outside,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            Assert.IsFalse(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void IsEqualReturnsFalseForDifferentWidth()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth + TestConstants.Tolerance.Outside,
                height = TestConstants.Dimensions.TestRectHeight
            };

            Assert.IsFalse(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void IsEqualReturnsFalseForDifferentHeight()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight + TestConstants.Tolerance.Outside
            };

            Assert.IsFalse(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void IsEqualReturnsTrueForZeroValues()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.Zero,
                y = TestConstants.Dimensions.Zero,
                width = TestConstants.Dimensions.Zero,
                height = TestConstants.Dimensions.Zero
            };

            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.Zero,
                y = TestConstants.Dimensions.Zero,
                width = TestConstants.Dimensions.Zero,
                height = TestConstants.Dimensions.Zero
            };

            Assert.IsTrue(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void SizeReturnsCorrectDimensionsForStandard()
        {
            var size = BannerSizeType.Standard.Size();

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.x);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, size.y);
        }

        [Test]
        public void SizeReturnsCorrectDimensionsForMedium()
        {
            var size = BannerSizeType.Medium.Size();

            Assert.AreEqual(TestConstants.Dimensions.MediumWidth, size.x);
            Assert.AreEqual(TestConstants.Dimensions.MediumHeight, size.y);
        }

        [Test]
        public void SizeReturnsCorrectDimensionsForLeaderboard()
        {
            var size = BannerSizeType.Leaderboard.Size();

            Assert.AreEqual(TestConstants.Dimensions.LeaderboardWidth, size.x);
            Assert.AreEqual(TestConstants.Dimensions.LeaderboardHeight, size.y);
        }

        [Test]
        public void SizeReturnsZeroVectorForAdaptive()
        {
            var size = BannerSizeType.Adaptive.Size();

            Assert.AreEqual(Vector2.zero, size);
        }

        [Test]
        public void SizeReturnsZeroVectorForUnknown()
        {
            var size = BannerSizeType.Unknown.Size();

            Assert.AreEqual(Vector2.zero, size);
        }

        [Test]
        public void SizeReturnsZeroVectorForInvalidValue()
        {
            var invalidValue = (BannerSizeType)999;
            var size = invalidValue.Size();

            Assert.AreEqual(Vector2.zero, size);
        }

        [Test]
        public void IsEqualToleranceIsExactly001()
        {
            var layoutParams1 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            // Test exactly at the tolerance boundary
            var layoutParams2 = new LayoutParams
            {
                x = TestConstants.Dimensions.TestCoordX + TestConstants.Tolerance.Assertion,
                y = TestConstants.Dimensions.TestCoordY,
                width = TestConstants.Dimensions.TestRectWidth,
                height = TestConstants.Dimensions.TestRectHeight
            };

            Assert.IsTrue(layoutParams1.IsEqual(layoutParams2));
        }

        [Test]
        public void LayoutParamsWithRotatedRectTransformCalculatesCorrectly()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);
            _rectTransform.rotation = Quaternion.Euler(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero, 45f);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams);
            Assert.Greater(layoutParams.width, TestConstants.Dimensions.Zero);
            Assert.Greater(layoutParams.height, TestConstants.Dimensions.Zero);
        }

        [Test]
        public void LayoutParamsWithScaledRectTransformCalculatesCorrectly()
        {
            _rectTransform.position = new Vector3(TestConstants.Dimensions.XPosition, TestConstants.Dimensions.YPosition, TestConstants.Dimensions.Zero);
            _rectTransform.sizeDelta = new Vector2(TestConstants.Dimensions.TestRectWidth, TestConstants.Dimensions.TestRectHeight);
            _rectTransform.localScale = new Vector3(2f, 2f, 1f);

            var layoutParams = _rectTransform.LayoutParams();

            Assert.IsNotNull(layoutParams);
            Assert.Greater(layoutParams.width, TestConstants.Dimensions.Zero);
            Assert.Greater(layoutParams.height, TestConstants.Dimensions.Zero);
        }
    }
}
