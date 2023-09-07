/*
* ChartboostMediationManager.mm
* Chartboost Mediation SDK iOS/Unity
*/

#import <objc/runtime.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>
#import <ChartboostMediationSDK/HeliumInitResultsEvent.h>
#import "UnityAppController.h"
#import "ChartboostMediationBannerAdWrapper.h"

// Converts C style string to NSString
#define GetStringParam(_x_) (_x_ != NULL) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil(_x_) (_x_ != NULL && strlen(_x_)) ? [NSString stringWithUTF8String:_x_] : nil

typedef void (*ChartboostMediationEvent)(const char* error);
typedef void (*ChartboostMediationILRDEvent)(const char* impressionData);
typedef void (*ChartboostMediationPartnerInitializationDataEvent)(const char* partnerInitializationData);
typedef void (*ChartboostMediationPlacementEvent)(const char* placementName, const char* error);
typedef void (*ChartboostMediationPlacementLoadEvent)(const char* placementName, const char* loadId, const char* auctionId, const char* partnerId, double price, const char* lineItemName, const char* lineItemId, const char* error);

// Fullscreen Events
typedef void (*ChartboostMediationFullscreenAdLoadResultEvent)(int hashCode, const void* adHashCode, const char *loadId, const char *winningBidJson, const char *metricsJson, const char *code, const char *message);
typedef void (*ChartboostMediationFullscreenAdShowResultEvent)(int hashCode, const char* metricsJson, const char *code, const char *message);
typedef void (*ChartboostMediationFullscreenAdEvent)(long hashCode, int eventType, const char *code, const char* message);

// Banner Events
typedef void (*ChartboostMediationBannerAdEvent)(long hashCode, int eventType);
typedef void (*ChartboostMediationBannerAdLoadResultEvent)(int hashCode, const void* adHashCode, const char *loadId, const char *metricsJson, const char *code, const char *message);


typedef void (^block)(void);

static void sendToMain(block block) {
    dispatch_async(dispatch_get_main_queue(), block);
}

template <typename TObj>
TObj objFromJsonString(const char* jsonString) {
    NSData* jsonData = [[NSString stringWithUTF8String:jsonString] dataUsingEncoding:NSUTF8StringEncoding];
    NSError* error = nil;
    TObj arr = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];

    if (error != nil)
        return nil;

    return arr;
}

static char* ConvertNSStringToCString(const NSString* nsString) {
    if (nsString == NULL) return NULL;
    const char* nsStringUtf8 = [nsString UTF8String];
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);
    return cString;
}

const char * getCStringOrNull(NSString* nsString) {
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);
    return cString;
}

const char* dictionaryToJSON(NSDictionary *data)
{
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:data options:0 error:&error];
    if (! jsonData) {
        NSLog(@"%s: error: %@", __func__, error.localizedDescription);
        return "";
     }
    NSString *json = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return getCStringOrNull(json);
}

const void serializeEvent(ChartboostMediationError *error, ChartboostMediationEvent event)
{
    if (event == nil)
        return;
    
    event(error.localizedDescription.UTF8String);
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
    
    NSString* partnerId = [winningBidInfo objectForKey:@"partner-id"] ?: @"";
    NSString* auctionId = [winningBidInfo objectForKey:@"auction-id"] ?: @"";
    NSNumber* price = [winningBidInfo objectForKey:@"price"] ?: 0;
    NSString* lineItemName = [winningBidInfo objectForKey:@"line_item_name"] ?: @"";
    NSString* lineItemId = [winningBidInfo objectForKey:@"line_item_id"] ?: @"";
    
    placementLoadEvent(placementName.UTF8String, requestIdentifier.UTF8String, auctionId.UTF8String, partnerId.UTF8String, [price doubleValue], lineItemName.UTF8String, lineItemId.UTF8String, error.localizedDescription.UTF8String);
}

static ChartboostMediationBannerView * _getBannerView(const void * uniqueId){
    ChartboostMediationBannerAdWrapper *bannerWrapper =(__bridge ChartboostMediationBannerAdWrapper*)uniqueId;
    ChartboostMediationBannerView* bannerView = bannerWrapper.bannerView;
    return bannerView;
}

static NSMutableDictionary * storedAds;
enum fullscreenEvents {RecordImpression = 0, Click = 1, Reward = 2, Close = 3, Expire = 4};
enum bannerEvents {BannerAppear = 0, BannerClick = 1, BannerRecordImpression = 2 };


