/*
* ChartboostMediationManager.mm
* Chartboost Mediation SDK iOS/Unity
*/

#import <objc/runtime.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>
#import <ChartboostMediationSDK/HeliumInitResultsEvent.h>
#import "UnityAppController.h"
#import "ChartboostMediationBannerAdWrapper.h"
#import "ChartboostUnityUtilities.h"

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
typedef void (*ChartboostMediationBannerAdLoadResultEvent)(int hashCode, const void* adHashCode, const char *loadId, const char *metricsJson, float width, float height, const char *code, const char *message);

// Fullscreen Queue Events
typedef void (*ChartboostMediationFullscreenAdQueueUpdateEvent)(long hashCode, const char * loadId, const char * metricsJson, const char * code, const char * message, int numberOfAdsReady);
typedef void (*ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent)(long hashCode, int numberOfAdsReady);

const char * sizeToJSON(ChartboostMediationBannerSize* size)
{
    NSString * aspectRatioKey =@"aspectRatio"; NSString * aspectRatioValue = [NSString stringWithFormat:@"%f", size.aspectRatio];
    NSString * widthKey = @"width"; NSString * widthValue = [NSString stringWithFormat:@"%f", size.size.width];
    NSString * heightKey = @"height"; NSString * heightValue = [NSString stringWithFormat:@"%f", size.size.height];
    NSString * typeKey = @"type"; NSString * typeValue = [NSString stringWithFormat:@"%d", (int)size.type];
    
    NSString *sizeTypeKey = @"sizeType";
    NSString *sizeTypeValue = @"";
    if(size.type == 0) {  // Fixed
        int width = size.size.width;
        switch (width) {
            case 320: sizeTypeValue = [NSString stringWithFormat:@"%d", 0]; break;  // Standard
            case 300: sizeTypeValue = [NSString stringWithFormat:@"%d", 1]; break;  // Medium
            case 728: sizeTypeValue = [NSString stringWithFormat:@"%d", 2]; break;  // Leaderboard
            default: sizeTypeValue = [NSString stringWithFormat:@"%d", -1];break;   // Unknown
        }
    }
    else{
        sizeTypeValue = [NSString stringWithFormat:@"%d", 3];   // Adaptive
    }
    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:sizeTypeValue,sizeTypeKey,aspectRatioValue,aspectRatioKey,widthValue,widthKey,heightValue,heightKey,typeValue, typeKey, nil];

    return toJSON(dict);
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

const void serializeEvent(ChartboostMediationError *error, ChartboostMediationEvent event)
{
    if (event == nil)
        return;
    
    event(toCStringOrNull(error.localizedDescription));
}

const void serializePlacementWithError(NSString *placementName, ChartboostMediationError *error, ChartboostMediationPlacementEvent placementEvent)
{
    if (placementEvent == nil)
        return;
    
    placementEvent(toCStringOrNull(placementName), toCStringOrNull(error.localizedDescription));
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
    
    placementLoadEvent(toCStringOrNull(placementName), toCStringOrNull(requestIdentifier), toCStringOrNull(auctionId), toCStringOrNull(partnerId), [price doubleValue], toCStringOrNull(lineItemName), toCStringOrNull(lineItemId), toCStringOrNull([error localizedDescription]));
}

static ChartboostMediationBannerView * _getBannerView(const void * uniqueId){
    ChartboostMediationBannerAdWrapper *bannerWrapper =(__bridge ChartboostMediationBannerAdWrapper*)uniqueId;
    ChartboostMediationBannerView* bannerView = bannerWrapper.bannerView;
    return bannerView;
}

static NSMutableDictionary * storedAds;
static NSMutableDictionary * storedWrappers;
enum fullscreenEvents {RecordImpression = 0, Click = 1, Reward = 2, Close = 3, Expire = 4};
enum bannerEvents {BannerAppear = 0, BannerClick = 1, BannerRecordImpression = 2 };
enum fullscreenQueueEvents { QueueUpdate = 0, QueueRemoveExpiredAd = 1 };


