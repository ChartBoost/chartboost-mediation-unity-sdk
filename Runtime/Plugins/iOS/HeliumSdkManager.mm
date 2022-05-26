/*
 * HeliumSdkManager.mm
 * Helium SDK
 */

#import "HeliumSdkManager.h"

// interstitial ad objects
NSMutableDictionary * storedAds = nil;

void UnityPause(int pause);
void UnitySendMessage(const char *className, const char *methodName, const char *param);

const char* serializeDictionary(NSDictionary *data)
{
	NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data options:0 error:NULL];
	NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    NSLog(@"event: %@", json);
	return json.UTF8String;
}

const char* serializeError(HeliumError *error)
{
	NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
						  error ? error.errorDescription : [NSNull null], @"errorDescription",
                          [NSNumber numberWithInteger: error ? error.errorCode : -1], @"errorCode",
						  nil];
	return serializeDictionary(data);
}

const char* serializePlacementError(NSString *placementName, HeliumError *error)
{
	NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
                          placementName, @"placementName",
                          error ? error.errorDescription : [NSNull null], @"errorDescription",
                          [NSNumber numberWithInteger: error ? error.errorCode : -1], @"errorCode",
						  nil];
	return serializeDictionary(data);
}

const char* serializeReward(NSString *placementName, NSInteger reward)
{
	NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
                          placementName, @"placementName",
                          [NSNumber numberWithInteger: reward], @"reward",
                          nil];
	return serializeDictionary(data);
}

const char* serializePlacementInfoDictionary(NSString *placementName, NSDictionary *info)
{
    NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
                          placementName, @"placementName",
                          info ? info : [NSNull null], @"info",
                          nil];
    return serializeDictionary(data);
}

const char* serializePlacementILRDDictionary(NSString *placementName, NSDictionary *ilrd)
{
    NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
                          placementName, @"placementName",
                          ilrd ? ilrd : [NSNull null], @"ilrd",
                          nil];
    return serializeDictionary(data);
}

static HeliumBackgroundEventCallback _bgEventCallback;

static void heliumSubscribeToILRDNotifications()
{
    static id ilrdObserverId = nil;
    if (ilrdObserverId != nil)
        [[NSNotificationCenter defaultCenter] removeObserver:ilrdObserverId];
    ilrdObserverId = [[NSNotificationCenter defaultCenter] addObserverForName:kHeliumDidReceiveILRDNotification object:nil queue:nil usingBlock:^(NSNotification* _Nonnull note) {
        HeliumImpressionData *ilrd = note.object;
        NSString *placement = ilrd.placement;
        NSDictionary *json = ilrd.jsonData;
        [HeliumSdkManager sendUnityEvent:@"DidReceiveILRD" withParam:serializePlacementILRDDictionary(placement, json) backgroundOK:YES];
    }];
}

@interface HeliumSdkManager() <HeliumSdkDelegate, CHBHeliumInterstitialAdDelegate, CHBHeliumRewardedAdDelegate, CHBHeliumBannerAdDelegate>

@end

@implementation HeliumSdkManager

@synthesize gameObjectName;

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

- (void)startHeliumWithAppId:(NSString*)appId
             andAppSignature:(NSString*)appSignature
                unityVersion:(NSString *)unityVersion
             bgEventCallback:(HeliumBackgroundEventCallback)bgEventCallback
{
    [[self class] setBgEventCallback:bgEventCallback];
    if (bgEventCallback) {
        heliumSubscribeToILRDNotifications();
    }
    [[HeliumSdk sharedHelium] startWithAppId: appId andAppSignature:appSignature delegate: self];
}

+ (HeliumBackgroundEventCallback)bgEventCallback
{
    return _bgEventCallback;
}

+ (void)setBgEventCallback:(HeliumBackgroundEventCallback)cb
{
    _bgEventCallback = cb;
}

- (void)setSubjectToCoppa:(BOOL)isSubject
{
    [[HeliumSdk sharedHelium] setSubjectToCoppa: isSubject];
}

- (void)setSubjectToGDPR:(BOOL)isSubject
{
    [[HeliumSdk sharedHelium] setSubjectToGDPR: isSubject];
}

- (void)setUserHasGivenConsent:(BOOL)hasGivenConsent
{
    [[HeliumSdk sharedHelium] setUserHasGivenConsent: hasGivenConsent];
}

- (void)setCCPAConsent:(BOOL)hasGivenConsent
{
    [[HeliumSdk sharedHelium] setCCPAConsent: hasGivenConsent];
}

