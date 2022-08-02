/*
 * HeliumSdkManager.mm
 * Helium SDK
 */

#import "HeliumSdkManager.h"
#import "HeliumSdk/HeliumSdk-Swift.h"

// interstitial ad objects
NSMutableDictionary * storedAds = nil;

// lifecycle callbacks
static HeliumEvent _didStartCallback;
static HeliumILRDEvent _didReceiveILRDCallback;

// interstitial callbacks
static HeliumPlacementEvent _interstitialDidLoadCallback;
static HeliumPlacementEvent _interstitialDidClickCallback;
static HeliumPlacementEvent _interstitialDidCloseCallback;
static HeliumPlacementEvent _interstitialDidShowCallback;
static HeliumPlacementEvent _interstitialDidRecordImpressionCallback;
static HeliumBidWinEvent _interstitialDidWinBidCallback;

// rewarded callbacks
static HeliumPlacementEvent _rewardedDidLoadCallback;
static HeliumPlacementEvent _rewardedDidClickCallback;
static HeliumPlacementEvent _rewardedDidCloseCallback;
static HeliumPlacementEvent _rewardedDidShowCallback;
static HeliumPlacementEvent _rewardedDidRecordImpressionCallback;
static HeliumBidWinEvent _rewardedDidWinBidCallback;
static HeliumRewardEvent _rewardedDidReceiveRewardCallback;

// banner callbacks
static HeliumPlacementEvent _bannerDidLoadCallback;
static HeliumPlacementEvent _bannerDidRecordImpressionCallback;
static HeliumPlacementEvent _bannerDidClickCallback;
static HeliumBidWinEvent _bannerDidWinBidCallback;

void UnityPause(int pause);

const char* serializeDictionary(NSDictionary *data)
{
	NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data options:0 error:NULL];
	NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    NSLog(@"event: %@", json);
	return json.UTF8String;
}

const void serializeError(HeliumError *error, HeliumEvent event)
{
    if (event == nil)
        return;

    int errorCode = -1;
    const char* errorDescription = "";

    if (error != nil)
    {
        errorCode =  [[NSNumber numberWithFloat:error.code] intValue];
        errorDescription = error.localizedDescription.UTF8String;
    }

    event(errorCode, errorDescription);
}

const void serializePlacementWithError(NSString *placementName, HeliumError *error, HeliumPlacementEvent placementEvent)
{
    if (placementEvent == nil)
        return;


    int errorCode = -1;
    const char* errorDescription = "";

    if (error != nil)
    {
        errorCode =  [[NSNumber numberWithFloat:error.code] intValue];
        errorDescription = error.localizedDescription.UTF8String;
    }

    placementEvent(placementName.UTF8String, errorCode, errorDescription);
}

const void serializeReward(NSString *placementName, NSInteger reward, HeliumRewardEvent rewardEvent)
{
    if (rewardEvent == nil)
        return;
    int rewardValue = (int)reward;
    rewardEvent(placementName.UTF8String, rewardValue);
}

const void serializeWinBidInfo(NSString *placementName, NSDictionary* info, HeliumBidWinEvent bidWinEvent)
{
    NSString* partnerId = [info objectForKey:@"partner-id"];
    NSString* auctionId = [info objectForKey:@"auction-id"];
    NSNumber* price = [info objectForKey:@"price"];

    if (partnerId == nil)
        partnerId = @"";

    if (auctionId == nil)
        auctionId = @"";

    if (price == nil)
        price = 0;

    bidWinEvent(placementName.UTF8String, auctionId.UTF8String, partnerId.UTF8String, [price doubleValue]);
}


static void heliumSubscribeToILRDNotifications()
{
    static id ilrdObserverId = nil;
    if (ilrdObserverId != nil)
        [[NSNotificationCenter defaultCenter] removeObserver:ilrdObserverId];
    ilrdObserverId = [[NSNotificationCenter defaultCenter] addObserverForName:kHeliumDidReceiveILRDNotification object:nil queue:nil usingBlock:^(NSNotification* _Nonnull note) {
        HeliumImpressionData *ilrd = note.object;
        NSString *placement = ilrd.placement;
        NSDictionary *json = ilrd.jsonData;
        NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
                              placement, @"placementName",
                              json ? json : [NSNull null], @"ilrd",
                              nil];
        const char* jsonToUnity = serializeDictionary(data);

        if (_didReceiveILRDCallback != nil)
            _didReceiveILRDCallback(jsonToUnity);
    }];
}

