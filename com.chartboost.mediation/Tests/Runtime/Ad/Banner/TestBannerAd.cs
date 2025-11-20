using System;
using System.Collections.Generic;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    /// <summary>
    /// Shared test helper class for BannerAdBase testing.
    /// Provides a concrete implementation of BannerAdBase with controllable properties and test hooks.
    /// </summary>
    internal class TestBannerAd : BannerAdBase
    {
        /// <summary>
        /// Tracks whether Dispose(bool) was called.
        /// </summary>
        public bool DisposeCalled { get; private set; }

        /// <summary>
        /// Tracks the disposing parameter value passed to Dispose(bool).
        /// </summary>
        public bool DisposingValue { get; private set; }

        /// <summary>
        /// Exposes the protected IsDisposed property for testing.
        /// </summary>
        public bool IsDisposedPublic => IsDisposed;

        /// <summary>
        /// Tracks whether WillAppear event was triggered.
        /// </summary>
        public bool WillAppearCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidClick event was triggered.
        /// </summary>
        public bool ClickCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidRecordImpression event was triggered.
        /// </summary>
        public bool RecordImpressionCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidBeginDrag event was triggered.
        /// </summary>
        public bool DragBeginCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidDrag event was triggered.
        /// </summary>
        public bool DragCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidEndDrag event was triggered.
        /// </summary>
        public bool DragEndCalled { get; private set; }

        /// <summary>
        /// Stores the X position from DidBeginDrag event.
        /// </summary>
        public float DragBeginX { get; private set; }

        /// <summary>
        /// Stores the Y position from DidBeginDrag event.
        /// </summary>
        public float DragBeginY { get; private set; }

        /// <summary>
        /// Stores the X position from DidDrag event.
        /// </summary>
        public float DragX { get; private set; }

        /// <summary>
        /// Stores the Y position from DidDrag event.
        /// </summary>
        public float DragY { get; private set; }

        /// <summary>
        /// Stores the X position from DidEndDrag event.
        /// </summary>
        public float DragEndX { get; private set; }

        /// <summary>
        /// Stores the Y position from DidEndDrag event.
        /// </summary>
        public float DragEndY { get; private set; }

        public TestBannerAd(IntPtr uniqueId) : base(uniqueId)
        {
            WillAppear += _ => WillAppearCalled = true;
            DidClick += _ => ClickCalled = true;
            DidRecordImpression += _ => RecordImpressionCalled = true;
            DidBeginDrag += (_, x, y) =>
            {
                DragBeginCalled = true;
                DragBeginX = x;
                DragBeginY = y;
            };
            DidDrag += (_, x, y) =>
            {
                DragCalled = true;
                DragX = x;
                DragY = y;
            };
            DidEndDrag += (_, x, y) =>
            {
                DragEndCalled = true;
                DragEndX = x;
                DragEndY = y;
            };
        }

        public override IReadOnlyDictionary<string, string> Keywords { get; set; }

        public override IReadOnlyDictionary<string, string> PartnerSettings { get; set; }

        public override BannerSize? BannerSize { get; } = null;

        public override BannerAdLoadRequest Request => _request;

        public override BidInfo? WinningBidInfo => null;

        public override string LoadId => null;

        public override Metrics? LoadMetrics => null;

        public override Vector2 Position { get; set; }

        public override Vector2 Pivot { get; set; }

        public override bool Visible { get; set; }

        public override bool Draggable { get; set; }

        public override ContainerSize ContainerSize { get; set; }

        public override BannerHorizontalAlignment HorizontalAlignment { get; set; }

        public override BannerVerticalAlignment VerticalAlignment { get; set; }

        protected override void Dispose(bool disposing)
        {
            DisposeCalled = true;
            DisposingValue = disposing;
            IsDisposed = true;
        }

        // Expose internal methods for testing
        public void TestOnWillAppear() => OnWillAppear();
        public void TestOnClick() => OnClick();
        public void TestOnRecordImpression() => OnRecordImpression();
        public void TestOnDragBegin(float x, float y) => OnDragBegin(x, y);
        public void TestOnDrag(float x, float y) => OnDrag(x, y);
        public void TestOnDragEnd(float x, float y) => OnDragEnd(x, y);
        public void TestSetContainerBackgroundColor(Color color) => SetContainerBackgroundColor(color);
        public void TestSetAdBackgroundColor(Color color) => SetAdBackgroundColor(color);
        public void TestSetAdRelativePosition(Vector2 position) => AdRelativePosition = position;
        public Vector2 TestGetAdRelativePosition() => AdRelativePosition;

    }

    /// <summary>
    /// Extended TestBannerAd that allows setting BannerSize for testing purposes.
    /// Used specifically for testing methods that require a non-null BannerSize.
    /// </summary>
    internal class TestBannerAdWithSize : TestBannerAd
    {
        public TestBannerAdWithSize(IntPtr uniqueId, BannerSize bannerSize) : base(uniqueId)
        {
            BannerSize = bannerSize;
        }

        public override BannerSize? BannerSize { get; }
    }
}
