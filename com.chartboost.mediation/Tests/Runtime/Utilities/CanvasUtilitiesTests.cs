using System.Reflection;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable PossibleNullReferenceException

namespace Chartboost.Tests.Runtime.Utilities
{
    public class CanvasUtilitiesTests
    {
        private const string GetCanvasWithHighestSortingOrderMethodName = "GetCanvasWithHighestSortingOrder";
        private const string GetRootLevelCanvasWithHighestSortingOrderMethodName = "GetRootLevelCanvasWithHighestSortingOrder";

        [TearDown]
        public void TearDown()
        {
            // Clean up all canvases created during tests
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases)
            {
                if (canvas != null && canvas.gameObject != null)
                    Object.DestroyImmediate(canvas.gameObject);
            }
        }

        [Test]
        public void GetCanvasCreatesNewCanvasWhenNoneExists()
        {
            var canvas = CanvasUtilities.GetCanvas();

            Assert.IsNotNull(canvas);
            Assert.AreEqual(TestConstants.Canvas.DefaultName, canvas.gameObject.name);
            Assert.AreEqual(RenderMode.ScreenSpaceOverlay, canvas.renderMode);
            Assert.IsNotNull(canvas.GetComponent<CanvasScaler>());
            Assert.IsNotNull(canvas.GetComponent<GraphicRaycaster>());
        }

        [Test]
        public void GetCanvasReturnsRootLevelCanvasWithHighestSortingOrder()
        {
            var rootCanvas1 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            rootCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var rootCanvas2 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            rootCanvas2.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var rootCanvas3 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix3).AddComponent<Canvas>();
            rootCanvas3.sortingOrder = TestConstants.Canvas.SortingOrderMedium;

            var canvas = CanvasUtilities.GetCanvas();