@interface ChartboostMediationObserver : NSObject <HeliumSdkDelegate, ChartboostMediationFullscreenAdDelegate, ChartboostMediationBannerViewDelegate, CHBHeliumInterstitialAdDelegate, CHBHeliumRewardedAdDelegate, CHBHeliumBannerAdDelegate>

+ (instancetype) sharedObserver;

// lifecycle callbacks
@property ChartboostMediationEvent didStartCallback;
@property ChartboostMediationILRDEvent didReceiveILRDCallback;
@property ChartboostMediationPartnerInitializationDataEvent didReceivePartnerInitializationDataCallback;

// interstitial callbacks
@property ChartboostMediationPlacementLoadEvent interstitialDidLoadCallback;
@property ChartboostMediationPlacementEvent interstitialDidClickCallback;
@property ChartboostMediationPlacementEvent interstitialDidCloseCallback;
@property ChartboostMediationPlacementEvent interstitialDidShowCallback;
@property ChartboostMediationPlacementEvent interstitialDidRecordImpressionCallback;

// rewarded callbacks
@property ChartboostMediationPlacementLoadEvent rewardedDidLoadCallback;
@property ChartboostMediationPlacementEvent rewardedDidClickCallback;
@property ChartboostMediationPlacementEvent rewardedDidCloseCallback;
@property ChartboostMediationPlacementEvent rewardedDidShowCallback;
@property ChartboostMediationPlacementEvent rewardedDidRecordImpressionCallback;
@property ChartboostMediationPlacementEvent rewardedDidReceiveRewardCallback;

// banner callbacks
@property ChartboostMediationPlacementLoadEvent bannerDidLoadCallback;
@property ChartboostMediationPlacementEvent bannerDidRecordImpressionCallback;
@property ChartboostMediationPlacementEvent bannerDidClickCallback;

// fullscreen callbacks
@property ChartboostMediationFullscreenAdEvent fullscreenAdEvents;

// banner callbacks
@property ChartboostMediationBannerAdEvent bannerAdEvents;

@end

@implementation ChartboostMediationObserver