@interface ChartboostMediationObserver : NSObject <HeliumSdkDelegate, ChartboostMediationFullscreenAdDelegate, ChartboostMediationBannerViewDelegate, CHBHeliumInterstitialAdDelegate, CHBHeliumRewardedAdDelegate, CHBHeliumBannerAdDelegate, ChartboostMediationFullscreenAdQueueDelegate>

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

// fullscreen queue callbacks
@property ChartboostMediationFullscreenAdQueueUpdateEvent fullscreenAdQueueUpdateEvent;
@property ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent fullscreenAdQueueRemoveExpiredAdEvent;

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
        [storedAds setObject:ad forKey:toNSStringOrEmpty(placement)];
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

- (void)storeWrapper:(id) wrapper forAd:(id)ad{
    if(storedWrappers == nil) {
        storedWrappers = [[NSMutableDictionary alloc] init];
    }
    NSNumber *key = [NSNumber numberWithLong:(long)ad];
    [storedWrappers setObject:wrapper forKey:key];
}

- (void)releaseWrapperForAd:(NSNumber*)ad {
    [storedWrappers removeObjectForKey:ad];
}

- (void)subscribeToPartnerInitializationNotifications
{
    static id partnerInitializationObserver = nil;

    if (partnerInitializationObserver != nil)
        [[NSNotificationCenter defaultCenter] removeObserver:partnerInitializationObserver];

    partnerInitializationObserver = [[NSNotificationCenter defaultCenter] addObserverForName:NSNotification.heliumDidReceiveInitResults object:nil queue:nil usingBlock:^(NSNotification * _Nonnull notification) {
        // Extract the results payload.
        NSDictionary *results = (NSDictionary *)notification.object;
        const char* jsonToUnity = toJSON(results);

        if (self->_didReceivePartnerInitializationDataCallback != nil)
            self->_didReceivePartnerInitializationDataCallback(jsonToUnity);
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
        const char* jsonToUnity = toJSON(data);

        if (self ->_didReceiveILRDCallback != nil)
            self ->_didReceiveILRDCallback(jsonToUnity);
    }];
}

- (void)serializeFullscreenEvent: (id<ChartboostMediationFullscreenAd>)ad  fullscreenEvent:(fullscreenEvents)fullscreenEvent error:(ChartboostMediationError *)error
{
    const char *code = "";
    const char *message = "";
    
    if (error != nil)
    {
        ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
        code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
        message = toCStringOrNull([error localizedDescription]);
    }
    
    _fullscreenAdEvents((long)ad, (int)fullscreenEvent, code, message);
}

- (void)serializeBannerEvent: (ChartboostMediationBannerView*) ad bannerEvent:(bannerEvents)bannerEvent {
    
    NSNumber *key = [NSNumber numberWithLong:(long)ad];
    ChartboostMediationBannerAdWrapper *wrapper = [storedWrappers objectForKey:key];
    _bannerAdEvents((long)wrapper, (int)bannerEvent);
}

- (void) serializeFullscreenAdQueueUpdateEvent: (ChartboostMediationFullscreenAdQueue *) queue adLoadResult:(ChartboostMediationFullscreenAdLoadResult *)adLoadResult numberOfAdsReady:(int)numberOfAdsReady {
    
    ChartboostMediationError *error = [adLoadResult error];
    if (error != nil)
    {
        ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
        const char *code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
        const char *message = toCStringOrNull([error localizedDescription]);
        _fullscreenAdQueueUpdateEvent((long)queue, "", "", code, message, numberOfAdsReady);
        return;
    }
    
    const char *loadId = toCStringOrNull([adLoadResult loadID]);
    const char *metricsJson = toJSON([adLoadResult metrics]);
    _fullscreenAdQueueUpdateEvent((long)queue, loadId, metricsJson, "", "", numberOfAdsReady);
}

- (void) serializeFullscreenAdQueueRemoveExpiredAdEvent: (ChartboostMediationFullscreenAdQueue *) queue numberOfAdsReady:(int)numberOfAdsReady
{
    _fullscreenAdQueueRemoveExpiredAdEvent((long)queue, numberOfAdsReady);
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
    // run on next iteration of the run loop
    toMain(^{
        [self serializeBannerEvent:bannerView bannerEvent:BannerAppear];
    });
}

