/*
* ChartboostMediationManager.h
* Chartboost Mediation SDK iOS/Unity
*/

#import <Foundation/Foundation.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK.h>

typedef void (*ChartboostMediationEvent)(const char* error);
typedef void (*ChartboostMediationILRDEvent)(const char* impressionData);
typedef void (*ChartboostMediationPartnerInitializationDataEvent)(const char* partnerInitializationData);
typedef void (*ChartboostMediationPlacementEvent)(const char* placementName, const char* error);
typedef void (*ChartboostMediationPlacementLoadEvent)(const char* placementName, const char* loadId, const char* auctionId, const char* partnerId, double price, const char* error);

@interface ChartboostMediationManager : NSObject

+ (ChartboostMediationManager*)sharedManager;

- (void)setLifeCycleCallbacks:(ChartboostMediationEvent)didStartCallback  didReceiveILRDCallback:(ChartboostMediationILRDEvent)didReceiveILRDCallback didReceivePartnerInitializationData:(ChartboostMediationPartnerInitializationDataEvent)didReceivePartnerInitializationDataCallback;
- (void)setInterstitialCallbacks:(ChartboostMediationPlacementLoadEvent)didLoadCallback didShowCallback:(ChartboostMediationPlacementEvent)didShowCallback  didCloseCallback:(ChartboostMediationPlacementEvent)didCloseCallback didClickCallback:(ChartboostMediationPlacementEvent)didClickCallback didRecordImpression:(ChartboostMediationPlacementEvent)didRecordImpression;
- (void)setRewardedCallbacks:(ChartboostMediationPlacementLoadEvent)didLoadCallback didShowCallback:(ChartboostMediationPlacementEvent)didShowCallback didCloseCallback:(ChartboostMediationPlacementEvent)didCloseCallback didClickCallback:(ChartboostMediationPlacementEvent)didClickCallback didRecordImpression:(ChartboostMediationPlacementEvent)didRecordImpression didReceiveRewardCallback:(ChartboostMediationPlacementEvent)didReceiveRewardCallback;
- (void)setBannerCallbacks:(ChartboostMediationPlacementLoadEvent)didLoadCallback didRecordImpression:(ChartboostMediationPlacementEvent)didRecordImpression didClickCallback:(ChartboostMediationPlacementEvent)didClickCallback;
- (void)startHeliumWithAppId:(NSString*)appId andAppSignature:(NSString*)appSignature unityVersion:(NSString *)unityVersion initializationOptions:(const char**)initializationOptions initializationOptionsSize:(int)initializationOptionsSize;
- (void)setSubjectToCoppa:(BOOL)isSubject;
- (void)setSubjectToGDPR:(BOOL)isSubject;
- (void)setUserHasGivenConsent:(BOOL)hasGivenConsent;
- (void)setCCPAConsent:(BOOL)hasGivenConsent;
- (void)setUserIdentifier:(NSString*)userIdentifier;
- (void)setTestMode:(BOOL)isTestModeEnabled;
- (NSString*)getUserIdentifier;
- (id<HeliumInterstitialAd>)getInterstitialAd:(NSString*)placementId;
- (id<HeliumRewardedAd>)getRewardedAd:(NSString*)placementId;
- (HeliumBannerView*)getBannerAd:(NSString*)placementName andSize:(CHBHBannerSize)size;
- (void)freeAd:(NSNumber*)adId placementName:(NSString*)placementName multiPlacementSupport:(BOOL) multiPlacementSupport;

@end