+ (instancetype) sharedObserver {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

struct Implementation {
    SEL selector;
    IMP imp;
};

- (Implementation) getImplementationFromClassNamed:(NSString*)className selectorName:(NSString*)selectorName
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
#pragma mark Public
- (void)storeAd:(id)ad placementName:(const char *)placement multiPlacementSupport:(BOOL)multiPlacementSupport{
    if (storedAds == nil)
        storedAds = [[NSMutableDictionary alloc] init];
    
    if (multiPlacementSupport)
    {
        NSNumber *key = [NSNumber numberWithLong:(long)ad];
        [storedAds setObject:ad forKey:key];
    }
    else
        [storedAds setObject:ad forKey:GetStringParam(placement)];
}

- (void)releaseAd:(NSNumber*)adId placementName:(NSString *)placementName multiPlacementSupport:(BOOL)multiPlacementSupport
{
    if (multiPlacementSupport)
        [storedAds removeObjectForKey:adId];
    else
    {
        if (placementName != nil)
            [storedAds removeObjectForKey:placementName];
    }
}

- (void)subscribeToPartnerInitializationNotifications
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

- (void)subscribeToILRDNotifications
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

- (void)serializeFullscreenEvent: (id<ChartboostMediationFullscreenAd>)ad  fullscreenEvent:(fullscreenEvents)fullscreenEvent error:(ChartboostMediationError *)error
{
    const char *code = "";
    const char *message = "";
    
    if (error != nil)
    {
        ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
        code = [[NSString stringWithFormat:@"CM_%ld", codeInt] UTF8String];
        message = [[error localizedDescription] UTF8String];
    }
    
    _fullscreenAdEvents((long)ad, (int)fullscreenEvent, code, message);
}

- (void)serializeBannerEvent: (ChartboostMediationBannerView*) ad bannerEvent:(bannerEvents)bannerEvent {
    
    _bannerAdEvents((long)ad, (int)bannerEvent);
}

- (UIViewController*) getBannerViewController: (ChartboostMediationBannerView*) bannerView size:(CGSize)size x:(float) x y:(float) y {
 
    UIViewController *unityVC = GetAppController().rootViewController;
    
    UILayoutGuide *safeGuide;
    if (@available(iOS 11.0, *))
        safeGuide = unityVC.view.safeAreaLayoutGuide;
    else
        safeGuide = unityVC.view.layoutMarginsGuide;
    
    [bannerView removeFromSuperview];
    [unityVC.view  addSubview:bannerView];
//     bannerView.translatesAutoresizingMaskIntoConstraints=NO;
    
    bannerView.frame = CGRectMake(x, y, size.width, size.height);
    
    return unityVC;
}


- (UIViewController*) getBannerViewController: (ChartboostMediationBannerView*) bannerView size:(CGSize)size screenLocation:(long) screenLocation {

    UIViewController *unityVC = GetAppController().rootViewController;
    
    UILayoutGuide *safeGuide;
    if (@available(iOS 11.0, *))
        safeGuide = unityVC.view.safeAreaLayoutGuide;
    else
        safeGuide = unityVC.view.layoutMarginsGuide;
    
    [bannerView removeFromSuperview];
    [unityVC.view  addSubview:bannerView];
    bannerView.translatesAutoresizingMaskIntoConstraints=NO;
    NSLayoutConstraint *xConstraint;

    switch (screenLocation) // X Constraints
    {
        case 1: // Top Center
        case 3: // Center
        case 5: // Bottom Center
            xConstraint = [bannerView.centerXAnchor constraintEqualToAnchor:safeGuide.centerXAnchor];
            break;
        case 2: // Top Right
        case 6: // Bottom Right
            xConstraint = [bannerView.trailingAnchor constraintEqualToAnchor:safeGuide.trailingAnchor];
            break;
        default:
            xConstraint = [bannerView.leadingAnchor constraintEqualToAnchor:safeGuide.leadingAnchor];
    }

    NSLayoutConstraint *yConstraint;
    switch (screenLocation) // Y Constraints
    {
        case 0: // Top Left:
        case 1: // Top Center
        case 2: // Top
            yConstraint = [bannerView.topAnchor constraintEqualToAnchor:safeGuide.topAnchor];
            break;;
        case 4: // Bottom Left
        case 5: // Bottom Center
        case 6: // Bottom Right
            yConstraint = [bannerView.bottomAnchor constraintEqualToAnchor:safeGuide.bottomAnchor];
            break;
        default:
            yConstraint = [bannerView.centerYAnchor constraintEqualToAnchor:safeGuide.centerYAnchor];
    }

    [NSLayoutConstraint activateConstraints:@[
        [bannerView.widthAnchor constraintEqualToConstant:size.width],
        [bannerView.heightAnchor constraintEqualToConstant:size.height],
        xConstraint,
        yConstraint
    ]];
    
    return unityVC;
}


#pragma mark HeliumSdkDelegate
- (void)heliumDidStartWithError:(ChartboostMediationError *)error;
{
    [[ChartboostMediationObserver sharedObserver] subscribeToILRDNotifications];
    [[ChartboostMediationObserver sharedObserver] subscribeToPartnerInitializationNotifications];
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
    if (!error)
        UnityPause(true);
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
    if (!error)
        UnityPause(true);
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
    [self serializeFullscreenEvent:ad fullscreenEvent:RecordImpression error:nil];
}

- (void)didClickWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    [self serializeFullscreenEvent:ad fullscreenEvent:Click error:nil];
}

- (void)didRewardWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    [self serializeFullscreenEvent:ad fullscreenEvent:Reward error:nil];
}

- (void)didCloseWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad error:(ChartboostMediationError * _Nullable)error {
    UnityPause(false);
    [self serializeFullscreenEvent:ad fullscreenEvent:Close error:error];
}

- (void)didExpireWithAd:(id <ChartboostMediationFullscreenAd> _Nonnull)ad {
    [self serializeFullscreenEvent:ad fullscreenEvent:Expire error:nil];
}


#pragma mark ChartboostMediationBannerAdDelegate
- (void)willAppearWithBannerView:(ChartboostMediationBannerView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:BannerAppear];
}

- (void)didClickWithBannerView:(ChartboostMediationBannerView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:BannerClick];
}

- (void)didRecordImpressionWithBannerView:(ChartboostMediationBannerView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:BannerRecordImpression];
}

@end

