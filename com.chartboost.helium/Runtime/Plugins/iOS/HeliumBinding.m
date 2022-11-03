/*
 * HeliumBinding.m
 * Helium SDK
 */

#import "HeliumSdk/HeliumSdk.h"
#import "HeliumSdkManager.h"
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

void _setLifeCycleCallbacks(HeliumEvent didStartCallback, HeliumILRDEvent didReceiveILRDCallback, HeliumPartnerInitializationDataEvent didReceivePartnerInitializationDataCallback)
{
    [[HeliumSdkManager sharedManager] setLifeCycleCallbacks:didStartCallback didReceiveILRDCallback:didReceiveILRDCallback didReceivePartnerInitializationData:didReceivePartnerInitializationDataCallback];
}

void _setInterstitialCallbacks(HeliumPlacementEvent didLoadCallback, HeliumPlacementEvent didShowCallback, HeliumPlacementEvent didClickCallback, HeliumPlacementEvent didCloseCallback, HeliumPlacementEvent didRecordImpression, HeliumBidWinEvent didWinBidCallback)
{
    [[HeliumSdkManager sharedManager] setInterstitialCallbacks:didLoadCallback didShowCallback:didShowCallback didClickCallback:didClickCallback didCloseCallback:didCloseCallback didRecordImpression:didRecordImpression didWinBidCallback:didWinBidCallback];
}

void _setRewardedCallbacks(HeliumPlacementEvent didLoadCallback, HeliumPlacementEvent didShowCallback, HeliumPlacementEvent didClickCallback, HeliumPlacementEvent didCloseCallback, HeliumPlacementEvent didRecordImpression, HeliumBidWinEvent didWinBidCallback, HeliumRewardEvent didReceiveRewardCallback){
    [[HeliumSdkManager sharedManager] setRewardedCallbacks:didLoadCallback didShowCallback:didShowCallback didClickCallback:didClickCallback didCloseCallback:didCloseCallback didRecordImpression:didRecordImpression didWinBidCallback:didWinBidCallback didReceiveRewardCallback:didReceiveRewardCallback];
}

void _setBannerCallbacks(HeliumPlacementEvent didLoadCallback, HeliumPlacementEvent didRecordImpression, HeliumPlacementEvent didClickCallback, HeliumBidWinEvent didWinBidCallback)
{
    [[HeliumSdkManager sharedManager] setBannerCallbacks:didLoadCallback didRecordImpression:didRecordImpression didClickCallback:didClickCallback  didWinBidCallback:didWinBidCallback];
}

void _heliumSdkInit(const char *appId, const char *appSignature, const char *unityVersion)
{
    [[HeliumSdkManager sharedManager] startHeliumWithAppId:GetStringParam(appId)
                                           andAppSignature:GetStringParam(appSignature)
                                              unityVersion:GetStringParam(unityVersion)];
}

void _heliumSdkSetSubjectToCoppa(BOOL isSubject)
{
    [[HeliumSdkManager sharedManager] setSubjectToCoppa:isSubject];
}

void _heliumSdkSetSubjectToGDPR(BOOL isSubject)
{
    [[HeliumSdkManager sharedManager] setSubjectToGDPR:isSubject];
}

void _heliumSdkSetUserHasGivenConsent(BOOL hasGivenConsent)
{
    [[HeliumSdkManager sharedManager] setUserHasGivenConsent:hasGivenConsent];
}

void _heliumSetCCPAConsent(BOOL hasGivenConsent)
{
    [[HeliumSdkManager sharedManager] setCCPAConsent:hasGivenConsent];
}

void _heliumSetUserIdentifier(const char * userIdentifier)
{
    [[HeliumSdkManager sharedManager] setUserIdentifier:GetStringParam(userIdentifier)];
}

char * _heliumGetUserIdentifier()
{
    return ConvertNSStringToCString([[HeliumSdkManager sharedManager] getUserIdentifier]);
}

BOOL _heliumSdkIsAnyViewVisible()
{
    return false; // TODO [HeliumSdkManager isAnyViewVisible];
}

void * _heliumSdkGetInterstitialAd(const char *placementName)
{
    return (__bridge void*) [[HeliumSdkManager sharedManager] getInterstitialAd:GetStringParam(placementName)];
}

