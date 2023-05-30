/*
* ChartboostMediationManager.mm
* Chartboost Mediation SDK iOS/Unity
*/

#import <objc/runtime.h>
#import "ChartboostMediationManager.h"
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>
#import <ChartboostMediationSDK/HeliumInitResultsEvent.h>

struct Implementation {
    SEL selector;
    IMP imp;
};

template <typename TObj>
TObj objFromJsonString(const char* jsonString) {
    NSData* jsonData = [[NSString stringWithUTF8String:jsonString] dataUsingEncoding:NSUTF8StringEncoding];
    NSError* error = nil;
    TObj arr = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];

    if (error != nil)
        return nil;

    return arr;
}

// interstitial ad objects
NSMutableDictionary * storedAds = nil;

// lifecycle callbacks
static ChartboostMediationEvent _didStartCallback;
static ChartboostMediationILRDEvent _didReceiveILRDCallback;
static ChartboostMediationPartnerInitializationDataEvent _didReceivePartnerInitializationDataCallback;

// interstitial callbacks
static ChartboostMediationPlacementLoadEvent _interstitialDidLoadCallback;
static ChartboostMediationPlacementEvent _interstitialDidClickCallback;
static ChartboostMediationPlacementEvent _interstitialDidCloseCallback;
static ChartboostMediationPlacementEvent _interstitialDidShowCallback;
static ChartboostMediationPlacementEvent _interstitialDidRecordImpressionCallback;

// rewarded callbacks
static ChartboostMediationPlacementLoadEvent _rewardedDidLoadCallback;
static ChartboostMediationPlacementEvent _rewardedDidClickCallback;
static ChartboostMediationPlacementEvent _rewardedDidCloseCallback;
static ChartboostMediationPlacementEvent _rewardedDidShowCallback;
static ChartboostMediationPlacementEvent _rewardedDidRecordImpressionCallback;
static ChartboostMediationPlacementEvent _rewardedDidReceiveRewardCallback;

// fullscreen callbacks
static ChartboostMediationFullscreenAdEvent _fullscreenAdEvents;
enum fullscreenEvents {RecordImpression = 0, Click = 1, Reward = 2, Close = 3, Expire = 4};

// banner callbacks
static ChartboostMediationPlacementLoadEvent _bannerDidLoadCallback;
static ChartboostMediationPlacementEvent _bannerDidRecordImpressionCallback;
static ChartboostMediationPlacementEvent _bannerDidClickCallback;

void UnityPause(int pause);

const char* dictionaryToJSON(NSDictionary *data)
{
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data options:0 error:&error];
    if (! jsonData) {
        NSLog(@"%s: error: %@", __func__, error.localizedDescription);
        return "";
     }
    NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return [json UTF8String];
}

const void addToStore(id ad, NSString *placement, BOOL multiplePlacementSupport){
    if (storedAds == nil)
        storedAds = [[NSMutableDictionary alloc] init];
    
    if (multiplePlacementSupport)
    {
        NSNumber *key = [NSNumber numberWithLong:(long)ad];
        [storedAds setObject:ad forKey:key];
    }
    else
        [storedAds setObject:ad forKey:placement];
}

const void serializeEvent(ChartboostMediationError *error, ChartboostMediationEvent event)
{
    if (event == nil)
        return;
    
    event(error.localizedDescription.UTF8String);
}

const void serializeFullscreenEvent(id<ChartboostMediationFullscreenAd> ad, fullscreenEvents event, ChartboostMediationError *error)
{
    const char *code = "";
    const char *message = "";
    
    if (error != nil)
    {
        ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
        code = [[NSString stringWithFormat:@"CM_%ld", codeInt] UTF8String];
        message = [[error localizedDescription] UTF8String];
    }
    
    _fullscreenAdEvents((long)ad, (int)event, code, message);
}

const void serializePlacementWithError(NSString *placementName, ChartboostMediationError *error, ChartboostMediationPlacementEvent placementEvent)
{
    if (placementEvent == nil)
        return;
    
    placementEvent(placementName.UTF8String, error.localizedDescription.UTF8String);
}

const void serializePlacementLoadWithError(NSString *placementName, NSString *requestIdentifier, NSDictionary *winningBidInfo, ChartboostMediationError *error, ChartboostMediationPlacementLoadEvent placementLoadEvent)
{
    if (placementLoadEvent == nil)
        return;
    
    NSString* partnerId = [winningBidInfo objectForKey:@"partner-id"];
    NSString* auctionId = [winningBidInfo objectForKey:@"auction-id"];
    NSNumber* price = [winningBidInfo objectForKey:@"price"];

    if (partnerId == nil)
        partnerId = @"";

    if (auctionId == nil)
        auctionId = @"";

    if (price == nil)
        price = 0;
    
    placementLoadEvent(placementName.UTF8String, requestIdentifier.UTF8String, auctionId.UTF8String, partnerId.UTF8String, [price doubleValue], error.localizedDescription.UTF8String);
}