#pragma mark Bridge Functions
extern "C"
{

void _setLifeCycleCallbacks(ChartboostMediationEvent didStartCallback, ChartboostMediationILRDEvent didReceiveILRDCallback, ChartboostMediationPartnerInitializationDataEvent didReceivePartnerInitializationDataCallback)
{
    [[ChartboostMediationObserver sharedObserver] setDidStartCallback:didStartCallback];
    [[ChartboostMediationObserver sharedObserver] setDidReceiveILRDCallback:didReceiveILRDCallback];
    [[ChartboostMediationObserver sharedObserver] setDidReceivePartnerInitializationDataCallback:didReceivePartnerInitializationDataCallback];
}

void _setInterstitialCallbacks(ChartboostMediationPlacementLoadEvent didLoadCallback, ChartboostMediationPlacementEvent didShowCallback, ChartboostMediationPlacementEvent didCloseCallback, ChartboostMediationPlacementEvent didClickCallback, ChartboostMediationPlacementEvent didRecordImpression)
{
    [[ChartboostMediationObserver sharedObserver] setInterstitialDidLoadCallback:didLoadCallback];
    [[ChartboostMediationObserver sharedObserver] setInterstitialDidShowCallback:didShowCallback];
    [[ChartboostMediationObserver sharedObserver] setInterstitialDidCloseCallback:didCloseCallback];
    [[ChartboostMediationObserver sharedObserver] setInterstitialDidClickCallback:didClickCallback];
    [[ChartboostMediationObserver sharedObserver] setInterstitialDidRecordImpressionCallback:didRecordImpression];
}

void _setRewardedCallbacks(ChartboostMediationPlacementLoadEvent didLoadCallback, ChartboostMediationPlacementEvent didShowCallback, ChartboostMediationPlacementEvent didCloseCallback, ChartboostMediationPlacementEvent didClickCallback, ChartboostMediationPlacementEvent didRecordImpression, ChartboostMediationPlacementEvent didReceiveRewardCallback)
{
    [[ChartboostMediationObserver sharedObserver] setRewardedDidLoadCallback:didLoadCallback];
    [[ChartboostMediationObserver sharedObserver] setRewardedDidShowCallback:didShowCallback];
    [[ChartboostMediationObserver sharedObserver] setRewardedDidCloseCallback:didCloseCallback];
    [[ChartboostMediationObserver sharedObserver] setRewardedDidClickCallback:didClickCallback];
    [[ChartboostMediationObserver sharedObserver] setRewardedDidRecordImpressionCallback:didRecordImpression];
    [[ChartboostMediationObserver sharedObserver] setRewardedDidReceiveRewardCallback:didReceiveRewardCallback];
}

void _setBannerCallbacks(ChartboostMediationPlacementLoadEvent didLoadCallback, ChartboostMediationPlacementEvent didRecordImpression, ChartboostMediationPlacementEvent didClickCallback)
{
    
    [[ChartboostMediationObserver sharedObserver] setBannerDidLoadCallback:didLoadCallback];
    [[ChartboostMediationObserver sharedObserver] setBannerDidRecordImpressionCallback:didRecordImpression];
    [[ChartboostMediationObserver sharedObserver] setBannerDidClickCallback:didClickCallback];
}

void _setFullscreenCallbacks(ChartboostMediationFullscreenAdEvent fullscreenAdEvents){
    [[ChartboostMediationObserver sharedObserver] setFullscreenAdEvents:fullscreenAdEvents];
}

void _setBannerAdCallbacks(ChartboostMediationBannerAdEvent bannerEvents){
    [[ChartboostMediationObserver sharedObserver] setBannerAdEvents:bannerEvents];
}

void _chartboostMediationInit(const char *appId, const char *appSignature, const char *unityVersion, const char** initializationOptions, int initializationOptionsSize)
{
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
    
    [[Helium sharedHelium] startWithAppId:GetStringParam(appId) andAppSignature:GetStringParam(appSignature) options:heliumInitializationOptions delegate:[ChartboostMediationObserver sharedObserver]];
}

void _chartboostMediationSetSubjectToCoppa(BOOL isSubject)
{
    [[Helium sharedHelium] setSubjectToCoppa:isSubject];
}

void _chartboostMediationSetSubjectToGDPR(BOOL isSubject)
{
    [[Helium sharedHelium] setSubjectToGDPR:isSubject];
}

void _chartboostMediationSetUserHasGivenConsent(BOOL hasGivenConsent)
{
    [[Helium sharedHelium] setUserHasGivenConsent:hasGivenConsent];
}

void _chartboostMediationSetCCPAConsent(BOOL hasGivenConsent)
{
    [[Helium sharedHelium] setCCPAConsent:hasGivenConsent];
}

void _chartboostMediationSetUserIdentifier(const char * userIdentifier)
{
    [[Helium sharedHelium] setUserIdentifier:GetStringParam(userIdentifier)];
}

char * _chartboostMediationGetUserIdentifier()
{
    return ConvertNSStringToCString([[Helium sharedHelium] userIdentifier]);
}

float _chartboostMediationGetUIScaleFactor() {
    // TODO: https://github.com/ChartBoost/ios-helium-sdk/pull/1314
    return UIScreen.mainScreen.scale;
}

void _chartboostMediationSetTestMode(BOOL isTestModeEnabled)
{
    Implementation implementation = [[ChartboostMediationObserver sharedObserver] getImplementationFromClassNamed:@"CHBHTestModeHelper" selectorName:@"setIsTestModeEnabled_isForcedOn:"];
    typedef void (*Signature)(id, SEL, BOOL);
    Signature function = (Signature)implementation.imp;
    function([ChartboostMediationObserver sharedObserver], implementation.selector, isTestModeEnabled);
}

void * _chartboostMediationGetInterstitialAd(const char *placementName)
{
    id<HeliumInterstitialAd> ad = [[Helium sharedHelium] interstitialAdProviderWithDelegate: [ChartboostMediationObserver sharedObserver] andPlacementName: GetStringParam(placementName)];
    if (ad == NULL)
        return NULL;
    
    [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:false];
    return (__bridge void*)ad;
}

BOOL _chartboostMediationInterstitialSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    if (ad.keywords == nil)
        ad.keywords = [[HeliumKeywords alloc] init];
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

char * _chartboostMediationInterstitialRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

void _chartboostMediationInterstitialAdLoad(const void * uniqueId)
{
    sendToMain(^{
        id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
        [ad loadAd];
    });
}

void _chartboostMediationInterstitialClearLoaded(const void * uniqueId)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    [ad clearLoadedAd];
}

