#import "CBMDelegates.h"

@interface CBMUnityObserver : NSObject<CBMFullscreenAdDelegate, CBMFullscreenAdQueueDelegate, CBMBannerAdViewDelegate>

+ (instancetype) sharedObserver;

#pragma mark Notifications
@property CBMExternDataEvent didReceivePartnerInitializationData;
@property CBMExternDataEvent didReceiveImpressionLevelRevenueData;

@property id partnerInitializationDataObserver;
@property id impressionLevelRevenueDataObserver;

- (void) subscribeNotificationObservers;
- (void) addImpressionLevelRevenueDataObserver;
- (void) addPartnerInitializationDataObserver;

#pragma mark ChartboostMediationFullscreenAdDelegate
@property CBMExternFullscreenAdEvent didReceiveFullscreenAdEvent;

#pragma mark ChartboostMediationFullscreenAdQueueDelegate
@property CBMExternFullscreenAdQueueUpdateEvent fullscreenAdQueueUpdateEvent;
@property CBMExternFullscreenAdQueueRemoveExpiredAdEvent fullscreenAdQueueRemoveExpiredAdEvent;

#pragma mark ChartboostMediationBannerViewDelegate
@property CBMExternBannerAdEvent didReceiveBannerAdEvent;
@end
