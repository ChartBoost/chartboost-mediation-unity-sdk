/*
 * HeliumSdkManager.h
 * Helium SDK
 */

#import <Foundation/Foundation.h>
#import <HeliumSdk/HeliumSdk.h>

#if !defined(HELIUM_UNITY_SDK_VERSION_STRING)
  #define HELIUM_UNITY_SDK_VERSION_STRING @"0.0.0"
#endif

typedef void (*HeliumEvent)(int errorCode, const char* errorDescription);
typedef void (*HeliumPlacementEvent)(const char* placementName, int errorCode, const char* errorDescription);
typedef void (*HeliumBidWinEvent) (const char* placementName, const char* partnerPlacementName, const char* auctionId, double price, const char* seat);
typedef void (*HeliumRewardEvent)(const char* placementName, int reward);
typedef void (*HeliumILRDEvent)(const char* impressionData);


@interface HeliumSdkManager : NSObject

@property (nonatomic, retain) NSString *gameObjectName;

+ (HeliumSdkManager*)sharedManager;

- (void)setLifeCycleCallbacks:(HeliumEvent)didStartCallback  didReceiveILRDCallback:(HeliumILRDEvent)didReceiveILRDCallback;
- (void)setInterstitialCallbacks:(HeliumPlacementEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didCloseCallback:(HeliumPlacementEvent)didCloseCallback  didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback;
- (void)setRewardedCallbacks:(HeliumPlacementEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didCloseCallback:(HeliumPlacementEvent)didCloseCallback  didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback didReceiveRewardCallback:(HeliumRewardEvent)didReceiveRewardCallback;
- (void)setBannerCallbacks:(HeliumPlacementEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback;
- (void)startHeliumWithAppId:(NSString*)appId andAppSignature:(NSString*)appSignature unityVersion:(NSString *)unityVersion;
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

@end