- (void)setUserIdentifier:(NSString*)userIdentifier
{
    [HeliumSdk sharedHelium].userIdentifier = userIdentifier;
}

- (NSString*)getUserIdentifier
{
    return [HeliumSdk sharedHelium].userIdentifier;
}

- (id<HeliumInterstitialAd>)getInterstitialAd:(NSString*)placementName
{
    id<HeliumInterstitialAd> ad = [[HeliumSdk sharedHelium] interstitialAdProviderWithDelegate: self andPlacementName: placementName];
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
    id<HeliumRewardedAd> ad = [[HeliumSdk sharedHelium] rewardedAdProviderWithDelegate: self andPlacementName: placementName];
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
    HeliumBannerView* ad = [[HeliumSdk sharedHelium] bannerProviderWithDelegate:self andPlacementName:placementName andSize:size];
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

+ (void)sendUnityEvent:(NSString*)eventName withParam:(const char*)param backgroundOK:(BOOL)bg
{
    if (bg && _bgEventCallback != nil)
        _bgEventCallback(eventName.UTF8String, param);
    else
        UnitySendMessage([HeliumSdkManager sharedManager].gameObjectName.UTF8String, eventName.UTF8String, param);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark HeliumSdkDelegate

- (void)heliumDidStartWithError:(HeliumError *)error;
{
    [self sendUnityEvent:@"DidStartEvent" withParam:serializeError(error)];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CHBHeliumInterstitialAdDelegate

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                             didLoadWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidLoadInterstitialEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                             didShowWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidShowInterstitialEvent" withParam:serializePlacementError(placementName, error)];
    if (!error) {
        UnityPause(true);
    }
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                            didCloseWithError:(HeliumError *)error
{
    UnityPause(false);
    [self sendUnityEvent:@"DidCloseInterstitialEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName
                    didLoadWinningBidWithInfo:(NSDictionary*)info
{
    [self sendUnityEvent:@"DidWinBidInterstitialEvent" withParam:serializePlacementInfoDictionary(placementName, info)];
}

- (void)heliumInterstitialAdWithPlacementName:(NSString *)placementName
                            didClickWithError:(nullable HeliumError *)error
{
    [self sendUnityEvent:@"DidClickInterstitialEvent" withParam:serializePlacementError(placementName, error)];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CHBHeliumRewardedVideoAdDelegate

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                         didLoadWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidLoadRewardedEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                         didShowWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidShowRewardedEvent" withParam:serializePlacementError(placementName, error)];
    if (!error) {
        UnityPause(true);
    }
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                        didCloseWithError:(HeliumError *)error
{
    UnityPause(false);
    [self sendUnityEvent:@"DidCloseRewardedEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                             didGetReward:(NSInteger)reward
{
    [self sendUnityEvent:@"DidReceiveRewardEvent" withParam:serializeReward(placementName, reward) backgroundOK:YES];
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName
                didLoadWinningBidWithInfo:(NSDictionary*)info
{
    [self sendUnityEvent:@"DidWinBidRewardedEvent" withParam:serializePlacementInfoDictionary(placementName, info)];
}

- (void)heliumRewardedAdWithPlacementName:(NSString *)placementName
                        didClickWithError:(nullable HeliumError *)error
{
    [self sendUnityEvent:@"DidClickRewardedEvent" withParam:serializePlacementError(placementName, error)];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CHBHeliumBannerAdDelegate

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
                       didLoadWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidLoadBannerEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
                       didShowWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidShowBannerEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
                      didCloseWithError:(HeliumError *)error
{
    [self sendUnityEvent:@"DidCloseBannerEvent" withParam:serializePlacementError(placementName, error)];
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
              didLoadWinningBidWithInfo:(NSDictionary*)info
{
    [self sendUnityEvent:@"DidWinBidBannerEvent" withParam:serializePlacementInfoDictionary(placementName, info)];
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName
                      didClickWithError:(nullable HeliumError *)error
{
    [self sendUnityEvent:@"DidClickBannerEvent" withParam:serializePlacementError(placementName, error)];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

- (void)sendUnityEvent:(NSString*)eventName withParam:(const char*)param backgroundOK:(BOOL)bg
{
    [[self class] sendUnityEvent:eventName withParam:param backgroundOK:bg];
}

- (void)sendUnityEvent:(NSString*)eventName withParam:(const char*)param
{
    [[self class] sendUnityEvent:eventName withParam:param backgroundOK:NO];
}

@end
