using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace Chartboost.AdFormats.Banner
{
    internal class ChartboostMediationBannerViewEditor : ChartboostMediationBannerViewBase
    {
        private bool _autoRefresh = false;
        private GameObject _bannerView;

        public ChartboostMediationBannerViewEditor()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Keywords = new Dictionary<string, string>();
            // ReSharper disable once VirtualMemberCallInConstructor
            HorizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Right;
            // ReSharper disable once VirtualMemberCallInConstructor
            VerticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;

        }

        public override Dictionary<string, string> Keywords { get; set; }
        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }
        public override BidInfo WinningBidInfo { get; protected set; }
        public override string LoadId { get; protected set; }
        public override Metrics? LoadMetrics { get; protected set; }
        public override ChartboostMediationBannerAdSize AdSize { get; protected set; }
        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            Request = request;
            if (_bannerView == null)
            {
                var canvas = GameObject.FindObjectOfType<Canvas>();
                
                _bannerView = new GameObject("BannerView");
                _bannerView.transform.parent = canvas.transform;
                _bannerView.AddComponent<RectTransform>();
                
                var canvasScale = canvas.transform.localScale;
                _bannerView.transform.localScale = new Vector3(1/canvasScale.x, 1/canvasScale.y, 1/canvasScale.z);
            }

            var anchor = Vector2.zero;
            var pivot = Vector2.zero;
            switch (screenLocation)
            {
                case ChartboostMediationBannerAdScreenLocation.TopLeft:
                    anchor = new Vector2(0, 1);
                    pivot = new Vector2(0, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopCenter:
                    anchor = new Vector2(0.5f, 1);
                    pivot = new Vector2(0.5f, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.TopRight:
                    anchor = new Vector2(1, 1);
                    pivot = new Vector2(1, 1);
                    break;
                case ChartboostMediationBannerAdScreenLocation.Center:
                    anchor = new Vector2(0.5f, .5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomLeft:
                    anchor = new Vector2(0, 0);
                    pivot = new Vector2(0, 0);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomCenter:
                    anchor = new Vector2(0.5f, 0);
                    pivot = new Vector2(0.5f, 0);
                    break;
                case ChartboostMediationBannerAdScreenLocation.BottomRight:
                    anchor = new Vector2(1, 0);
                    pivot = new Vector2(1, 0);
                    break;
            }
            
            var rect = _bannerView.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(request.Size.Width, request.Size.Height);

            if (_bannerView.GetComponentInChildren<Image>() == null)
            {
                var ad = new GameObject("ad");
                ad.AddComponent<Image>();
                ad.transform.parent = _bannerView.transform;
                ad.transform.localScale = Vector3.one;
            }
            
            var metrics = new Metrics
            {
                auctionId = "auction"
            };

            if (!_autoRefresh)
            {
                _autoRefresh = true;
                AutoRefresh();
            }
            
            UpdateAd();
            
            
            OnBannerWillAppear(this);
            
            var adLoadResult = new ChartboostMediationBannerAdLoadResult("loadId", metrics, null);
            return await Task.FromResult(adLoadResult);
        }

        public override void Reset()
        {
            _autoRefresh = false;
            Object.Destroy(_bannerView.GetComponent<Image>());
        }
        
        private async void AutoRefresh()
        {   
            if(_bannerView == null || Input.GetKeyDown(KeyCode.Space))
                return;

            await Task.Delay(5000);
            
            var auctions = new[] { "auction1", "auction2", "auction3", "auction4" };
            var partners = new[] { "partner1", "partner2", "partner3", "partner4" };
            var lineItems = new[] { "lineItem1", "lineItem2", "lineItem3", "lineItem4" };
            var lineItemIds = new[] { "lineItemId1", "lineItemId2", "lineItemId3", "lineItemId4" };
            var prices = new double[] { 1, 2, 3, 4, };
            
            int rand = UnityEngine.Random.Range(0, 4);

            WinningBidInfo = new BidInfo(auctions[rand], partners[rand], prices[rand], lineItems[rand],
                lineItemIds[rand]);
            LoadMetrics = new Metrics
            {
                auctionId = auctions[rand]
            };
            // Size = (rand % 2 == 0) ? ChartboostMediationBannerSize.STANDARD : ChartboostMediationBannerSize.LEADERBOARD;
            AdSize = ChartboostMediationBannerAdSize.Standard;

            if(_bannerView == null)
                return;
            
            UpdateAd();
            
            Debug.Log($" VERTICAL ALIGNMENT : {VerticalAlignment}");
            
            OnBannerWillAppear(this);
            
            AutoRefresh();
        }

        private void UpdateAd()
        {
            var ad = _bannerView.GetComponentInChildren<Image>();
            if(ad == null)
                return;
            
            var adRect = ad.GetComponent<RectTransform>();
            adRect.sizeDelta = new Vector2(AdSize.Width, AdSize.Height);
            
            var pivot = new Vector2(0.5f, 0.5f);
            var anchor = pivot;
            pivot.x = HorizontalAlignment switch
            {
                ChartboostMediationBannerHorizontalAlignment.Left => 0,
                ChartboostMediationBannerHorizontalAlignment.Center => 0.5f,
                ChartboostMediationBannerHorizontalAlignment.Right => 1,
                _ => pivot.x
            };
            
            pivot.y = VerticalAlignment switch
            {
                ChartboostMediationBannerVerticalAlignment.Top => 0,
                ChartboostMediationBannerVerticalAlignment.Center => 0.5f,
                ChartboostMediationBannerVerticalAlignment.Bottom => 1,
                _ => pivot.y
            };

            adRect.anchoredPosition = pivot;
            adRect.pivot = pivot;
            
        }
    }
}