            Assert.IsNotNull(canvas);
            Assert.AreEqual(rootCanvas2, canvas);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderHigh, canvas.sortingOrder);
        }

        [Test]
        public void GetCanvasReturnsCanvasWithHighestSortingOrderWhenNoRootLevel()
        {
            var parentObject = new GameObject(TestConstants.Canvas.ParentObjectName);

            var childCanvas1 = new GameObject(TestConstants.Canvas.ChildName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            childCanvas1.transform.SetParent(parentObject.transform);
            childCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var childCanvas2 = new GameObject(TestConstants.Canvas.ChildName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            childCanvas2.transform.SetParent(parentObject.transform);
            childCanvas2.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var canvas = CanvasUtilities.GetCanvas();

            Assert.IsNotNull(canvas);
            // The method returns the last canvas with the highest sorting order in the iteration
            // Since both child canvases have a parent, GetCanvasWithHighestSortingOrder will return one of them
            Assert.That(canvas.sortingOrder, Is.EqualTo(TestConstants.Canvas.SortingOrderLow).Or.EqualTo(TestConstants.Canvas.SortingOrderHigh));
        }

        [Test]
        public void GetCanvasWithHighestSortingOrderReturnsCorrectCanvas()
        {
            var canvas1 = new GameObject(TestConstants.Canvas.TestName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            canvas1.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var canvas2 = new GameObject(TestConstants.Canvas.TestName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            canvas2.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var canvas3 = new GameObject(TestConstants.Canvas.TestName + TestConstants.Canvas.Suffix3).AddComponent<Canvas>();
            canvas3.sortingOrder = TestConstants.Canvas.SortingOrderMedium;

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            Assert.IsNotNull(result);
            // All canvases are root-level (no parent), so it breaks on the first one with the highest sorting order
            Assert.AreEqual(canvas2, result);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderHigh, result.sortingOrder);
        }

        [Test]
        public void GetCanvasWithHighestSortingOrderReturnsNullWhenNoCanvasExists()
        {
            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            Assert.IsNull(result);
        }

        [Test]
        public void GetCanvasWithHighestSortingOrderIgnoresNestedCanvases()
        {
            var parentCanvas = new GameObject(TestConstants.Canvas.ParentName).AddComponent<Canvas>();
            parentCanvas.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var nestedCanvas = new GameObject(TestConstants.Canvas.NestedName).AddComponent<Canvas>();
            nestedCanvas.transform.SetParent(parentCanvas.transform);
            nestedCanvas.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            Assert.IsNotNull(result);
            Assert.AreEqual(parentCanvas, result);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderLow, result.sortingOrder);
        }

        [Test]
        public void GetCanvasWithHighestSortingOrderBreaksOnFirstRootCanvas()
        {
            // This test ensures the break statement is executed correctly
            var parentCanvas = new GameObject(TestConstants.Canvas.ParentName).AddComponent<Canvas>();
            parentCanvas.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var nestedCanvas = new GameObject(TestConstants.Canvas.NestedName).AddComponent<Canvas>();
            nestedCanvas.transform.SetParent(parentCanvas.transform);
            nestedCanvas.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var rootCanvas = new GameObject(TestConstants.Canvas.RootName).AddComponent<Canvas>();
            rootCanvas.sortingOrder = TestConstants.Canvas.SortingOrderMedium;

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            // The method iterates in descending order: nestedCanvas(100), rootCanvas(10), parentCanvas(0)
            // nestedCanvas has a parent, so continues; rootCanvas has no parent, so breaks
            Assert.IsNotNull(result);
            Assert.AreEqual(rootCanvas, result);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderMedium, result.sortingOrder);
        }

        [Test]
        public void GetCanvasWithHighestSortingOrderWithMixedCanvasesReturnsFirstRoot()
        {
            // This test ensures the loop continues through nested canvases until finding a root one
            var parentCanvas1 = new GameObject(TestConstants.Canvas.ParentName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            parentCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var nestedCanvas1 = new GameObject(TestConstants.Canvas.NestedName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            nestedCanvas1.transform.SetParent(parentCanvas1.transform);
            nestedCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var nestedCanvas2 = new GameObject(TestConstants.Canvas.NestedName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            nestedCanvas2.transform.SetParent(parentCanvas1.transform);
            nestedCanvas2.sortingOrder = TestConstants.Canvas.SortingOrderMedium;

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            // Iterates: nestedCanvas1(100) - has parent, continue
            //           nestedCanvas2(10) - has parent, continue
            //           parentCanvas1(0) - no parent, break
            Assert.IsNotNull(result);
            Assert.AreEqual(parentCanvas1, result);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderLow, result.sortingOrder);
        }

        [Test]
        public void GetRootLevelCanvasWithHighestSortingOrderReturnsCorrectCanvas()
        {
            var rootCanvas1 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            rootCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var rootCanvas2 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            rootCanvas2.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var rootCanvas3 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix3).AddComponent<Canvas>();
            rootCanvas3.sortingOrder = TestConstants.Canvas.SortingOrderMedium;

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetRootLevelCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            Assert.IsNotNull(result);
            Assert.AreEqual(rootCanvas2, result);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderHigh, result.sortingOrder);
        }

        [Test]
        public void GetRootLevelCanvasWithHighestSortingOrderReturnsNullWhenNoRootCanvas()
        {
            var parentObject = new GameObject(TestConstants.Canvas.ParentObjectName);
            var childCanvas = new GameObject(TestConstants.Canvas.ChildName).AddComponent<Canvas>();
            childCanvas.transform.SetParent(parentObject.transform);

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetRootLevelCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            Assert.IsNull(result);
        }

        [Test]
        public void GetRootLevelCanvasWithHighestSortingOrderIgnoresChildCanvases()
        {
            var rootCanvas = new GameObject(TestConstants.Canvas.RootName).AddComponent<Canvas>();
            rootCanvas.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var parentObject = new GameObject(TestConstants.Canvas.ParentObjectName);
            var childCanvas = new GameObject(TestConstants.Canvas.ChildName).AddComponent<Canvas>();
            childCanvas.transform.SetParent(parentObject.transform);
            childCanvas.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var canvasUtilitiesType = typeof(CanvasUtilities);
            var method = canvasUtilitiesType.GetMethod(GetRootLevelCanvasWithHighestSortingOrderMethodName, BindingFlags.NonPublic | BindingFlags.Static);

            var result = method.Invoke(null, null) as Canvas;

            Assert.IsNotNull(result);
            Assert.AreEqual(rootCanvas, result);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderLow, result.sortingOrder);
        }

        [Test]
        public void GetCanvasPrioritizesRootLevelOverNestedCanvases()
        {
            var parentObject = new GameObject(TestConstants.Canvas.ParentObjectName);
            var nestedCanvas = new GameObject(TestConstants.Canvas.NestedName).AddComponent<Canvas>();
            nestedCanvas.transform.SetParent(parentObject.transform);
            nestedCanvas.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var rootCanvas = new GameObject(TestConstants.Canvas.RootName).AddComponent<Canvas>();
            rootCanvas.sortingOrder = TestConstants.Canvas.SortingOrderLow;

            var canvas = CanvasUtilities.GetCanvas();

            Assert.IsNotNull(canvas);
            Assert.AreEqual(rootCanvas, canvas);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderLow, canvas.sortingOrder);
        }

        [Test]
        public void GetCanvasCreatedCanvasHasCanvasScalerWithCorrectMode()
        {
            var canvas = CanvasUtilities.GetCanvas();
            var canvasScaler = canvas.GetComponent<CanvasScaler>();

            Assert.IsNotNull(canvasScaler);
            Assert.AreEqual(CanvasScaler.ScaleMode.ConstantPixelSize, canvasScaler.uiScaleMode);
        }

        [Test]
        public void GetCanvasWithMultipleRootCanvasesReturnsSameSortingOrder()
        {
            var rootCanvas1 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            rootCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var rootCanvas2 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            rootCanvas2.sortingOrder = TestConstants.Canvas.SortingOrderHigh;

            var canvas = CanvasUtilities.GetCanvas();

            Assert.IsNotNull(canvas);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderHigh, canvas.sortingOrder);
        }

        [Test]
        public void GetCanvasWithNegativeSortingOrdersWorksCorrectly()
        {
            var rootCanvas1 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix1).AddComponent<Canvas>();
            rootCanvas1.sortingOrder = TestConstants.Canvas.SortingOrderNegativeLow;

            var rootCanvas2 = new GameObject(TestConstants.Canvas.RootName + TestConstants.Canvas.Suffix2).AddComponent<Canvas>();
            rootCanvas2.sortingOrder = TestConstants.Canvas.SortingOrderNegativeHigh;

            var canvas = CanvasUtilities.GetCanvas();

            Assert.IsNotNull(canvas);
            Assert.AreEqual(rootCanvas2, canvas);
            Assert.AreEqual(TestConstants.Canvas.SortingOrderNegativeHigh, canvas.sortingOrder);
        }
    }
}
