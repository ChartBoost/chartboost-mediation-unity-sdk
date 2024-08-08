using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Default.Ad.Banner
{
    /// <summary>
    /// Default implementation of <see cref="IBannerAd"/> for any unsupported platforms.
    /// </summary>
    public class BannerAdDefault : BannerAdBase
    {
        internal BannerAdDefault() : base(new IntPtr()) { }

        /// <inheritdoc cref="IBannerAd.Keywords"/>
        public override IReadOnlyDictionary<string, string> Keywords { get; set; }

        /// <inheritdoc cref="IBannerAd.PartnerSettings"/>
        public override IReadOnlyDictionary<string, string> PartnerSettings { get; set; }
        
        /// <inheritdoc />
        public override Vector2 Position { get; set; }
        
        /// <inheritdoc />
        public override Vector2 Pivot { get; set; }
        
        /// <inheritdoc />
        public override bool Visible { get; set; }
        
        /// <inheritdoc />
        public override bool Draggable { get; set; }

        /// <inheritdoc cref="IBannerAd.Request"/>
        public override BannerAdLoadRequest Request => _request;
        
        /// <inheritdoc cref="IBannerAd.WinningBidInfo"/>
        public override BidInfo WinningBidInfo =>  new();
        
        /// <inheritdoc cref="IBannerAd.LoadId"/>
        public override string LoadId => null;
        
        /// <inheritdoc cref="IBannerAd.LoadMetrics"/>
        public override Metrics? LoadMetrics => null;
        
        /// <inheritdoc cref="IBannerAd.AdSize"/>
        public override BannerSize? BannerSize => null;
        
        /// <inheritdoc cref="IBannerAd.ContainerSize"/>
        public override ContainerSize ContainerSize { get; set; }
        
        /// <inheritdoc cref="IBannerAd.HorizontalAlignment"/>
        public override BannerHorizontalAlignment HorizontalAlignment { get; set; }
        
        /// <inheritdoc cref="IBannerAd.VerticalAlignment"/>
        public override BannerVerticalAlignment VerticalAlignment { get; set; }

        /// <inheritdoc cref="IBannerAd.Load(BannerAdLoadRequest)"/>
        public override Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
        {
            base.Load(request);
            return Task.FromResult(new BannerAdLoadResult(new ChartboostMediationError(Errors.UnsupportedPlatform)));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing) {}
    }
}
