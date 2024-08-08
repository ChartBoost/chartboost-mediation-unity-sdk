#import <objc/runtime.h>
#import "ChartboostUnityUtilities.h"
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <ChartboostCoreSDK/ChartboostCoreSDK-Swift.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>

#pragma mark Generic Data Delegates
typedef void (*CBMExternDataEvent)(const char* data);

#pragma mark Fullscreen Ad Delegates
typedef void (*CBMExternFullscreenAdLoadResultEvent)(int hashCode, const void* adHashCode, const char *loadId, const char *metricsJson, const char *bidInfoJson, const char *code, const char *message);
typedef void (*CBMExternFullscreenAdShowResultEvent)(int hashCode, const char* metricsJson, const char *code, const char *message);
typedef void (*CBMExternFullscreenAdEvent)(long hashCode, int eventType, const char *code, const char* message);

#pragma mark Fullscreen Ad Queue Delegates
typedef void (*CBMExternFullscreenAdQueueUpdateEvent)(long hashCode, const char * loadId, const char * metricsJson, const char *bidInfoJson, const char * code, const char * message, int numberOfAdsReady);
typedef void (*CBMExternFullscreenAdQueueRemoveExpiredAdEvent)(long hashCode, int numberOfAdsReady);

#pragma mark Banner Ad Delegates
typedef void (*CBMExternBannerAdLoadResultEvent)(int hashCode, const void* adHashCode, const char *loadId, const char *metricsJson, const char *bidInfoJson, float width, float height, const char *code, const char *message);
typedef void (*CBMExternBannerAdEvent)(long hashCode, int eventType);
typedef void (*CBMExternBannerAdDragEvent)(void * hashCode, float x, float y);

enum fullscreenAdEvents { FullscreenAdRecordImpression = 0, FullscreenAdClick = 1, FullscreenAdReward = 2, FullscreenAdClose = 3, FullscreenAdExpire = 4};
enum fullscreenAdQueueEvents { FullscreenAdQueueUpdate = 0, FullscreenAdQueueRemoveExpiredAd = 1 };
enum bannerAdEvents { BannerAdAppear = 0, BannerAdClick = 1, BannerAdRecordImpression = 2 };