- (void)didClickWithBannerView:(ChartboostMediationBannerView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:BannerClick];
}

- (void)didRecordImpressionWithBannerView:(ChartboostMediationBannerView *)bannerView {
    [self serializeBannerEvent:bannerView bannerEvent:BannerRecordImpression];
}

#pragma mark FullscreenAdQueue
- (void)fullscreenAdQueue:(ChartboostMediationFullscreenAdQueue *)adQueue didFinishLoadingWithResult:(ChartboostMediationAdLoadResult *)didFinishLoadingWithResult numberOfAdsReady:(NSInteger)numberOfAdsReady {
    [self serializeFullscreenAdQueueUpdateEvent:adQueue adLoadResult:didFinishLoadingWithResult numberOfAdsReady:numberOfAdsReady];
}

- (void)fullscreenAdQueueDidRemoveExpiredAd:(ChartboostMediationFullscreenAdQueue *)adQueue numberOfAdsReady:(NSInteger)numberOfAdsReady {
    [self serializeFullscreenAdQueueRemoveExpiredAdEvent:adQueue numberOfAdsReady:numberOfAdsReady];
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

void _setFullscreenAdQueueCallbacks(ChartboostMediationFullscreenAdQueueUpdateEvent updateEvent, ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent removeExpiredAdEvent){
    [[ChartboostMediationObserver sharedObserver] setFullscreenAdQueueUpdateEvent:updateEvent];
    [[ChartboostMediationObserver sharedObserver] setFullscreenAdQueueRemoveExpiredAdEvent:removeExpiredAdEvent];
}

void _chartboostMediationInit(const char *appId, const char *appSignature, const char *unityVersion, const char** initializationOptions, int initializationOptionsSize)
{
    HeliumInitializationOptions* heliumInitializationOptions = nil;

    if (initializationOptionsSize > 0) {
        NSMutableArray *initializationPartners = [NSMutableArray new];
        for (int x = 0; x < initializationOptionsSize; x++)
        {
            if(strlen(initializationOptions[x]) > 0)
                [initializationPartners addObject:toNSStringOrNull(initializationOptions[x])];
        }
        heliumInitializationOptions = [[HeliumInitializationOptions alloc] initWithSkippedPartnerIdentifiers:initializationPartners];
    }

    [[Helium sharedHelium] startWithAppId:toNSStringOrEmpty(appId) andAppSignature:toNSStringOrEmpty(appSignature) options:heliumInitializationOptions delegate:[ChartboostMediationObserver sharedObserver]];
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
    [[Helium sharedHelium] setUserIdentifier:toNSStringOrEmpty(userIdentifier)];
}

const char * _chartboostMediationGetUserIdentifier()
{
    return toCStringOrNull([[Helium sharedHelium] userIdentifier]);
}

float _chartboostMediationGetUIScaleFactor() {
    // `UIScreen.main` was deprecated in iOS 16. Apple doc:
    //   https://developer.apple.com/documentation/uikit/uiscreen/1617815-main
    // Since `UIScreen.main` has been working correctly at least up to iOS 16, the custom
    // implementation only targets iOS 17+, not iOS 13+.
    if (@available(iOS 17.0, *)) {
        NSSet<UIScene*> *connectedScenes = UIApplication.sharedApplication.connectedScenes;
        NSArray *activationStates = @[@(UISceneActivationStateForegroundActive), @(UISceneActivationStateForegroundInactive), @(UISceneActivationStateBackground), @(UISceneActivationStateUnattached)];
        for (NSNumber *activationState in activationStates) {
            UISceneActivationState state = (UISceneActivationState)activationState.integerValue;
            for (UIScene* connectedScene in connectedScenes) {
                UIWindowScene *windowScene = (UIWindowScene*)connectedScene;
                if (windowScene) {
                    if (windowScene.activationState == state) {
                        return windowScene.screen.scale;
                    }
                }
            }
        }
    }

    // fallback
    return UIScreen.mainScreen.scale;
}

void _chartboostMediationSetTestMode(BOOL isTestModeEnabled)
{
    Helium.isTestModeEnabled = isTestModeEnabled;    
}

void _chartboostMediationDiscardOversizedAds(BOOL shouldDiscard)
{
    [[Helium sharedHelium] setDiscardOversizedAds:shouldDiscard];
}

const char * _chartboostMediationAdaptersInfo()
{
    NSMutableArray * jsonArray = [NSMutableArray array];

    NSArray<HeliumAdapterInfo*> * adapters =  [[Helium sharedHelium] initializedAdapterInfo];
    for (HeliumAdapterInfo *adapter in adapters) {
        NSString * partnerVersionKey        = @"partnerVersion";      NSString * partnerVersionValue     = adapter.partnerVersion;
        NSString * adapterVersionKey        = @"adapterVersion";      NSString * adapterVersionValue     = adapter.adapterVersion;
        NSString * partnerIdentifierKey     = @"partnerIdentifier";   NSString * partnerIdentifierValue  = adapter.partnerIdentifier;
        NSString * partnerDisplayNameKey    = @"partnerDisplayName";  NSString * partnerDisplayNameValue = adapter.partnerDisplayName;

        NSDictionary *adapterDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                               partnerVersionValue,partnerVersionKey,
                                           adapterVersionValue,adapterVersionKey,
                                           partnerIdentifierValue,partnerIdentifierKey,
                                           partnerDisplayNameValue,partnerDisplayNameKey,
                                           nil
        ];

        [jsonArray addObject:adapterDictionary];
    }

    return toJSON(jsonArray);
}

