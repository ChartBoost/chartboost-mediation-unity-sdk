/*
 * HeliumSdkManager.h
 * Helium SDK
 */

#import <Foundation/Foundation.h>
#import <HeliumSdk/Helium.h>

typedef void (*HeliumEvent)(const char* error);
typedef void (*HeliumILRDEvent)(const char* impressionData);
typedef void (*HeliumPartnerInitializationDataEvent)(const char* partnerInitializationData);
typedef void (*HeliumPlacementEvent)(const char* placementName, const char* error);
typedef void (*HeliumPlacementLoadEvent)(const char* placementName, const char* loadId, const char* error);
typedef void (*HeliumBidWinEvent) (const char* placementName, const char* auctionId, const char* partnerId, double price);

@interface HeliumSdkManager : NSObject

+ (HeliumSdkManager*)sharedManager;

- (void)setLifeCycleCallbacks:(HeliumEvent)didStartCallback  didReceiveILRDCallback:(HeliumILRDEvent)didReceiveILRDCallback didReceivePartnerInitializationData:(HeliumPartnerInitializationDataEvent)didReceivePartnerInitializationDataCallback;
- (void)setInterstitialCallbacks:(HeliumPlacementLoadEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback  didCloseCallback:(HeliumPlacementEvent)didCloseCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didRecordImpression:(HeliumPlacementEvent)didRecordImpression didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback;
- (void)setRewardedCallbacks:(HeliumPlacementLoadEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback didCloseCallback:(HeliumPlacementEvent)didCloseCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didRecordImpression:(HeliumPlacementEvent)didRecordImpression didReceiveRewardCallback:(HeliumPlacementEvent)didReceiveRewardCallback didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback;
- (void)setBannerCallbacks:(HeliumPlacementLoadEvent)didLoadCallback didRecordImpression:(HeliumPlacementEvent)didRecordImpression didClickCallback:(HeliumPlacementEvent)didClickCallback didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback;
- (void)startHeliumWithAppId:(NSString*)appId andAppSignature:(NSString*)appSignature unityVersion:(NSString *)unityVersion initializationOptions:(const char**)initializationOptions initializationOptionsSize:(int)initializationOptionsSize;
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
