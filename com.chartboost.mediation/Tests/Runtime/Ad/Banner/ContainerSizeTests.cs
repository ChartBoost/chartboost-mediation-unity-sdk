using System.Collections.Generic;
using Chartboost.Mediation.Ad.Banner;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class ContainerSizeTests
    {
        private const int Width = (int)TestConstants.Dimensions.StandardWidth;
        private const int Height = (int)TestConstants.Dimensions.StandardHeight;
        private const int Wrap = TestConstants.Dimensions.WrapContent;

        private static IEnumerable<TestCaseData> FactoryCases()
        {
            yield return new TestCaseData(new ContainerSize(Width, Height), Width, Height).SetName("ConstructorInitializesCorrectly");
            yield return new TestCaseData(new ContainerSize(0, Height), 0, Height).SetName("ConstructorWithZeroWidthInitializesCorrectly");
            yield return new TestCaseData(new ContainerSize(Width, 0), Width, 0).SetName("ConstructorWithZeroHeightInitializesCorrectly");
            yield return new TestCaseData(new ContainerSize(-5, -10), -5, -10).SetName("ConstructorWithNegativeValuesInitializesCorrectly");
            yield return new TestCaseData(ContainerSize.WrapHorizontal(Height), Wrap, Height).SetName("WrapHorizontalCreatesCorrectSize");
            yield return new TestCaseData(ContainerSize.WrapHorizontal(0), Wrap, 0).SetName("WrapHorizontalWithZeroHeightInitializesCorrectly");
            yield return new TestCaseData(ContainerSize.WrapVertical(Width), Width, Wrap).SetName("WrapVerticalCreatesCorrectSize");
            yield return new TestCaseData(ContainerSize.WrapVertical(0), 0, Wrap).SetName("WrapVerticalWithZeroWidthInitializesCorrectly");
            yield return new TestCaseData(ContainerSize.WrapContent(), Wrap, Wrap).SetName("WrapContentCreatesCorrectSize");
            yield return new TestCaseData(ContainerSize.FixedSize(Width, Height), Width, Height).SetName("FixedSizeCreatesCorrectSize");
            yield return new TestCaseData(ContainerSize.FixedSize(1920, 1080), 1920, 1080).SetName("FixedSizeWithLargeValuesInitializesCorrectly");
        }

        // Pairs built separately from equal arguments
        private static IEnumerable<TestCaseData> EqualPairs()
        {
            yield return new TestCaseData(new ContainerSize(Width, Height), new ContainerSize(Width, Height)).SetName("EqualForIdenticalSizes");
            yield return new TestCaseData(ContainerSize.WrapContent(), ContainerSize.WrapContent()).SetName("EqualForWrapContentSizes");
            yield return new TestCaseData(ContainerSize.WrapHorizontal(Height), ContainerSize.WrapHorizontal(Height)).SetName("EqualForWrapHorizontalSizes");
            yield return new TestCaseData(ContainerSize.WrapVertical(Width), ContainerSize.WrapVertical(Width)).SetName("EqualForWrapVerticalSizes");
            yield return new TestCaseData(new ContainerSize(0, 0), new ContainerSize(0, 0)).SetName("EqualForZeroSizes");
            yield return new TestCaseData(new ContainerSize(-5, -10), new ContainerSize(-5, -10)).SetName("EqualForNegativeSizes");
        }

        private static IEnumerable<TestCaseData> UnequalPairs()
        {
            var size = new ContainerSize(Width, Height);
            yield return new TestCaseData(size, new ContainerSize(Width + 1, Height)).SetName("NotEqualForDifferentWidths");
            yield return new TestCaseData(size, new ContainerSize(Width, Height + 1)).SetName("NotEqualForDifferentHeights");
            yield return new TestCaseData(size, new ContainerSize(Width + 1, Height + 1)).SetName("NotEqualForDifferentWidthAndHeight");
        }

        [TestCaseSource(nameof(FactoryCases))]
        public void FactoryProducesExpectedDimensions(ContainerSize size, int expectedWidth, int expectedHeight)
        {
            Assert.AreEqual(expectedWidth, size.Width);
            Assert.AreEqual(expectedHeight, size.Height);
        }

        [TestCaseSource(nameof(EqualPairs))]
        public void EqualSizesAreEqualAndShareHashCode(ContainerSize first, ContainerSize second)
        {
            Assert.IsTrue(first.Equals(second));
            Assert.IsTrue(first.Equals((object)second));
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [TestCaseSource(nameof(UnequalPairs))]
        public void DifferentSizesAreNotEqualAndHashDifferently(ContainerSize first, ContainerSize second)
        {
            Assert.IsFalse(first.Equals(second));
            Assert.IsFalse(first.Equals((object)second));
            Assert.AreNotEqual(first.GetHashCode(), second.GetHashCode());
        }

        [TestCase(null)]
        [TestCase("not a ContainerSize")]
        public void EqualsObjectReturnsFalseForNonContainerSize(object other)
        {
            Assert.IsFalse(new ContainerSize(Width, Height).Equals(other));
        }
    }
}