void * _chartboostMediationGetInterstitialAd(const char *placementName)
{
    id<HeliumInterstitialAd> ad = [[Helium sharedHelium] interstitialAdProviderWithDelegate: [ChartboostMediationObserver sharedObserver] andPlacementName: toNSStringOrEmpty(placementName)];
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
    return [ad.keywords setKeyword:toNSStringOrEmpty(keyword) value:toNSStringOrEmpty(value)];
}

const char * _chartboostMediationInterstitialRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return toCStringOrNull([ad.keywords removeKeyword:toNSStringOrEmpty(keyword)]);
}

void _chartboostMediationInterstitialAdLoad(const void * uniqueId)
{
    toMain(^{
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
    toMain(^{
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
    id<HeliumRewardedAd> ad = [[Helium sharedHelium] rewardedAdProviderWithDelegate: [ChartboostMediationObserver sharedObserver] andPlacementName: toNSStringOrEmpty(placementName)];
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
    return [ad.keywords setKeyword:toNSStringOrEmpty(keyword) value:toNSStringOrEmpty(value)];
}

const char * _chartboostMediationRewardedRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    return toCStringOrNull([ad.keywords removeKeyword:toNSStringOrEmpty(keyword)]);
}

void _chartboostMediationRewardedAdLoad(const void * uniqueId)
{
    toMain(^{
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
    toMain(^{
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
    ad.customData = toNSStringOrNull(customData);
}

void _chartboostMediationLoadFullscreenAd(const char *placementName, const char *keywords, int hashCode, ChartboostMediationFullscreenAdLoadResultEvent callback)
{
    NSDictionary *formattedKeywords = objFromJsonString<NSDictionary *>(keywords);

    ChartboostMediationAdLoadRequest *loadRequest = [[ChartboostMediationAdLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) keywords:formattedKeywords];

    [[Helium sharedHelium] loadFullscreenAdWithRequest:loadRequest completion:^(ChartboostMediationFullscreenAdLoadResult * adLoadResult) {
        ChartboostMediationError *error = [adLoadResult error];
        if (error != nil)
        {
            ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
            const char *code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
            const char *message = toCStringOrNull([error localizedDescription]);
            callback(hashCode, NULL, "", "", "", code, message);
            return;
        }

        id<ChartboostMediationFullscreenAd> ad = [adLoadResult ad];
        [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:true];
        [ad setDelegate:[ChartboostMediationObserver sharedObserver]];
        const char *loadId = toCStringOrNull([adLoadResult loadID]);
        const char *winningBidJson = toJSON([ad winningBidInfo]);
        const char *metricsJson = toJSON([adLoadResult metrics]);
        callback(hashCode, (__bridge void*)ad, loadId, winningBidJson, metricsJson, "", "");
    }];
}

void _chartboostMediationShowFullscreenAd(const void *uniqueId, int hashCode, ChartboostMediationFullscreenAdShowResultEvent callback)
{
    toMain(^{
        id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
        [ad showWith:UnityGetGLViewController() completion:^(ChartboostMediationAdShowResult *adShowResult) {
            ChartboostMediationError *error = [adShowResult error];
            if (error != nil)
            {
                ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
                const char *code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
                const char *message = toCStringOrNull([error localizedDescription]);
                callback(hashCode, "", code, message);
                return;
            }

            const char *metricsJson = toJSON([adShowResult metrics]);
            UnityPause(true);
            callback(hashCode, metricsJson, "", "");
        }];
    });
}

void _chartboostMediationInvalidateFullscreenAd(const void *uniqueId)
{
    toMain(^() {
        id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
        [ad invalidate];
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:nil multiPlacementSupport:true];
    });
}

void _chartboostMediationFullscreenSetCustomData(const void *uniqueId, const char *customData)
{
    id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
    [ad setCustomData:toNSStringOrNull(customData)];
}

const char * _chartboostMediationFullscreenAdLoadId(const void *uniqueId)
{
    id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
    return toCStringOrNull([ad loadID]);
}

const char * _chartboostMediationFullscreenAdWinningBidInfo(const void *uniqueId)
{
    id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
    return toJSON([ad winningBidInfo]);
}

const char * _chartboostMediationFullscreenAdRequest(const void *uniqueId)
{
    id<ChartboostMediationFullscreenAd> ad = (__bridge id<ChartboostMediationFullscreenAd>)uniqueId;
    NSString * placementKey = @"placementName";     NSString * placemnentValue = ad.request.placement;
    NSString * keywordsKey = @"keywords";           NSDictionary * keywordsValue = ad.request.keywords;
    
    NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:placemnentValue,placementKey,keywordsValue,keywordsKey, nil];
    return toJSON(dict);
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

    HeliumBannerView *ad = [[Helium sharedHelium] bannerProviderWithDelegate:[ChartboostMediationObserver sharedObserver] andPlacementName:toNSStringOrEmpty(placementName) andSize:cbSize];

    if (ad == NULL)
        return NULL;

    [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:true];
    return (__bridge void*)ad;
}

__deprecated
BOOL _chartboostMediationBannerSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    toMain(^{
        if (ad.keywords == nil)
            ad.keywords = [[HeliumKeywords alloc] init];
    });
    return [ad.keywords setKeyword:toNSStringOrEmpty(keyword) value:toNSStringOrEmpty(value)];
}

__deprecated
const char * _chartboostMediationBannerRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    return toCStringOrNull([ad.keywords removeKeyword:toNSStringOrEmpty(keyword)]);
}

__deprecated
void _chartboostMediationBannerAdLoad(const void * uniqueId, long screenLocation)
{
    toMain(^{
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
    toMain(^(){
        id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
        [ad clearAd];
    });
}

__deprecated
void _chartboostMediationBannerRemove(const void * uniqueId)
{
    toMain(^(){
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView removeFromSuperview];
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:nil multiPlacementSupport:true];
    });
}

__deprecated
void _chartboostMediationBannerSetVisibility(const void * uniqueId, BOOL isVisible)
{
    toMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView setHidden:!isVisible];
    });
}

