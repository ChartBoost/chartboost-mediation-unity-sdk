#import "CBMUnityObserver.h"
#import "CBMAdStore.h"
#import "CBMBannerAdWrapper.h"

@implementation CBMUnityObserver

+ (instancetype)sharedObserver {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
    });

    return _sharedObject;
}

#pragma mark Notifications
- (void)subscribePartnerAdapterInitializationResults {
    [[NSNotificationCenter defaultCenter] addObserverForName:NSNotification.chartboostMediationDidReceivePartnerAdapterInitResults object:nil queue:nil usingBlock:^(NSNotification* _Nonnull notification) {
            NSDictionary *results = (NSDictionary *)notification.object;
            const char* jsonToUnity = toJSON(results);

            if (self->_didReceivePartnerInitializationData != nil)
                self->_didReceivePartnerInitializationData(jsonToUnity);
    }];
}

#pragma mark ChartboostMediationFullscreenAdDelegate
- (void)serializeFullscreenAdEvent: (CBMFullscreenAd *)ad  fullscreenEvent:(CBMFullscreenAdEventType)fullscreenAdEvent error:(CBMError *)error
{
    if (_didReceiveFullscreenAdEvent == nil)
        return;

    const char *code = "";
    const char *message = "";

    if (error != nil)
    {
        CBMErrorCode codeInt = [error chartboostMediationCode];
        code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
        message = toCStringOrNull([error localizedDescription]);
    }

    _didReceiveFullscreenAdEvent((long)ad, fullscreenAdEvent, code, message);
}

- (void)didRecordImpressionWithAd:(CBMFullscreenAd *)ad{
    [self serializeFullscreenAdEvent:ad fullscreenEvent:CBMFullscreenAdEventRecordImpression error:nil];
}

- (void)didClickWithAd:(CBMFullscreenAd *)ad{
    [self serializeFullscreenAdEvent:ad fullscreenEvent:CBMFullscreenAdEventClick error:nil];
}

- (void)didRewardWithAd:(CBMFullscreenAd *)ad {
    [self serializeFullscreenAdEvent:ad fullscreenEvent:CBMFullscreenAdEventReward error:nil];
}

- (void)didCloseWithAd:(CBMFullscreenAd *)ad error:(CBMError * _Nullable)error {
    UnityPause(false);
    [self serializeFullscreenAdEvent:ad fullscreenEvent:CBMFullscreenAdEventClose error:error];
}

- (void)didExpireWithAd:(CBMFullscreenAd *)ad {
    [self serializeFullscreenAdEvent:ad fullscreenEvent:CBMFullscreenAdEventExpire error:nil];
}

#pragma mark ChartboostMediationFullscreenAdQueueDelegate
- (void)fullscreenAdQueue:(CBMFullscreenAdQueue *)adQueue didFinishLoadingWithResult:(CBMAdLoadResult *)adLoadResult numberOfAdsReady:(NSInteger)numberOfAdsReady {
    CBMError *error = [adLoadResult error];

    if (_fullscreenAdQueueUpdateEvent == nil)
        return;

    if (error != nil)
    {
        CBMErrorCode codeInt = [error chartboostMediationCode];
        const char *code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
        const char *message = toCStringOrNull([error localizedDescription]);
        _fullscreenAdQueueUpdateEvent((long)adQueue, NULL, NULL, NULL, code, message, (int)numberOfAdsReady);
        return;
    }

    const char * loadId = toCStringOrNull([adLoadResult loadID]);
    const char * metricsJson = toJSON([adLoadResult metrics]);
    const char * winningBidInfoJson = toJSON([adLoadResult winningBidInfo]);

    _fullscreenAdQueueUpdateEvent((long)adQueue, loadId, metricsJson, winningBidInfoJson, NULL, NULL, (int)numberOfAdsReady);
}

- (void)fullscreenAdQueueDidRemoveExpiredAd:(CBMFullscreenAdQueue *)adQueue numberOfAdsReady:(NSInteger)numberOfAdsReady {
    if (_fullscreenAdQueueRemoveExpiredAdEvent != nil)
        _fullscreenAdQueueRemoveExpiredAdEvent((long)adQueue, (int)numberOfAdsReady);
}

#pragma mark ChartboostMediationBannerViewDelegate
- (void)serializeBannerEvent: (CBMBannerAdView*) ad bannerEvent:(CBMBannerAdEventType)bannerAdEvent {
    if (_didReceiveBannerAdEvent == nil)
        return;

    CBMBannerAdWrapper *wrapper = [[CBMAdStore sharedStore] getBannerAd:(__bridge void*)ad];
    _didReceiveBannerAdEvent((long)wrapper, bannerAdEvent);
    [wrapper updateFrame];
}

- (void)willAppearWithBannerView:(CBMBannerAdView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:CBMBannerAdEventAppear];
}

- (void)didClickWithBannerView:(CBMBannerAdView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:CBMBannerAdEventClick];
}

- (void)didRecordImpressionWithBannerView:(CBMBannerAdView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:CBMBannerAdEventRecordImpression];
}
@end

extern "C" {
    void _CBMSetPartnerAdapterInitializationResultsCallback(CBMExternDataEvent didReceivePartnerInitializationData){
        [[CBMUnityObserver sharedObserver] setDidReceivePartnerInitializationData:didReceivePartnerInitializationData];
        [[CBMUnityObserver sharedObserver] subscribePartnerAdapterInitializationResults];
    }
}
