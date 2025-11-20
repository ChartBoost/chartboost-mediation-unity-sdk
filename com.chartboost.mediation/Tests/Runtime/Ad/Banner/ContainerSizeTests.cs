using Chartboost.Mediation.Ad.Banner;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class ContainerSizeTests
    {

        [Test]
        public void ConstructorInitializesCorrectly()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual((int)TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual((int)TestConstants.Dimensions.StandardHeight, size.Height);
        }

        [Test]
        public void WrapHorizontalCreatesCorrectSize()
        {
            var size = ContainerSize.WrapHorizontal((int)TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(TestConstants.Dimensions.WrapContent, size.Width);
            Assert.AreEqual((int)TestConstants.Dimensions.StandardHeight, size.Height);
        }

        [Test]
        public void WrapVerticalCreatesCorrectSize()
        {
            var size = ContainerSize.WrapVertical((int)TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual((int)TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.WrapContent, size.Height);
        }

        [Test]
        public void WrapContentCreatesCorrectSize()
        {
            var size = ContainerSize.WrapContent();

            Assert.AreEqual(TestConstants.Dimensions.WrapContent, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.WrapContent, size.Height);
        }

        [Test]
        public void FixedSizeCreatesCorrectSize()
        {
            var size = ContainerSize.FixedSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual((int)TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual((int)TestConstants.Dimensions.StandardHeight, size.Height);
        }

        [Test]
        public void ConstructorWithZeroWidthInitializesCorrectly()
        {
            var size = new ContainerSize(0, (int)TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(0, size.Width);
            Assert.AreEqual((int)TestConstants.Dimensions.StandardHeight, size.Height);
        }

        [Test]
        public void ConstructorWithZeroHeightInitializesCorrectly()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, 0);

            Assert.AreEqual((int)TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(0, size.Height);
        }

        [Test]
        public void ConstructorWithNegativeValuesInitializesCorrectly()
        {
            var size = new ContainerSize(-5, -10);

            Assert.AreEqual(-5, size.Width);
            Assert.AreEqual(-10, size.Height);
        }

        [Test]
        public void WrapHorizontalWithZeroHeightInitializesCorrectly()
        {
            var size = ContainerSize.WrapHorizontal(0);

            Assert.AreEqual(TestConstants.Dimensions.WrapContent, size.Width);
            Assert.AreEqual(0, size.Height);
        }

        [Test]
        public void WrapVerticalWithZeroWidthInitializesCorrectly()
        {
            var size = ContainerSize.WrapVertical(0);

            Assert.AreEqual(0, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.WrapContent, size.Height);
        }

        [Test]
        public void FixedSizeWithLargeValuesInitializesCorrectly()
        {
            const int largeWidth = 1920;
            const int largeHeight = 1080;
            var size = ContainerSize.FixedSize(largeWidth, largeHeight);

            Assert.AreEqual(largeWidth, size.Width);
            Assert.AreEqual(largeHeight, size.Height);
        }

        [Test]
        public void WidthPropertyIsReadOnly()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var width = size.Width;

            Assert.AreEqual((int)TestConstants.Dimensions.StandardWidth, width);
        }

        [Test]
        public void HeightPropertyIsReadOnly()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var height = size.Height;

            Assert.AreEqual((int)TestConstants.Dimensions.StandardHeight, height);
        }

        #region Equality Tests

        [Test]
        public void EqualsReturnsTrueForIdenticalSizes()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);

            Assert.IsTrue(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsFalseForDifferentWidths()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth + 1, (int)TestConstants.Dimensions.StandardHeight);

            Assert.IsFalse(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsFalseForDifferentHeights()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight + 1);

            Assert.IsFalse(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsFalseForDifferentWidthAndHeight()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth + 1, (int)TestConstants.Dimensions.StandardHeight + 1);

            Assert.IsFalse(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsTrueForWrapContentSizes()
        {
            var size1 = ContainerSize.WrapContent();
            var size2 = ContainerSize.WrapContent();

            Assert.IsTrue(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsTrueForWrapHorizontalSizes()
        {
            var size1 = ContainerSize.WrapHorizontal((int)TestConstants.Dimensions.StandardHeight);
            var size2 = ContainerSize.WrapHorizontal((int)TestConstants.Dimensions.StandardHeight);

            Assert.IsTrue(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsTrueForWrapVerticalSizes()
        {
            var size1 = ContainerSize.WrapVertical((int)TestConstants.Dimensions.StandardWidth);
            var size2 = ContainerSize.WrapVertical((int)TestConstants.Dimensions.StandardWidth);

            Assert.IsTrue(size1.Equals(size2));
        }

        [Test]
        public void EqualsObjectReturnsTrueForIdenticalSizes()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            object size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);

            Assert.IsTrue(size1.Equals(size2));
        }

        [Test]
        public void EqualsObjectReturnsFalseForDifferentSizes()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            object size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth + 1, (int)TestConstants.Dimensions.StandardHeight);

            Assert.IsFalse(size1.Equals(size2));
        }

        [Test]
        public void EqualsObjectReturnsFalseForNull()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);

            Assert.IsFalse(size.Equals(null));
        }

        [Test]
        public void EqualsObjectReturnsFalseForDifferentType()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            object other = "not a ContainerSize";

            Assert.IsFalse(size.Equals(other));
        }

        [Test]
        public void EqualsReturnsTrueForZeroSizes()
        {
            var size1 = new ContainerSize(0, 0);
            var size2 = new ContainerSize(0, 0);

            Assert.IsTrue(size1.Equals(size2));
        }

        [Test]
        public void EqualsReturnsTrueForNegativeSizes()
        {
            var size1 = new ContainerSize(-5, -10);
            var size2 = new ContainerSize(-5, -10);

            Assert.IsTrue(size1.Equals(size2));
        }

        #endregion

        #region GetHashCode Tests

        [Test]
        public void GetHashCodeReturnsSameValueForIdenticalSizes()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeReturnsDifferentValuesForDifferentWidths()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth + 1, (int)TestConstants.Dimensions.StandardHeight);

            Assert.AreNotEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeReturnsDifferentValuesForDifferentHeights()
        {
            var size1 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var size2 = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight + 1);

            Assert.AreNotEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeReturnsSameValueForWrapContentSizes()
        {
            var size1 = ContainerSize.WrapContent();
            var size2 = ContainerSize.WrapContent();

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeReturnsSameValueForWrapHorizontalSizes()
        {
            var size1 = ContainerSize.WrapHorizontal((int)TestConstants.Dimensions.StandardHeight);
            var size2 = ContainerSize.WrapHorizontal((int)TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeReturnsSameValueForWrapVerticalSizes()
        {
            var size1 = ContainerSize.WrapVertical((int)TestConstants.Dimensions.StandardWidth);
            var size2 = ContainerSize.WrapVertical((int)TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeIsConsistent()
        {
            var size = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            var hashCode1 = size.GetHashCode();
            var hashCode2 = size.GetHashCode();

            Assert.AreEqual(hashCode1, hashCode2);
        }

        [Test]
        public void GetHashCodeReturnsSameValueForZeroSizes()
        {
            var size1 = new ContainerSize(0, 0);
            var size2 = new ContainerSize(0, 0);

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        [Test]
        public void GetHashCodeReturnsSameValueForNegativeSizes()
        {
            var size1 = new ContainerSize(-5, -10);
            var size2 = new ContainerSize(-5, -10);

            Assert.AreEqual(size1.GetHashCode(), size2.GetHashCode());
        }

        #endregion
    }
}