BOOL _heliumSdkInterstitialSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    if (ad.keywords == nil)
        ad.keywords = [[HeliumKeywords alloc] init];
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

char * _heliumSdkInterstitialRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

void _heliumSdkInterstitialAdLoad(const void * uniqueId)
{
    sendToBackground(^{
        id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
        [ad loadAd];
    });
}

BOOL _heliumSdkInterstitialClearLoaded(const void * uniqueId)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return [ad clearLoadedAd];
}

void _heliumSdkInterstitialAdShow(const void * uniqueId)
{
    sendToBackground(^{
        id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
        [ad showAdWithViewController: UnityGetGLViewController()];
    });
}

BOOL _heliumSdkInterstitialAdReadyToShow(const void * uniqueId)
{
    id<HeliumInterstitialAd> ad = (__bridge id<HeliumInterstitialAd>)uniqueId;
    return [ad readyToShow];
}

void _heliumSdkFreeInterstitialAdObject(const void * uniqueId)
{
    sendToBackground(^{
        [[HeliumSdkManager sharedManager] freeInterstitialAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}

void * _heliumSdkGetRewardedAd(const char *placementName)
{
    return (__bridge void*) [[HeliumSdkManager sharedManager] getRewardedAd:GetStringParam(placementName)];
}

BOOL _heliumSdkRewardedSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    if (ad.keywords == nil)
        ad.keywords = [[HeliumKeywords alloc] init];
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

char * _heliumSdkRewardedRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

void _heliumSdkRewardedAdLoad(const void * uniqueId)
{
    sendToBackground(^{
        id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
        [ad loadAd];
    });
}

BOOL _heliumSdkRewardedClearLoaded(const void * uniqueId)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    return [ad clearLoadedAd];
}

void _heliumSdkRewardedAdShow(const void * uniqueId)
{
    sendToBackground(^{
        id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
        [ad showAdWithViewController: UnityGetGLViewController()];
    });
}

BOOL _heliumSdkRewardedAdReadyToShow(const void * uniqueId)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    return [ad readyToShow];
}

void _heliumSdkRewardedAdSetCustomData(const void * uniqueId, const char * customData)
{
    id<HeliumRewardedAd> ad = (__bridge id<HeliumRewardedAd>)uniqueId;
    ad.customData = GetStringParam(customData);
}

void _heliumSdkFreeRewardedAdObject(const void * uniqueId)
{
    sendToBackground(^{
        [[HeliumSdkManager sharedManager] freeRewardedAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}

void * _heliumSdkGetBannerAd(const char *placementName, long size)
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
    return (__bridge void*) [[HeliumSdkManager sharedManager] getBannerAd:GetStringParam(placementName) andSize:cbSize];
}

BOOL _heliumSdkBannerSetKeyword(const void *uniqueId, const char *keyword, const char *value)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    if (ad.keywords == nil)
        ad.keywords = [[HeliumKeywords alloc] init];
    return [ad.keywords setKeyword:GetStringParam(keyword) value:GetStringParam(value)];
}

char * _heliumSdkBannerRemoveKeyword(const void *uniqueId, const char *keyword)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    return ConvertNSStringToCString([ad.keywords removeKeyword:GetStringParam(keyword)]);
}

void _heliumSdkBannerAdLoad(const void * uniqueId, long screenLocation)
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

BOOL _heliumSdkBannerClearLoaded(const void * uniqueId)
{
    id<HeliumBannerAd> ad = (__bridge id<HeliumBannerAd>)uniqueId;
    return [ad clearAd];
}

void _heliumSdkBannerRemove(const void * uniqueId)
{
    sendToMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView removeFromSuperview];
        [[HeliumSdkManager sharedManager] freeBannerAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}

void _heliumSdkBannerSetVisibility(const void * uniqueId, BOOL isVisible)
{
    sendToMain(^{
        HeliumBannerView* bannerView = (__bridge HeliumBannerView*)uniqueId;
        [bannerView setHidden:!isVisible];
    });
}

void _heliumSdkFreeBannerAdObject(const void * uniqueId)
{
    sendToBackground(^{
        [[HeliumSdkManager sharedManager] freeBannerAd: [NSNumber numberWithLong:(long)uniqueId]];
    });
}