@interface HeliumSdkManager() <HeliumSdkDelegate, CHBHeliumInterstitialAdDelegate, CHBHeliumRewardedAdDelegate, HeliumBannerAdDelegate>

@end

@implementation HeliumSdkManager

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (HeliumSdkManager*)sharedManager
{
	static HeliumSdkManager *sharedSingleton;

	if (!sharedSingleton)
		sharedSingleton = [[HeliumSdkManager alloc] init];

	return sharedSingleton;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)setLifeCycleCallbacks:(HeliumEvent)didStartCallback didReceiveILRDCallback:(HeliumILRDEvent)didReceiveILRDCallback
{
    _didStartCallback = didStartCallback;
    _didReceiveILRDCallback = didReceiveILRDCallback;
}

- (void)setInterstitialCallbacks:(HeliumPlacementEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didCloseCallback:(HeliumPlacementEvent)didCloseCallback didRecordImpression:(HeliumPlacementEvent)didRecordImpression didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback
{
    _interstitialDidLoadCallback = didLoadCallback;
    _interstitialDidShowCallback = didShowCallback;
    _interstitialDidClickCallback = didClickCallback;
    _interstitialDidCloseCallback = didCloseCallback;
    _interstitialDidRecordImpressionCallback = didRecordImpression;
    _interstitialDidWinBidCallback = didWinBidCallback;
}

- (void)setRewardedCallbacks:(HeliumPlacementEvent)didLoadCallback didShowCallback:(HeliumPlacementEvent)didShowCallback didClickCallback:(HeliumPlacementEvent)didClickCallback didCloseCallback:(HeliumPlacementEvent)didCloseCallback didRecordImpression:(HeliumPlacementEvent)didRecordImpression didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback didReceiveRewardCallback:(HeliumRewardEvent)didReceiveRewardCallback
{
    _rewardedDidLoadCallback = didLoadCallback;
    _rewardedDidShowCallback = didShowCallback;
    _rewardedDidClickCallback = didClickCallback;
    _rewardedDidCloseCallback = didCloseCallback;
    _rewardedDidRecordImpressionCallback = didRecordImpression;
    _rewardedDidWinBidCallback = didWinBidCallback;
    _rewardedDidReceiveRewardCallback = didReceiveRewardCallback;
}

- (void)setBannerCallbacks:(HeliumPlacementEvent)didLoadCallback didRecordImpression:(HeliumPlacementEvent)didRecordImpression didClickCallback:(HeliumPlacementEvent)didClickCallback didWinBidCallback:(HeliumBidWinEvent)didWinBidCallback
{
    _bannerDidLoadCallback = didLoadCallback;
    _bannerDidRecordImpressionCallback = didRecordImpression;
    _bannerDidClickCallback = didClickCallback;
    _bannerDidWinBidCallback = didWinBidCallback;
}

- (void)startHeliumWithAppId:(NSString*)appId
             andAppSignature:(NSString*)appSignature
                unityVersion:(NSString *)unityVersion
{
    heliumSubscribeToILRDNotifications();
    [[Helium sharedHelium] startWithAppId: appId andAppSignature:appSignature delegate: self];
}

- (void)setSubjectToCoppa:(BOOL)isSubject
{
    [[Helium sharedHelium] setSubjectToCoppa: isSubject];
}

- (void)setSubjectToGDPR:(BOOL)isSubject
{
    [[Helium sharedHelium] setSubjectToGDPR: isSubject];
}

- (void)setUserHasGivenConsent:(BOOL)hasGivenConsent
{
    [[Helium sharedHelium] setUserHasGivenConsent: hasGivenConsent];
}

- (void)setCCPAConsent:(BOOL)hasGivenConsent
{
    [[Helium sharedHelium] setCCPAConsent: hasGivenConsent];
}

- (void)setUserIdentifier:(NSString*)userIdentifier
{
    [Helium sharedHelium].userIdentifier = userIdentifier;
}

- (NSString*)getUserIdentifier
{
    return [Helium sharedHelium].userIdentifier;
}

- (id<HeliumInterstitialAd>)getInterstitialAd:(NSString*)placementName
{
    id<HeliumInterstitialAd> ad = [[Helium sharedHelium] interstitialAdProviderWithDelegate: self andPlacementName: placementName];
    if (ad == NULL)
        return NULL;

    // Else return the address of the ad as an int, which can be used as a unique id
    // Also store the object in a dictionary so that it can later be deleted
    if (storedAds == nil)
        storedAds = [[NSMutableDictionary alloc] init];
    [storedAds setObject:ad forKey:[NSNumber numberWithLong:(long)ad]];
    return ad;
}

- (id<HeliumRewardedAd>)getRewardedAd:(NSString*)placementName
{
    id<HeliumRewardedAd> ad = [[Helium sharedHelium] rewardedAdProviderWithDelegate: self andPlacementName: placementName];
    if (ad == NULL)
        return NULL;

    // Else return the address of the ad as an int, which can be used as a unique id
    // Also store the object in a dictionary so that it can later be deleted
    if (storedAds == nil)
        storedAds = [[NSMutableDictionary alloc] init];
    [storedAds setObject:ad forKey:[NSNumber numberWithLong:(long)ad]];
    return ad;
}

- (HeliumBannerView*)getBannerAd:(NSString*)placementName andSize:(CHBHBannerSize)size
{
    HeliumBannerView* ad = [[Helium sharedHelium] bannerProviderWithDelegate:self andPlacementName:placementName andSize:size];
    if (ad == NULL)
        return NULL;

    // Else return the address of the ad as an int, which can be used as a unique id
    // Also store the object in a dictionary so that it can later be deleted
    if (storedAds == nil)
        storedAds = [[NSMutableDictionary alloc] init];
    [storedAds setObject:ad forKey:[NSNumber numberWithLong:(long)ad]];
    return ad;
}

- (void)freeInterstitialAd:(NSNumber*)adId
{
    [storedAds removeObjectForKey:adId];
}

- (void)freeRewardedAd:(NSNumber*)adId
{
    [storedAds removeObjectForKey:adId];
}

- (void)freeBannerAd:(NSNumber*)adId
{
    [storedAds removeObjectForKey:adId];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark HeliumSdkDelegate

- (void)heliumDidStartWithError:(HeliumError *)error;
{
    serializeError(error, _didStartCallback);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CHBHeliumInterstitialAdDelegate

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                             didLoadWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _interstitialDidLoadCallback);
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                             didShowWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _interstitialDidShowCallback);
    if (!error) {
        UnityPause(true);
    }
}

