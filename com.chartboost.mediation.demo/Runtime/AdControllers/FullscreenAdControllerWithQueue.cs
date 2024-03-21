using Chartboost.AdFormats.Fullscreen.Queue;
using Chartboost.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public sealed class FullscreenAdControllerWithQueue : FullscreenAdController
    {
        private readonly Button _loadButton;
        private readonly Button _queueButton;
        private readonly Toggle _queueToggle;
        private readonly ChartboostMediationFullscreenAdQueue _queue;
        
        public FullscreenAdControllerWithQueue(string placementIdentifier, Toggle queueToggle, Button queueButton, Button loadButton) : base(
            placementIdentifier)
        {
            _loadButton = loadButton;
            _queueButton = queueButton;
            _queueToggle = queueToggle;
            
            _queue = ChartboostMediationFullscreenAdQueue.Queue(placementIdentifier);
            _queue.DidUpdate += OnQueueUpdated;
            _queue.DidRemoveExpiredAd += OnQueueRemovedExpiredAd;
            
            _queueToggle.isOn = _queue.IsRunning;
            _queueButton.onClick.AddListener(StartStopQueue);
            _queueToggle.onValueChanged.AddListener(OnEnableQueueingToggle);
            _queueButton.GetComponentInChildren<Text>().text = _queue.IsRunning ? "Stop Queue" : "Start Queue";
        }

        public override void Load()
        {
            if (!_queueToggle.isOn)
            {
                base.Load();
                return;
            }
            
            if (!_queue.HasNextAd())
            {
                Debug.LogError("Queue Is Empty !");
                return;
            }
                
            FullscreenPlacement = _queue.GetNextAd();
            FullscreenPlacement.DidRecordImpression += OnDidRecordImpression;
            FullscreenPlacement.DidClick += OnDidClick;
            FullscreenPlacement.DidReward += OnDidReward;
            FullscreenPlacement.DidExpire += OnDidExpire;
            FullscreenPlacement.DidClose += OnDidClose;
            
            _loadButton.GetComponentInChildren<Text>().text = $"Load from Queue({_queue.NumberOfAdsReady}/{_queue.QueueCapacity})";
        }

        /// <inheritdoc />
        public override void Invalidate()
        {
            base.Invalidate();
            _queueToggle.isOn = false;
            _queue.Stop();
            _queue.DidUpdate -= OnQueueUpdated;
            _queue.DidRemoveExpiredAd -= OnQueueRemovedExpiredAd;

            _queueButton.onClick.RemoveListener(StartStopQueue);
            _queueToggle.onValueChanged.RemoveListener(OnEnableQueueingToggle);
            
            _loadButton.GetComponentInChildren<Text>().text = "Load";
            _queueButton.GetComponentInChildren<Text>().text = "Start Queue";
        }

        private void StartStopQueue()
        {
            if (_queue.IsRunning)
            {
                _queue.Stop();
                _queueButton.GetComponentInChildren<Text>().text = "Start Queue";
            }
            else
            {
                _queue.Start();
                _queueButton.GetComponentInChildren<Text>().text = "Stop Queue";
            }
        }

        private void OnEnableQueueingToggle(bool isOn)
        {
            _loadButton.GetComponentInChildren<Text>().text = isOn
                ? $"Load from Queue({_queue.NumberOfAdsReady}/{_queue.QueueCapacity})"
                : "Load";
        }
        
        private void OnQueueUpdated(ChartboostMediationFullscreenAdQueue queue, ChartboostMediationAdLoadResult adLoadResult, int numberOfAdsReady)
        {
            Debug.Log("Fullscreen Queue Updated !");
            if(_queueToggle.isOn)
                _loadButton.GetComponentInChildren<Text>().text = $"Load from Queue({_queue.NumberOfAdsReady}/{_queue.QueueCapacity})";
        }

        private void OnQueueRemovedExpiredAd(ChartboostMediationFullscreenAdQueue queue, int numberOfAdsRead)
        {
            Debug.Log("Fullscreen Queue Removed Expired Ad !");
            if(_queueToggle.isOn)
                _loadButton.GetComponentInChildren<Text>().text = $"Load from Queue({_queue.NumberOfAdsReady}/{_queue.QueueCapacity})";
        }
    }
}