static void subscribeToILRDNotifications()
{
    static id ilrdObserverId = nil;

    if (ilrdObserverId != nil)
        [[NSNotificationCenter defaultCenter] removeObserver:ilrdObserverId];

    ilrdObserverId = [[NSNotificationCenter defaultCenter] addObserverForName:NSNotification.heliumDidReceiveILRD object:nil queue:nil usingBlock:^(NSNotification* _Nonnull note) {
        HeliumImpressionData *ilrd = note.object;
        NSString *placement = ilrd.placement;
        NSDictionary *json = ilrd.jsonData;
        NSDictionary *data = [NSDictionary dictionaryWithObjectsAndKeys:
                              placement, @"placementName",
                              json ? json : [NSNull null], @"ilrd",
                              nil];
        const char* jsonToUnity = dictionaryToJSON(data);

        if (_didReceiveILRDCallback != nil)
            _didReceiveILRDCallback(jsonToUnity);
    }];
}

static void subscribeToPartnerInitializationNotifications()
{
    static id partnerInitializationObserver = nil;

    if (partnerInitializationObserver != nil)
        [[NSNotificationCenter defaultCenter] removeObserver:partnerInitializationObserver];

    partnerInitializationObserver = [[NSNotificationCenter defaultCenter] addObserverForName:NSNotification.heliumDidReceiveInitResults object:nil queue:nil usingBlock:^(NSNotification * _Nonnull notification) {
        // Extract the results payload.
        NSDictionary *results = (NSDictionary *)notification.object;
        const char* jsonToUnity = dictionaryToJSON(results);
        if (_didReceivePartnerInitializationDataCallback != nil)
            _didReceivePartnerInitializationDataCallback(jsonToUnity);
    }];
}

@interface ChartboostMediationManager() <HeliumSdkDelegate, ChartboostMediationFullscreenAdDelegate, CHBHeliumInterstitialAdDelegate, CHBHeliumRewardedAdDelegate, CHBHeliumBannerAdDelegate>

@end

@implementation ChartboostMediationManager