void _chartboostMediationInterstitialAdShow(const void * uniqueId)
{
    sendToMain(^{
        id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
        [ad showAdWithViewController: UnityGetGLViewController()];
    });
}

BOOL _chartboostMediationInterstitialAdReadyToShow(const void * uniqueId)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return [ad readyToShow];
}

void * _chartboostMediationGetRewardedAd(const char *placementName)
{
    id<HeliumRewardedAd> ad = [[Helium sharedHelium] rewardedAdProviderWithDelegate: [ChartboostMediationObserver sharedObserver] andPlacementName: GetStringParam(placementName)];
    if (ad == NULL)
        return NULL;
    
    [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:false];
    return (__bridge void*)ad;
}

BOOL _chartboostMediationRewardedSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    if (ad.keywords == nil)
        ad.keywords = [[HeliumKeywords alloc] init];
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

char * _chartboostMediationRewardedRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

void _chartboostMediationRewardedAdLoad(const void * uniqueId)
{
    sendToMain(^{
        id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
        [ad loadAd];
    });
}

void _chartboostMediationRewardedClearLoaded(const void * uniqueId)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    [ad clearLoadedAd];
}

void _chartboostMediationRewardedAdShow(const void * uniqueId)
{
    sendToMain(^{
        id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
        [ad showAdWithViewController: UnityGetGLViewController()];
    });
}

BOOL _chartboostMediationRewardedAdReadyToShow(const void * uniqueId)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    return [ad readyToShow];
}

void _chartboostMediationRewardedAdSetCustomData(const void * uniqueId, const char * customData)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    ad.customData = GetStringParam(customData);
}

void _chartboostMediationLoadFullscreenAd(const char *placementName, const char *keywords, int hashCode, ChartboostMediationFullscreenAdLoadResultEvent callback)
{
    NSDictionary *formattedKeywords = objFromJsonString<NSDictionary *>(keywords);
    
    ChartboostMediationAdLoadRequest *loadRequest = [[ChartboostMediationAdLoadRequest alloc] initWithPlacement:GetStringParam(placementName) keywords:formattedKeywords];
    
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
        [ad setDelegate:[ChartboostMediationObserver sharedObserver]];
        [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:true];
        const char *loadId = [[adLoadResult loadID] UTF8String];
        const char *winningBidJson = dictionaryToJSON([ad winningBidInfo]);
        const char *metricsJson = dictionaryToJSON([adLoadResult metrics]);
        callback(hashCode, (__bridge void*)ad, loadId, winningBidJson, metricsJson, "", "");
    }];
}

void _chartboostMediationShowFullscreenAd(const void *uniqueId, int hashCode, ChartboostMediationFullscreenAdShowResultEvent callback)
{
    sendToMain(^{
        id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
        [ad showWith:UnityGetGLViewController() completion:^(ChartboostMediationAdShowResult *adShowResult) {
            ChartboostMediationError *error = [adShowResult error];
            if (error != nil)
            {
                ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
                const char *code = [[NSString stringWithFormat:@"CM_%ld", codeInt] UTF8String];
                const char *message = [[error localizedDescription] UTF8String];
                callback(hashCode, "", code, message);
                return;
            }
        
            const char *metricsJson = dictionaryToJSON([adShowResult metrics]);
            UnityPause(true);
            callback(hashCode, metricsJson, "", "");
        }];
    });
}