__deprecated
void _chartboostMediationFreeAdObject(const void * uniqueId, const char * placementName, bool multiPlacementSupport)
{
    toMain(^(){
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:toNSStringOrEmpty(placementName) multiPlacementSupport:multiPlacementSupport];
    });
}


const void* _chartboostMediationLoadBannerView(ChartboostMediationBannerAdDragEvent dragListener){

    ChartboostMediationBannerView *bannerView = [[ChartboostMediationBannerView alloc] init];
    [bannerView setDelegate:[ChartboostMediationObserver sharedObserver]];

    ChartboostMediationBannerAdWrapper *wrapper = [[ChartboostMediationBannerAdWrapper alloc] initWithBannerView:bannerView andDragListener:dragListener];
    [[ChartboostMediationObserver sharedObserver] storeAd:wrapper placementName:nil multiPlacementSupport:true];
    [[ChartboostMediationObserver sharedObserver] storeWrapper:wrapper forAd:bannerView];

    return (__bridge void*)wrapper;
}

void _chartboostMediationBannerViewLoadAdWithScreenPos(const void *uniqueId, const char *placementName, int sizeType, float width, float height, long screenLocation, int hashCode, ChartboostMediationBannerAdLoadResultEvent callback) {
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);

    ChartboostMediationBannerSize *size;
    switch(sizeType){
        case 0 : size = [ChartboostMediationBannerSize standard]; break;
        case 1 : size = [ChartboostMediationBannerSize medium]; break;
        case 2 : size = [ChartboostMediationBannerSize leaderboard]; break;
        case 3 : size = [ChartboostMediationBannerSize adaptiveWithWidth:width maxHeight:height]; break;
        default: size =  [ChartboostMediationBannerSize adaptiveWithWidth:0 maxHeight:0]; break;
    }

    ChartboostMediationBannerLoadRequest *loadRequest = [[ChartboostMediationBannerLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) size:size];
    UIViewController* viewController = [[ChartboostMediationObserver sharedObserver] getBannerViewController:bannerView size:size.size screenLocation:screenLocation];

    // Load
    [bannerView loadWith:loadRequest viewController:viewController completion:^(ChartboostMediationBannerLoadResult *adLoadResult) {
        ChartboostMediationError *error = [adLoadResult error];
        if (error != nil)
        {
            ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
            const char *code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
            const char *message = toCStringOrNull([error localizedDescription]);
            callback(hashCode, uniqueId, "", "", 0, 0, code, message);
            return;
        }

        const char *loadId = toCStringOrNull([adLoadResult loadID]);
        const char *metricsJson = toJSON([adLoadResult metrics]);
        float width = adLoadResult.size.size.width;
        float height = adLoadResult.size.size.height;
        callback(hashCode, uniqueId, loadId, metricsJson, width, height, "", "");
    }];

    ChartboostMediationBannerAdWrapper *bannerWrapper = (__bridge ChartboostMediationBannerAdWrapper *)uniqueId;
    bannerWrapper.usesConstraints = true;
}

