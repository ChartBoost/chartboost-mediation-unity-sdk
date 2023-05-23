/*
* ChartboostMediationManager.mm
* Chartboost Mediation SDK iOS/Unity
*/

#import <objc/runtime.h>
#import "ChartboostMediationManager.h"
#import "ChartboostMediationBannerAdDragger.h"
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>
#import <ChartboostMediationSDK/HeliumInitResultsEvent.h>


struct Implementation {
    SEL selector;
    IMP imp;
};

// interstitial ad objects
NSMutableDictionary * storedAds = nil;
NSMutableDictionary * bannerDraggers = nil;

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

// banner callbacks
static ChartboostMediationPlacementLoadEvent _bannerDidLoadCallback;
static ChartboostMediationPlacementEvent _bannerDidRecordImpressionCallback;
static ChartboostMediationPlacementEvent _bannerDidClickCallback;

void UnityPause(int pause);

const char* serializeDictionary(NSDictionary *data)
{
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data options:0 error:NULL];
    NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    NSLog(@"event: %@", json);
    return json.UTF8String;
}

const char* formatError(ChartboostMediationError *error)
{
    if (error == nil)
        return "";
    
    NSInteger code = error.code;
    
    NSString* localizedDescription = @"";
    if (localizedDescription != nil)
        localizedDescription = error.localizedDescription;
    
    NSString* localizedFailureReason = @"";
    if (localizedFailureReason != nil)
        localizedFailureReason = error.localizedFailureReason;
    
    NSString* localizedRecoverySuggestion = @"";
    if (localizedRecoverySuggestion != nil)
        localizedRecoverySuggestion = error.localizedRecoverySuggestion;
    
    NSString* formattedError = @"";
    
    formattedError = [formattedError stringByAppendingFormat:@"(CM_%ld) Message: %@ Cause: %@ Resolution: %@", code, localizedDescription, localizedFailureReason, localizedRecoverySuggestion];
    
    return formattedError.UTF8String;
}

const void serializeEvent(ChartboostMediationError *error, ChartboostMediationEvent event)
{
    if (event == nil)
        return;
    
    event(formatError(error));
}

const void serializePlacementWithError(NSString *placementName, ChartboostMediationError *error, ChartboostMediationPlacementEvent placementEvent)
{
    if (placementEvent == nil)
        return;
    
    placementEvent(placementName.UTF8String, formatError(error));
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
    
    placementLoadEvent(placementName.UTF8String, requestIdentifier.UTF8String, auctionId.UTF8String, partnerId.UTF8String, [price doubleValue], formatError(error));
}

static void subscribeToILRDNotifications()
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

static void subscribeToPartnerInitializationNotifications()
{
    static id partnerInitializationObserver = nil;
    
    if (partnerInitializationObserver != nil)
        [[NSNotificationCenter defaultCenter] removeObserver:partnerInitializationObserver];
    
    partnerInitializationObserver = [[NSNotificationCenter defaultCenter] addObserverForName:kHeliumDidReceiveInitResultsNotification object:nil queue:nil usingBlock:^(NSNotification * _Nonnull notification) {
        // Extract the results payload.
        NSDictionary *results = (NSDictionary *)notification.object;
        const char* jsonToUnity = serializeDictionary(results);
        if (_didReceivePartnerInitializationDataCallback != nil)
            _didReceivePartnerInitializationDataCallback(jsonToUnity);
    }];
}

@interface ChartboostMediationManager() <HeliumSdkDelegate, CHBHeliumInterstitialAdDelegate, CHBHeliumRewardedAdDelegate, CHBHeliumBannerAdDelegate>

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
    [bannerDraggers removeObjectForKey:adId];
}



- (void) enableBannerDrag:(const void*) uniqueId listener:(ChartboostMediationBannerDragEvent) dragListener
{    
    HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
            
    ChartboostMediationBannerAdDragger* dragger = [[ChartboostMediationBannerAdDragger alloc] init];
    dragger.dragListener = dragListener;
    
    // create pan gesture recognizer for bannerView
    UIPanGestureRecognizer *panGesture = [[UIPanGestureRecognizer alloc] initWithTarget:dragger action:@selector(handlePan:)];
    [bannerView addGestureRecognizer:panGesture];
    
    // add to dragger dictionary
    if(bannerDraggers == nil)
        bannerDraggers = [[NSMutableDictionary alloc] init];
        
    // associate dragger to banner object uniqueId (each bannerAd should have it's own drag listener)
    [bannerDraggers setObject:dragger forKey:[NSNumber numberWithLong:(long)uniqueId]];
}

- (void) disableBannerDrag : (const void*) uniqueId
{
    // Obtain wrapper object associated with this banner and destroy it
    ChartboostMediationBannerAdDragger* wrapper = [bannerDraggers objectForKey:[NSNumber numberWithLong:(long)uniqueId]];
    wrapper.dragListener = NULL;
    wrapper = NULL;
    
    // also remove the dictionary entry
    [bannerDraggers removeObjectForKey:[NSNumber numberWithLong:(long)uniqueId]];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark HeliumSdkDelegate

- (void)heliumDidStartWithError:(ChartboostMediationError *)error;
{
    serializeEvent(error, _didStartCallback);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
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
///////////////////////////////////////////////////////////////////////////////////////////////////
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

///////////////////////////////////////////////////////////////////////////////////////////////////
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

@end