-(Implementation)getImplementationFromClassNamed:(NSString*)className selectorName:(NSString*)selectorName
{
    Class cls = NSClassFromString(className);
    SEL selector = NSSelectorFromString(selectorName);
    Method method = class_getClassMethod(cls, selector);
    IMP imp = method_getImplementation(method);
    struct Implementation implementation;
    implementation.selector = selector;
    implementation.imp = imp;
    return implementation;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (ChartboostMediationManager*)sharedManager
{
    static ChartboostMediationManager *sharedSingleton;
    
    if (!sharedSingleton)
        sharedSingleton = [[ChartboostMediationManager alloc] init];
    
    return sharedSingleton;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)setLifeCycleCallbacks:(ChartboostMediationEvent)didStartCallback didReceiveILRDCallback:(ChartboostMediationILRDEvent)didReceiveILRDCallback didReceivePartnerInitializationData:(ChartboostMediationPartnerInitializationDataEvent)didReceivePartnerInitializationDataCallback
{
    _didStartCallback = didStartCallback;
    _didReceiveILRDCallback = didReceiveILRDCallback;
    _didReceivePartnerInitializationDataCallback = didReceivePartnerInitializationDataCallback;
}

- (void)setInterstitialCallbacks:(ChartboostMediationPlacementLoadEvent)didLoadCallback didShowCallback:(ChartboostMediationPlacementEvent)didShowCallback  didCloseCallback:(ChartboostMediationPlacementEvent)didCloseCallback didClickCallback:(ChartboostMediationPlacementEvent)didClickCallback didRecordImpression:(ChartboostMediationPlacementEvent)didRecordImpression
{
    _interstitialDidLoadCallback = didLoadCallback;
    _interstitialDidShowCallback = didShowCallback;
    _interstitialDidClickCallback = didClickCallback;
    _interstitialDidCloseCallback = didCloseCallback;
    _interstitialDidRecordImpressionCallback = didRecordImpression;
}

- (void)setRewardedCallbacks:(ChartboostMediationPlacementLoadEvent)didLoadCallback didShowCallback:(ChartboostMediationPlacementEvent)didShowCallback didCloseCallback:(ChartboostMediationPlacementEvent)didCloseCallback didClickCallback:(ChartboostMediationPlacementEvent)didClickCallback didRecordImpression:(ChartboostMediationPlacementEvent)didRecordImpression didReceiveRewardCallback:(ChartboostMediationPlacementEvent)didReceiveRewardCallback
{
    _rewardedDidLoadCallback = didLoadCallback;
    _rewardedDidShowCallback = didShowCallback;
    _rewardedDidClickCallback = didClickCallback;
    _rewardedDidCloseCallback = didCloseCallback;
    _rewardedDidRecordImpressionCallback = didRecordImpression;
    _rewardedDidReceiveRewardCallback = didReceiveRewardCallback;
}

- (void)setFullscreenCallbacks:(ChartboostMediationFullscreenAdEvent)fullscreenAdEvents {
    _fullscreenAdEvents = fullscreenAdEvents;
}

- (void)setBannerCallbacks:(ChartboostMediationPlacementLoadEvent)didLoadCallback didRecordImpression:(ChartboostMediationPlacementEvent)didRecordImpression didClickCallback:(ChartboostMediationPlacementEvent)didClickCallback
{
    _bannerDidLoadCallback = didLoadCallback;
    _bannerDidRecordImpressionCallback = didRecordImpression;
    _bannerDidClickCallback = didClickCallback;
}

- (void)startHeliumWithAppId:(NSString*)appId andAppSignature:(NSString*)appSignature unityVersion:(NSString *)unityVersion initializationOptions:(const char**)initializationOptions initializationOptionsSize:(int)initializationOptionsSize
{
    subscribeToILRDNotifications();
    subscribeToPartnerInitializationNotifications();
    HeliumInitializationOptions* heliumInitializationOptions = nil;
    
    if (initializationOptionsSize > 0) {
        NSMutableArray *initializationPartners = [NSMutableArray new];
        for (int x = 0; x < initializationOptionsSize; x++)
        {
            if(strlen(initializationOptions[x]) > 0)
                [initializationPartners addObject:[NSString stringWithUTF8String:initializationOptions[x]]];
        }
        heliumInitializationOptions = [[HeliumInitializationOptions alloc] initWithSkippedPartnerIdentifiers:initializationPartners];
    }
    
    [[Helium sharedHelium] startWithAppId:appId andAppSignature:appSignature options:heliumInitializationOptions delegate:self];
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

-(void)setTestMode:(BOOL)isTestModeEnabled
{
    Implementation implementation = [self getImplementationFromClassNamed:@"CHBHTestModeHelper" selectorName:@"setIsTestModeEnabled_isForcedOn:"];
    typedef void (*Signature)(id, SEL, BOOL);
    Signature function = (Signature)implementation.imp;
    function(self, implementation.selector, isTestModeEnabled);
}

- (id<HeliumInterstitialAd>)getInterstitialAd:(NSString*)placementName
{
    id<HeliumInterstitialAd> ad = [[Helium sharedHelium] interstitialAdProviderWithDelegate: self andPlacementName: placementName];
    if (ad == NULL)
        return NULL;
    
    addToStore(ad, placementName, FALSE);
    return ad;
}

- (id<HeliumRewardedAd>)getRewardedAd:(NSString*)placementName
{
    id<HeliumRewardedAd> ad = [[Helium sharedHelium] rewardedAdProviderWithDelegate: self andPlacementName: placementName];
    if (ad == NULL)
        return NULL;
    
    addToStore(ad, placementName, FALSE);
    return ad;
}

- (void) getFullscreenAd:(NSString *)placementId keywords:(const char *)keywords hashCode:(int)hashCode callback:(ChartboostMediationFullscreenAdLoadResultEvent)callback  {
    
    NSDictionary *formattedKeywords = objFromJsonString<NSDictionary *>(keywords);
    
    auto *loadRequest = [[ChartboostMediationAdLoadRequest alloc] initWithPlacement:placementId keywords:formattedKeywords];
    
    [[Helium sharedHelium] loadFullscreenAdWithRequest:loadRequest completion:^(ChartboostMediationFullscreenAdLoadResult * adLoadResult) {
        ChartboostMediationError *error = [adLoadResult error];
        if (error != nil)
        {
            ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
            const char *code = [[NSString stringWithFormat:@"CM_%ld", codeInt] UTF8String];
            const char *message = [[error localizedDescription] UTF8String];
            callback(hashCode, NULL, "", "", "", code, message);
            return;
        }
        
        id<ChartboostMediationFullscreenAd> ad = [adLoadResult ad];
        [ad setDelegate:self];
        addToStore(ad, placementId, true);
        const char *loadId = [[adLoadResult loadID] UTF8String];
        const char *winningBidJson = dictionaryToJSON([ad winningBidInfo]);
        const char *metricsJson = dictionaryToJSON([adLoadResult metrics]);
        callback(hashCode, (__bridge void*)ad, loadId, winningBidJson, metricsJson, "", "");
    }];
}

- (HeliumBannerView*)getBannerAd:(NSString*)placementName andSize:(CHBHBannerSize)size
{
    HeliumBannerView *ad = [[Helium sharedHelium] bannerProviderWithDelegate:self andPlacementName:placementName andSize:size];
    if (ad == NULL)
        return NULL;
    addToStore(ad, placementName, TRUE);
    return ad;
}

- (void)freeAd:(NSNumber*)adId placementName:(NSString *)placementName multiPlacementSupport:(BOOL)multiPlacementSupport
{
    if (multiPlacementSupport)
        [storedAds removeObjectForKey:adId];
    else
    {
        if (placementName != nil)
            [storedAds removeObjectForKey:placementName];
    }
}

#pragma mark HeliumSdkDelegate

- (void)heliumDidStartWithError:(ChartboostMediationError *)error;
{
    serializeEvent(error, _didStartCallback);
}

#pragma mark CHBHeliumInterstitialAdDelegate

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName requestIdentifier:(NSString *) requestIdentifier winningBidInfo:(NSDictionary<NSString *, id> *)winningBidInfo didLoadWithError:(ChartboostMediationError *)error
{
    serializePlacementLoadWithError(placementName, requestIdentifier, winningBidInfo, error, _interstitialDidLoadCallback);
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName didShowWithError:(ChartboostMediationError *)error
{
    serializePlacementWithError(placementName, error, _interstitialDidShowCallback);
    if (!error) {
        UnityPause(true);
    }
}

- (void)heliumInterstitialAdWithPlacementName:(NSString*)placementName didCloseWithError:(ChartboostMediationError *)error
{
    UnityPause(false);
    serializePlacementWithError(placementName, error, _interstitialDidCloseCallback);
}

- (void)heliumInterstitialAdWithPlacementName:(NSString *)placementName didClickWithError:(ChartboostMediationError *)error
{
    serializePlacementWithError(placementName, error, _interstitialDidClickCallback);
}

- (void)heliumInterstitialAdDidRecordImpressionWithPlacementName: (NSString*)placementName
{
    serializePlacementWithError(placementName, nil, _interstitialDidRecordImpressionCallback);
}

#pragma mark CHBHeliumRewardedVideoAdDelegate
- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName requestIdentifier:(NSString *) requestIdentifier winningBidInfo:(NSDictionary<NSString *, id> *)winningBidInfo didLoadWithError:(ChartboostMediationError *)error
{
    serializePlacementLoadWithError(placementName, requestIdentifier, winningBidInfo, error, _rewardedDidLoadCallback);
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName didShowWithError:(ChartboostMediationError *)error
{
    serializePlacementWithError(placementName, error, _rewardedDidShowCallback);
    if (!error) {
        UnityPause(true);
    }
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName didCloseWithError:(ChartboostMediationError *)error
{
    UnityPause(false);
    serializePlacementWithError(placementName, error, _rewardedDidCloseCallback);
}

- (void)heliumRewardedAdWithPlacementName:(NSString*)placementName didClickWithError:(ChartboostMediationError *)error
{
    serializePlacementWithError(placementName, error, _rewardedDidClickCallback);
}

- (void)heliumRewardedAdDidRecordImpressionWithPlacementName: (NSString*)placementName
{
    serializePlacementWithError(placementName, nil, _rewardedDidRecordImpressionCallback);
}

- (void)heliumRewardedAdDidGetRewardWithPlacementName:(NSString *)placementName
{
    serializePlacementWithError(placementName, nil, _rewardedDidReceiveRewardCallback);
}

#pragma mark CHBHeliumBannerAdDelegate

- (void)heliumBannerAdWithPlacementName:(NSString*)placementName requestIdentifier:(NSString *) requestIdentifier winningBidInfo:(NSDictionary<NSString *, id> *)winningBidInfo didLoadWithError:(ChartboostMediationError *)error
{
    serializePlacementLoadWithError(placementName, requestIdentifier, winningBidInfo, error, _bannerDidLoadCallback);
}

- (void)heliumBannerAdWithPlacementName:(NSString *)placementName didClickWithError:(ChartboostMediationError *)error
{
    serializePlacementWithError(placementName, error, _bannerDidClickCallback);
}

- (void)heliumBannerAdDidRecordImpressionWithPlacementName: (NSString*)placementName
{
    serializePlacementWithError(placementName, nil, _bannerDidRecordImpressionCallback);
}

#pragma mark ChartboostMediationFullscreenAdDelegate
- (void)didRecordImpressionWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    serializeFullscreenEvent(ad, RecordImpression, nil);
}

- (void)didClickWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    serializeFullscreenEvent(ad, Click, nil);
}
- (void)didRewardWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    serializeFullscreenEvent(ad, Reward, nil);
}

- (void)didCloseWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad error:(ChartboostMediationError * _Nullable)error {
    serializeFullscreenEvent(ad, Close, error);
}

- (void)didExpireWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    serializeFullscreenEvent(ad, Expire, nil);
}
@end
