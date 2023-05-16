/*
* ChartboostMediationBinding.mm
* Chartboost Mediation SDK iOS/Unity
*/
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>
#import "ChartboostMediationManager.h"
#import "UnityAppController.h"

// Converts C style string to NSString
#define GetStringParam(_x_) (_x_ != NULL) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

// Converts C style string to NSString as long as it isnt empty
#define GetStringParamOrNil(_x_) (_x_ != NULL && strlen(_x_)) ? [NSString stringWithUTF8String:_x_] : nil

static char* ConvertNSStringToCString(const NSString* nsString) {
    if (nsString == NULL) return NULL;
    const char* nsStringUtf8 = [nsString UTF8String];
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);
    return cString;
}

typedef void (^block)(void);

static void sendToMain(block block) {
    dispatch_async(dispatch_get_main_queue(), block);
}

static void sendToBackground(block block) {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0), block);
}

void _setLifeCycleCallbacks(ChartboostMediationEvent didStartCallback, ChartboostMediationILRDEvent didReceiveILRDCallback, ChartboostMediationPartnerInitializationDataEvent didReceivePartnerInitializationDataCallback)
{
    [[ChartboostMediationManager sharedManager] setLifeCycleCallbacks:didStartCallback didReceiveILRDCallback:didReceiveILRDCallback didReceivePartnerInitializationData:didReceivePartnerInitializationDataCallback];
}

void _setInterstitialCallbacks(ChartboostMediationPlacementLoadEvent didLoadCallback, ChartboostMediationPlacementEvent didShowCallback, ChartboostMediationPlacementEvent didCloseCallback, ChartboostMediationPlacementEvent didClickCallback, ChartboostMediationPlacementEvent didRecordImpression)
{
    [[ChartboostMediationManager sharedManager] setInterstitialCallbacks:didLoadCallback didShowCallback:didShowCallback didCloseCallback:didCloseCallback didClickCallback:didClickCallback didRecordImpression:didRecordImpression];
}

void _setRewardedCallbacks(ChartboostMediationPlacementLoadEvent didLoadCallback, ChartboostMediationPlacementEvent didShowCallback, ChartboostMediationPlacementEvent didCloseCallback, ChartboostMediationPlacementEvent didClickCallback, ChartboostMediationPlacementEvent didRecordImpression, ChartboostMediationPlacementEvent didReceiveRewardCallback){
        
    [[ChartboostMediationManager sharedManager] setRewardedCallbacks:didLoadCallback didShowCallback:didShowCallback didCloseCallback:didCloseCallback didClickCallback:didClickCallback didRecordImpression:didRecordImpression didReceiveRewardCallback:didReceiveRewardCallback];
}

void _setBannerCallbacks(ChartboostMediationPlacementLoadEvent didLoadCallback, ChartboostMediationPlacementEvent didRecordImpression, ChartboostMediationPlacementEvent didClickCallback)
{
    [[ChartboostMediationManager sharedManager] setBannerCallbacks:didLoadCallback didRecordImpression:didRecordImpression didClickCallback:didClickCallback];
}

void _chartboostMediationInit(const char *appId, const char *appSignature, const char *unityVersion, const char** initOptions, int optionsSize)
{
    [[ChartboostMediationManager sharedManager] startHeliumWithAppId:GetStringParam(appId) andAppSignature:GetStringParam(appSignature) unityVersion:GetStringParam(unityVersion) initializationOptions:initOptions initializationOptionsSize:optionsSize];
}

void _chartboostMediationSetSubjectToCoppa(BOOL isSubject)
{
    [[ChartboostMediationManager sharedManager] setSubjectToCoppa:isSubject];
}

void _chartboostMediationSetSubjectToGDPR(BOOL isSubject)
{
    [[ChartboostMediationManager sharedManager] setSubjectToGDPR:isSubject];
}

void _chartboostMediationSetUserHasGivenConsent(BOOL hasGivenConsent)
{
    [[ChartboostMediationManager sharedManager] setUserHasGivenConsent:hasGivenConsent];
}

void _chartboostMediationSetCCPAConsent(BOOL hasGivenConsent)
{
    [[ChartboostMediationManager sharedManager] setCCPAConsent:hasGivenConsent];
}

void _chartboostMediationSetUserIdentifier(const char * userIdentifier)
{
    [[ChartboostMediationManager sharedManager] setUserIdentifier:GetStringParam(userIdentifier)];
}

char * _chartboostMediationGetUserIdentifier()
{
    return ConvertNSStringToCString([[ChartboostMediationManager sharedManager] getUserIdentifier]);
}

void _chartboostMediationSetTestMode(BOOL isTestModeEnabled)
{
    [[ChartboostMediationManager sharedManager] setTestMode:isTestModeEnabled];
}

float _chartboostMediationGetRetinaScaleFactor() {
    return UIScreen.mainScreen.scale;
}