void _chartboostMediationBannerViewLoadAdWithXY(const void *uniqueId, const char *placementName, int sizeType, float width, float height, float x, float y, int hashCode, ChartboostMediationBannerAdLoadResultEvent callback) {
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);

    ChartboostMediationBannerSize *size;
    switch(sizeType){
        case 0 : size = [ChartboostMediationBannerSize standard]; break;
        case 1 : size = [ChartboostMediationBannerSize medium]; break;
        case 2 : size = [ChartboostMediationBannerSize leaderboard]; break;
        case 3 : size = [ChartboostMediationBannerSize adaptiveWithWidth:width maxHeight:height]; break;
        default: size =  [ChartboostMediationBannerSize adaptiveWithWidth:0 maxHeight:0]; break;
    }

    ChartboostMediationBannerLoadRequest *loadRequest = [[ChartboostMediationBannerLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) size:size];
    UIViewController* viewController = [[ChartboostMediationObserver sharedObserver] getBannerViewController:bannerView size:size.size x:x y:y];

    // Load
    [bannerView loadWith:loadRequest viewController:viewController completion:^(ChartboostMediationBannerLoadResult *adLoadResult) {
        ChartboostMediationError *error = [adLoadResult error];
        if (error != nil)
        {
            ChartboostMediationErrorCode codeInt = [error chartboostMediationCode];
            const char *code = toCStringOrNull([NSString stringWithFormat:@"CM_%ld", codeInt]);
            const char *message = toCStringOrNull([error localizedDescription]);
            callback(hashCode, uniqueId, "", "", 0, 0, code, message);
            return;
        }

        const char *loadId = toCStringOrNull([adLoadResult loadID]);
        const char *metricsJson = toJSON([adLoadResult metrics]);
        float width = adLoadResult.size.size.width;
        float height = adLoadResult.size.size.height;
        callback(hashCode, uniqueId, loadId, metricsJson, width, height, "", "");
    }];

    ChartboostMediationBannerAdWrapper *bannerWrapper = (__bridge ChartboostMediationBannerAdWrapper *)uniqueId;
    bannerWrapper.usesConstraints = false;
}