- (void)heliumInterstitialAdWithPlacementName:(NSString *)placementName
                            didClickWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _interstitialDidClickCallback);
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                            didCloseWithError:(HeliumError *)error
{
    UnityPause(false);
    serializePlacementWithError(placementName, error, _interstitialDidCloseCallback);
}

- (void)heliumInterstitialAdDidRecordImpressionWithPlacementName: (NSString*)placementName
{
    serializePlacementWithError(placementName, nil, _interstitialDidRecordImpressionCallback);
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                    didLoadWinningBidWithInfo:(NSDictionary*)info
{
    serializeWinBidInfo(placementName, info, _interstitialDidWinBidCallback);
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CHBHeliumRewardedVideoAdDelegate

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                         didLoadWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _rewardedDidLoadCallback);
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                         didShowWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _rewardedDidShowCallback);
    if (!error) {
        UnityPause(true);
    }
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                        didClickWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _rewardedDidClickCallback);
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                        didCloseWithError:(HeliumError *)error
{
    UnityPause(false);
    serializePlacementWithError(placementName, error, _rewardedDidCloseCallback);
}

- (void)heliumRewardedAdDidRecordImpressionWithPlacementName: (NSString*)placementName
{
    serializePlacementWithError(placementName, nil, _rewardedDidRecordImpressionCallback);
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                             didGetReward:(NSInteger)reward
{
    serializeReward(placementName, reward, _rewardedDidReceiveRewardCallback);
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                didLoadWinningBidWithInfo:(NSDictionary*)info
{
    serializeWinBidInfo(placementName, info, _rewardedDidWinBidCallback);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CHBHeliumBannerAdDelegate

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
                       didLoadWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _bannerDidLoadCallback);
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
                      didClickWithError:(HeliumError *)error
{
    serializePlacementWithError(placementName, error, _bannerDidClickCallback);
}

- (void)heliumBannerAdDidRecordImpressionWithPlacementName: (NSString*)placementName
{
    serializePlacementWithError(placementName, nil, _bannerDidRecordImpressionCallback);
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
              didLoadWinningBidWithInfo:(NSDictionary*)info
{
    serializeWinBidInfo(placementName, info, _bannerDidWinBidCallback);
}
@end