void _chartboostMediationInvalidateFullscreenAd(const void *uniqueId)
{
    sendToMain(^() {
        id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
        [ad invalidate];
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:nil multiPlacementSupport:true];
    });
}

void _chartboostMediationFullscreenSetCustomData(const void *uniqueId, const char *customData)
{
    sendToMain(^() {
        id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
        [ad setCustomData:GetStringParam(customData)];
    });
}


__deprecated
void * _chartboostMediationGetBannerAd(const char *placementName, long size)
{
    CHBHBannerSize cbSize;
    switch (size) {
        case 2:
            cbSize = CHBHBannerSize_Leaderboard;
            break;
        case 1:
            cbSize = CHBHBannerSize_Medium;
            break;
            
        default:
            cbSize = CHBHBannerSize_Standard;
            break;
    }
    
    HeliumBannerView *ad = [[Helium sharedHelium] bannerProviderWithDelegate:[ChartboostMediationObserver sharedObserver] andPlacementName:GetStringParam(placementName) andSize:cbSize];

    if (ad == NULL)
        return NULL;
        
    [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:true];
    return (__bridge void*)ad;
}

__deprecated
BOOL _chartboostMediationBannerSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    sendToMain(^{
        if (ad.keywords == nil)
            ad.keywords = [[HeliumKeywords alloc] init];
    });
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

__deprecated
char * _chartboostMediationBannerRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

__deprecated
void _chartboostMediationBannerAdLoad(const void * uniqueId, long screenLocation)
{
    sendToMain(^{
        //     TopLeft = 0,
        //     TopCenter = 1,
        //     TopRight = 2,
        //     Center = 3,
        //     BottomLeft = 4,
        //     BottomCenter = 5,
        //     BottomRight = 6
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        //TODO handle the null case
        UIViewController *unityVC = GetAppController().rootViewController;
        UILayoutGuide *safeGuide;
        if (@available(iOS 11.0, *))
            safeGuide = unityVC.view.safeAreaLayoutGuide;
        else
            safeGuide = unityVC.view.layoutMarginsGuide;
        bannerView.translatesAutoresizingMaskIntoConstraints=NO;
        [bannerView removeFromSuperview];
        [unityVC.view  addSubview:bannerView];
        NSLayoutConstraint *xConstraint;

        switch (screenLocation) // X Constraints
        {
            case 1: // Top Center
            case 3: // Center
            case 5: // Bottom Center
                xConstraint = [bannerView.centerXAnchor constraintEqualToAnchor:safeGuide.centerXAnchor];
                break;
            case 2: // Top Right
            case 6: // Bottom Right
                xConstraint = [bannerView.trailingAnchor constraintEqualToAnchor:safeGuide.trailingAnchor];
                break;
            default:
                xConstraint = [bannerView.leadingAnchor constraintEqualToAnchor:safeGuide.leadingAnchor];
        }

        NSLayoutConstraint *yConstraint;
        switch (screenLocation) // Y Constraints
        {
            case 0: // Top Left:
            case 1: // Top Center
            case 2: // Top
                yConstraint = [bannerView.topAnchor constraintEqualToAnchor:safeGuide.topAnchor];
                break;;
            case 4: // Bottom Left
            case 5: // Bottom Center
            case 6: // Bottom Right
                yConstraint = [bannerView.bottomAnchor constraintEqualToAnchor:safeGuide.bottomAnchor];
                break;
            default:
                yConstraint = [bannerView.centerYAnchor constraintEqualToAnchor:safeGuide.centerYAnchor];
        }

        [NSLayoutConstraint activateConstraints:@[
            [bannerView.widthAnchor constraintEqualToConstant:bannerView.frame.size.width],
            [bannerView.heightAnchor constraintEqualToConstant:bannerView.frame.size.height],
            xConstraint,
            yConstraint
        ]];

        id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
        [ad loadAdWithViewController:unityVC];
    });
}

__deprecated
void _chartboostMediationBannerClearLoaded(const void * uniqueId)
{
    sendToMain(^(){
        id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
        [ad clearAd];
    });
}