void _chartboostMediationBannerViewSetKeywords(const void* uniqueId, const char * keywords){
    ChartboostMediationBannerView *ad = _getBannerView(uniqueId);
    NSMutableDictionary *formattedKeywords = objFromJsonString<NSMutableDictionary *>(keywords);
    ad.keywords = formattedKeywords;
}

const char * _chartboostMediationBannerViewGetAdSize(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    return sizeToJSON(bannerView.size);
}

const char * _chartboostMediationBannerViewGetContainerSize(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);

    // Note : `bannerView.size.type` is always 0 even if load request was made with adaptive size
    // Therefore, we always set it to adaptive here and let Unity updated it based on size type at load request
    // if(bannerView.size.type == 0)    // Fixed
    //      return sizeToJSON(bannerView.request.size);

    // Adaptive
    ChartboostMediationBannerSize *size = [ChartboostMediationBannerSize adaptiveWithWidth:bannerView.frame.size.width maxHeight:bannerView.frame.size.height];
    return sizeToJSON(size);
}

const char * _chartboostMediationBannerViewGetWinningBidInfo(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    if(bannerView.winningBidInfo != nil)
        return toJSON(bannerView.winningBidInfo);
    return NULL;
}

const char * _chartboostMediationBannerViewGetLoadMetrics(const void* uniqueId){
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    if(bannerView.loadMetrics != nil)
        return toJSON(bannerView.loadMetrics);
    return NULL;
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

void _chartboostMediationBannerViewResizeToFit(const void* uniqueId, int axis, float pivotX, float pivotY) {
    ChartboostMediationBannerAdWrapper *bannerWrapper = (__bridge ChartboostMediationBannerAdWrapper *)uniqueId;
    [bannerWrapper resize:axis pivotX:pivotX pivotY:pivotY];
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
    ChartboostMediationBannerView *bannerView = _getBannerView(uniqueId);
    [bannerView reset];
}

void _chartboostMediationBannerViewDestroy(const void * uniqueId)
{
    toMain(^(){
        ChartboostMediationBannerView* bannerView = _getBannerView(uniqueId);
        [bannerView removeFromSuperview];
        [[ChartboostMediationObserver sharedObserver] releaseAd: [NSNumber numberWithLong:(long)uniqueId] placementName:nil multiPlacementSupport:true];

        [[ChartboostMediationObserver sharedObserver] releaseWrapperForAd:[NSNumber numberWithLong:(long)bannerView]];
    });
}

void _chartboostMediationBannerViewMoveTo(const void * uniqueId, float x, float y)
{
    toMain(^(){
        ChartboostMediationBannerView* bannerView = _getBannerView(uniqueId);
        CGRect frame = bannerView.frame;
        CGPoint origin = bannerView.frame.origin;
        origin.x = x;
        origin.y = y;
        frame.origin = origin;
        bannerView.frame = frame;
    });
}

const char * _chartboostMediationGetPartnerConsentDictionary() {
    NSDictionary<NSString *, NSNumber* >* consents   = [[Helium sharedHelium] partnerConsents];
    return toJSON(consents);
}

void _chartboostMediationSetPartnerConsent(const char* partnerIdentifier, bool consentGranted){

    if (partnerIdentifier == nil)
        return;

    NSString* partnerId = toNSStringOrEmpty(partnerIdentifier);
    NSNumber* consentValue = [NSNumber numberWithBool:consentGranted];
    NSMutableDictionary* mutableCopy  = [[[Helium sharedHelium] partnerConsents] mutableCopy];
    [mutableCopy setValue:consentValue forKey:partnerId];
    [[Helium sharedHelium] setPartnerConsents:mutableCopy];
}

void _chartboostMediationAddPartnerConsents(const char* partnerConsentsJson){
    NSDictionary* partnerConsents = objFromJsonString<NSDictionary *>(partnerConsentsJson);
    NSMutableDictionary* mutableCopy  = [[[Helium sharedHelium] partnerConsents] mutableCopy];
    [mutableCopy addEntriesFromDictionary:partnerConsents];
    [[Helium sharedHelium] setPartnerConsents:mutableCopy];
}

void _chartboostMediationReplacePartnerConsents(const char* partnerConsentsJson){
    NSDictionary* partnerConsents = objFromJsonString<NSDictionary *>(partnerConsentsJson);
    [[Helium sharedHelium] setPartnerConsents:partnerConsents];
}

void _chartboostMediationClearConsents()
{
    NSDictionary * emptyDictionary = [NSDictionary dictionary];
    [[Helium sharedHelium] setPartnerConsents:emptyDictionary];
}

int _chartboostMediationRemovePartnerConsent(const char* partnerIdentifier)
{
    NSString* partnerId = toNSStringOrEmpty(partnerIdentifier);
    NSMutableDictionary* mutableCopy  = [[[Helium sharedHelium] partnerConsents] mutableCopy];
    NSNumber* previousValue = [[mutableCopy valueForKey:partnerId] copy];

    if (previousValue == nil)
        return -1;

    [mutableCopy removeObjectForKey:partnerId];
    [[Helium sharedHelium] setPartnerConsents:mutableCopy];
    return [previousValue intValue];
}

void * _chartboostMediationFullscreenAdQueueQueue(const char * placementName)
{
    ChartboostMediationFullscreenAdQueue * queue = [ChartboostMediationFullscreenAdQueue queueForPlacement:toNSStringOrEmpty(placementName)];
    // TODO: This will reset the delegate on the queue with same placement name. Should be fine though ?
    [queue setDelegate:[ChartboostMediationObserver sharedObserver]];
    return (__bridge void *)queue;
}
        

void _chartboostMediationFullscreenAdQueueSetKeywords(const void * uniqueId, const char * keywordsJson)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    NSMutableDictionary *formattedKeywords = objFromJsonString<NSMutableDictionary *>(keywordsJson);
    [queue setKeywords:formattedKeywords];
}
        
        
int _chartboostMediationFullscreenAdQueueQueueCapacity(const void * uniqueId)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    return queue.queueCapacity;
}

        
int _chartboostMediationFullscreenAdQueueNumberOfAdsReady(const void * uniqueId) 
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    return queue.numberOfAdsReady;
}
        
        
bool _chartboostMediationFullscreenAdQueueIsRunning(const void * uniqueId)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    return queue.isRunning;
}
        
void * _chartboostMediationFullscreenAdQueueGetNextAd(const void * uniqueId)
{
    ChartboostMediationFullscreenAdQueue* queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    id<ChartboostMediationFullscreenAd> ad = [queue getNextAd];
    const char * placementName = toCStringOrNull(ad.request.placement);
    [[ChartboostMediationObserver sharedObserver] storeAd:ad placementName:placementName multiPlacementSupport:true];
    [ad setDelegate:[ChartboostMediationObserver sharedObserver]];

    return (__bridge void*)ad;
}

bool _chartboostMediationFullscreenAdQueueHasNextAd(const void * uniqueId)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    return queue.hasNextAd;
}
        
void _chartboostMediationFullscreenAdQueueStart(const void * uniqueId)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    [queue start];
}
        
void _chartboostMediationFullscreenAdQueueStop(const void * uniqueId)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    [queue stop];
}

void _chartboostMediationFullscreenAdQueueSetCapacity(const void * uniqueId, int capacity)
{
    ChartboostMediationFullscreenAdQueue * queue = (__bridge ChartboostMediationFullscreenAdQueue *)uniqueId;
    [queue setQueueCapacity:capacity];
}

}
