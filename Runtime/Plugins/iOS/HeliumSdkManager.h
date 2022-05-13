/*
 * HeliumSdkManager.h
 * Helium SDK
 */

#import <Foundation/Foundation.h>
#import <HeliumSdk/HeliumSdk.h>

#if !defined(HELIUM_UNITY_SDK_VERSION_STRING)
  #define HELIUM_UNITY_SDK_VERSION_STRING @"0.0.0"
#endif

typedef void (*HeliumBackgroundEventCallback)(const char* eventName, const char* eventArgsJson);

@interface HeliumSdkManager : NSObject

@property (class, nonatomic) HeliumBackgroundEventCallback bgEventCallback;
@property (nonatomic, retain) NSString *gameObjectName;

+ (HeliumSdkManager*)sharedManager;

- (void)startHeliumWithAppId:(NSString*)appId andAppSignature:(NSString*)appSignature unityVersion:(NSString *)unityVersion bgEventCallback:(HeliumBackgroundEventCallback)bgEventCallback;
- (void)setSubjectToCoppa:(BOOL)isSubject;
- (void)setSubjectToGDPR:(BOOL)isSubject;
- (void)setUserHasGivenConsent:(BOOL)hasGivenConsent;
- (void)setCCPAConsent:(BOOL)hasGivenConsent;
- (void)setUserIdentifier:(NSString*)userIdentifier;
- (NSString*)getUserIdentifier;
- (id<HeliumInterstitialAd>)getInterstitialAd:(NSString*)placementId;
- (id<HeliumRewardedAd>)getRewardedAd:(NSString*)placementId;
- (HeliumBannerView*)getBannerAd:(NSString*)placementName andSize:(CHBHBannerSize)size;
- (void)freeInterstitialAd:(NSNumber*)adId;
- (void)freeRewardedAd:(NSNumber*)adId;
- (void)freeBannerAd:(NSNumber*)adId;

+ (void)sendUnityEvent:(NSString*)eventName withParam:(const char*)param backgroundOK:(BOOL)bg;

@end