void * _chartboostMediationGetInterstitialAd(const char *placementName)
{
    return (__bridge void*) [[ChartboostMediationManager sharedManager] getInterstitialAd:GetStringParam(placementName)];
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
    sendToBackground(^{
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
    sendToBackground(^{
        id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
        [ad showAdWithViewController: UnityGetGLViewController()];
    });
}

BOOL _chartboostMediationInterstitialAdReadyToShow(const void * uniqueId)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return [ad readyToShow];
}

void _chartboostMediationFreeInterstitialAdObject(const void * uniqueId)
{
    sendToBackground(^{
        [[ChartboostMediationManager sharedManager] freeInterstitialAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}

void * _chartboostMediationGetRewardedAd(const char *placementName)
{
    return (__bridge void*) [[ChartboostMediationManager sharedManager] getRewardedAd:GetStringParam(placementName)];
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
    sendToBackground(^{
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
    sendToBackground(^{
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

void _chartboostMediationFreeRewardedAdObject(const void * uniqueId)
{
    sendToBackground(^{
        [[ChartboostMediationManager sharedManager] freeRewardedAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}

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
    return (__bridge void*) [[ChartboostMediationManager sharedManager] getBannerAd:GetStringParam(placementName) andSize:cbSize];
}

BOOL _chartboostMediationBannerSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    if (ad.keywords == nil)
        ad.keywords = [[HeliumKeywords alloc] init];
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

char * _chartboostMediationBannerRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

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

void _chartboostMediationBannerAdLoadWithParams(const void * uniqueId, float x, float y, long width, long height)
{
    sendToMain(^{
        
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        //TODO handle the null case
        UIViewController *unityVC = GetAppController().rootViewController;
        UILayoutGuide *safeGuide;
        if (@available(iOS 11.0, *))
            safeGuide = unityVC.view.safeAreaLayoutGuide;
        else
            safeGuide = unityVC.view.layoutMarginsGuide;
//        bannerView.translatesAutoresizingMaskIntoConstraints=NO;
        [bannerView removeFromSuperview];
        [unityVC.view  addSubview:bannerView];
        
        float scale = UIScreen.mainScreen.scale;
        bannerView.frame = CGRectMake(x/scale, y/scale, width/scale, height/scale);
//        bannerView.layer.contentsScale = UIScreen.mainScreen.scale;
                
                        
        NSLog(@"Dragging added");

        
        NSLog(@"scale: %f", scale);
        
        NSLog(@"Unity input: %f, %f, %li, %li", x,y,width,height);
        
        
        NSLog(@"BannerView bounds: %f, %f, %f, %f", bannerView.bounds.origin.x, bannerView.bounds.origin.y, bannerView.bounds.size.width, bannerView.bounds.size.height );
        
        NSLog(@"BannerView frame: %f, %f, %f, %f", bannerView.frame.origin.x, bannerView.frame.origin.y, bannerView.frame.size.width, bannerView.frame.size.height );
        
        NSLog(@"SuperView bounds: %f, %f, %f, %f", [bannerView superview].bounds.origin.x, [bannerView superview].bounds.origin.y, [bannerView superview].bounds.size.width, [bannerView superview].bounds.size.height);
        
        NSLog(@"SuperView frame: %f, %f, %f, %f", [bannerView superview].frame.origin.x, [bannerView superview].frame.origin.y, [bannerView superview].frame.size.width, [bannerView superview].frame.size.height);

        id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
        [ad loadAdWithViewController:unityVC];
    });
}

void _chartboostMediationBannerEnableDrag(const void * uniqueId, ChartboostMediationBannerDragEvent dragListener){
    [[ChartboostMediationManager sharedManager] enableBannerDrag:uniqueId listener:dragListener];
}

void _chartboostMediationBannerDisableDrag(const void * uniqueId){
    [[ChartboostMediationManager sharedManager] disableBannerDrag:uniqueId];
}

void _chartboostMediationBannerAdSetlayoutParams(const void * uniqueId, float x, float y, long width, long height)
{
    sendToMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        bannerView.frame = CGRectMake(x, y, width, height);
        bannerView.layer.contentsScale = UIScreen.mainScreen.scale;
    });
}

void _chartboostMediationBannerClearLoaded(const void * uniqueId)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    [ad clearAd];
}

void _chartboostMediationBannerRemove(const void * uniqueId)
{
    sendToMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView removeFromSuperview];
        [[ChartboostMediationManager sharedManager] freeBannerAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}

void _chartboostMediationBannerSetVisibility(const void * uniqueId, BOOL isVisible)
{
    sendToMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView setHidden:!isVisible];
    });
}

void _chartboostMediationFreeBannerAdObject(const void * uniqueId)
{
    sendToBackground(^{
        [[ChartboostMediationManager sharedManager] freeBannerAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}
 