__deprecated
void _chartboostMediationBannerRemove(const void * uniqueId)
{
    sendToMain(^(){
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView removeFromSuperview];
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:nil multiPlacementSupport:true];
    });
}

__deprecated
void _chartboostMediationBannerSetVisibility(const void * uniqueId, BOOL isVisible)
{
    sendToMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView setHidden:!isVisible];
    });
}

__deprecated
void _chartboostMediationFreeAdObject(const void * uniqueId, const char * placementName, bool multiPlacementSupport)
{
    sendToMain(^(){
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:GetStringParam(placementName) multiPlacementSupport:multiPlacementSupport];
    });
}


const void* _chartboostMediationLoadBannerView(ChartboostMediationBannerAdDragEvent dragListener){
        
    ChartboostMediationBannerView *bannerView = [[ChartboostMediationBannerView alloc] init];
    [bannerView setDelegate:[ChartboostMediationObserver sharedObserver]];
    
    ChartboostMediationBannerAdWrapper *wrapper = [[ChartboostMediationBannerAdWrapper alloc] initWithBannerView:bannerView andDragListener:dragListener];
    [[ChartboostMediationObserver sharedObserver] storeAd:wrapper placementName:nil multiPlacementSupport:true];
    
    return (__bridge void*)wrapper;
}

void _chartboostMediationBannerViewLoadAdWithScreenPos(const void *uniqueId, const char *placementName, const char* sizeName, float width, float height, long screenLocation, int hashCode, ChartboostMediationBannerAdLoadResultEvent callback) {
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);

    ChartboostMediationBannerSize *size;
    NSArray *sizeNames = @[@"ADAPTIVE", @"STANDARD", @"MEDIUM", @"LEADERBOARD"];
    long item = [sizeNames indexOfObject:GetStringParam(sizeName)];
    switch(item){
        case 0 : size = [ChartboostMediationBannerSize adaptiveWithWidth:width maxHeight:height]; break;
        case 1 : size = [ChartboostMediationBannerSize standard]; break;
        case 2 : size = [ChartboostMediationBannerSize medium]; break;
        case 3 : size = [ChartboostMediationBannerSize leaderboard]; break;
    }
    
    ChartboostMediationBannerLoadRequest *loadRequest = [[ChartboostMediationBannerLoadRequest alloc] initWithPlacement:GetStringParam(placementName) size:size];
    UIViewController* viewController = [[ChartboostMediationObserver sharedObserver] getBannerViewController:bannerView size:size.size screenLocation:screenLocation];
    
    // Load
    [bannerView loadWith:loadRequest viewController:viewController completion:^(ChartboostMediationBannerLoadResult *adLoadResult) {
        ChartboostMediationError *error = [adLoadResult error];
        if (error != nil)
        {
            ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
            const char *code = [[NSString stringWithFormat:@"CM_%ld", codeInt] UTF8String];
            const char *message = [[error localizedDescription] UTF8String];
            callback(hashCode, uniqueId, "", "", code, message);
            return;
        }
        
        const char *loadId = [[adLoadResult loadID] UTF8String];
        const char *metricsJson = dictionaryToJSON([adLoadResult metrics]);
        callback(hashCode, uniqueId, loadId, metricsJson, "", "");
    }];    
}

void _chartboostMediationBannerViewLoadAdWithXY(const void *uniqueId, const char *placementName, const char* sizeName, float width, float height, float x, float y, int hashCode, ChartboostMediationBannerAdLoadResultEvent callback) {
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);

    ChartboostMediationBannerSize *size;
    NSArray *sizeNames = @[@"ADAPTIVE", @"STANDARD", @"MEDIUM", @"LEADERBOARD"];
    long item = [sizeNames indexOfObject:GetStringParam(sizeName)];
    switch(item){
        case 0 : size = [ChartboostMediationBannerSize adaptiveWithWidth:width maxHeight:height]; break;
        case 1 : size = [ChartboostMediationBannerSize standard]; break;
        case 2 : size = [ChartboostMediationBannerSize medium]; break;
        case 3 : size = [ChartboostMediationBannerSize leaderboard]; break;
    }
    
    ChartboostMediationBannerLoadRequest *loadRequest = [[ChartboostMediationBannerLoadRequest alloc] initWithPlacement:GetStringParam(placementName) size:size];
    UIViewController* viewController = [[ChartboostMediationObserver sharedObserver] getBannerViewController:bannerView size:size.size x:x y:y];
    
    // Load
    [bannerView loadWith:loadRequest viewController:viewController completion:^(ChartboostMediationBannerLoadResult *adLoadResult) {
        ChartboostMediationError *error = [adLoadResult error];
        if (error != nil)
        {
            ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
            const char *code = [[NSString stringWithFormat:@"CM_%ld", codeInt] UTF8String];
            const char *message = [[error localizedDescription] UTF8String];
            callback(hashCode, uniqueId, "", "", code, message);
            return;
        }
        
        const char *loadId = [[adLoadResult loadID] UTF8String];
        const char *metricsJson = dictionaryToJSON([adLoadResult metrics]);
        callback(hashCode, uniqueId, loadId, metricsJson, "", "");
    }];    
}

void _chartboostMediationBannerViewSetKeywords(const void* uniqueId, const char * keywords){
    ChartboostMediationBannerView *ad = _getBannerView(uniqueId);
    
    
    NSMutableDictionary *formattedKeywords = objFromJsonString<NSMutableDictionary *>(keywords);
//    NSMutableDictionary *adKeywords = ad.keywords == nil ? [[NSMutableDictionary alloc] init] : [ad.keywords mutableCopy];
    ad.keywords = formattedKeywords;
        
    NSLog(@"ad.keywords count : %lu", ad.keywords.count);
    NSLog(@"keys : %@", [ad.keywords allKeys]);
}

const char * _chartboostMediationBannerViewGetSize(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);

    NSString * aspectRatioKey =@"aspectRatio"; NSString * aspectRatioValue = [NSString stringWithFormat:@"%f", bannerView.size.aspectRatio];
    NSString * widthKey = @"width"; NSString * widthValue = [NSString stringWithFormat:@"%f", bannerView.size.size.width];
    NSString * heightKey = @"height"; NSString * heightValue = [NSString stringWithFormat:@"%f", bannerView.size.size.height];
    NSString * typeKey = @"type"; NSString * typeValue = [NSString stringWithFormat:@"%d", (int)bannerView.size.type];
    
    NSString *nameKey = @"name";
    NSString *nameValue = @"";
    if(bannerView.size.type == 0) {  // Fixed
        int width = bannerView.size.size.width;
        switch (width) {
            case 320: nameValue = @"STANDARD"; break;
            case 300: nameValue = @"MEDIUM"; break;
            case 728: nameValue = @"LEADERBOARD"; break;
            default:break;
        }
    }
    else{
        nameValue = @"ADAPTIVE";
    }
    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:nameValue,nameKey,aspectRatioValue,aspectRatioKey,widthValue,widthKey,heightValue,heightKey,typeValue, typeKey, nil];

    return dictionaryToJSON(dict);
}

const char * _chartboostMediationBannerViewGetWinningBidInfo(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    return dictionaryToJSON(bannerView.winningBidInfo);
}

const char * _chartboostMediationBannerViewGetLoadMetrics(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    return dictionaryToJSON(bannerView.loadMetrics);
}

void _chartboostMediationBannerViewSetHorizontalAlignment(const void* uniqueId, int horizontalAlignment){
            
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    bannerView.horizontalAlignment = (ChartboostMediationBannerHorizontalAlignment)horizontalAlignment;
}

int _chartboostMediationBannerViewGetHorizontalAlignment(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    return (int)bannerView.horizontalAlignment;
}

void _chartboostMediationBannerViewSetVerticalAlignment(const void* uniqueId, int verticalAlignment){
            
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    bannerView.verticalAlignment = (ChartboostMediationBannerVerticalAlignment)verticalAlignment;
}

int _chartboostMediationBannerViewGetVerticalAlignment(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    return (int)bannerView.verticalAlignment;
}

void _chartboostMediationBannerViewSetDraggability(const void* uniqueId, BOOL canDrag){
    ChartboostMediationBannerAdWrapper *bannerWrapper = (__bridge ChartboostMediationBannerAdWrapper *)uniqueId;
    [bannerWrapper setDraggable:canDrag];        
}

void _chartboostMediationBannerViewSetVisibility(const void* uniqueId, BOOL visible){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    [bannerView setHidden:!visible];
}

void _chartboostMediationBannerViewReset(const void* uniqueId){
    ChartboostMediationBannerView *bannerView =(__bridge ChartboostMediationBannerView*)uniqueId;
    [bannerView reset];
}

void _chartboostMediationBannerViewDestroy(const void * uniqueId)
{
    sendToMain(^(){
        ChartboostMediationBannerView* bannerView = _getBannerView(uniqueId);
        [bannerView removeFromSuperview];
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:nil multiPlacementSupport:true];
    });
}



}
