#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "CBMBannerAdWrapper.h"
#import "CBMUnityUtilities.h"
#import "UnityAppController.h"


#pragma mark Banner Utility Methods

NSString* const errorFormat = @"CM_%ld";

static CBMBannerSize* GetBannerSize(int sizeType, float width, float height){
    CBMBannerSize *size;
    switch(sizeType){
        case 0 : size = [CBMBannerSize standard]; break;
        case 1 : size = [CBMBannerSize medium]; break;
        case 2 : size = [CBMBannerSize leaderboard]; break;
        case 3 : size = [CBMBannerSize adaptiveWithWidth:width maxHeight:height]; break;
        default: size =  [CBMBannerSize adaptiveWithWidth:0 maxHeight:0]; break;
    }
    return size;
}

#pragma mark Extern Methods
extern "C" {

    void _CBMBannerAdSetCallbacks(CBMExternBannerAdEvent bannerAdEvents){
        [[CBMUnityObserver sharedObserver] setDidReceiveBannerAdEvent:bannerAdEvents];
    }

    const void* _CBMGetBannerAd(CBMExternBannerAdDragEvent dragListener) {
        // Get BannerView
        CBMBannerAdView* bannerView = [[CBMBannerAdView alloc] init];
        
        // Set delegate
        [bannerView setDelegate:[CBMUnityObserver sharedObserver]];
        
        // Wrap
        CBMBannerAdWrapper* wrapper = [[CBMBannerAdWrapper alloc] initWithBannerView:bannerView dragListener:dragListener];
            
        NSNumber *key = [NSNumber numberWithLong:(long)bannerView];
        [[CBMAdStore sharedStore] storeAd:wrapper withKey:key];
        return (__bridge void*)wrapper;
    }

    const char * _CBMBannerAdGetKeywords(const void* uniqueId) {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toJSON([bannerAdWrapper keywords]);
    }

    void _CBMBannerAdSetKeywords(const void* uniqueId, const char * keywordsJson){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if (keywordsJson != NULL) {
            NSMutableDictionary *keywords = toObjectFromJson(keywordsJson);
            [bannerAdWrapper setKeywords:keywords];
        }
        else {
            [bannerAdWrapper setKeywords:nil];
        }
    }
    const char * _CBMBannerAdGetPartnerSettings(const void * uniqueId)
    {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toJSON([bannerAdWrapper partnerSettings]);
    }
    
    void _CBMBannerAdSetPartnerSettings(const void* uniqueId, const char * partnerSettingsJson){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if (partnerSettingsJson != NULL) {
            NSMutableDictionary *partnerSettings = toObjectFromJson(partnerSettingsJson);
            [bannerAdWrapper setPartnerSettings:partnerSettings];
        }
        else {
            [bannerAdWrapper setPartnerSettings:nil];
        }
    }
    
    void _CBMBannerAdSetPosition(const void * uniqueId, float x, float y)
    {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGPoint pos = CGPointMake(x, y);
        [bannerAdWrapper setPosition:pos];
    }
    
    const char * _CBMBannerAdGetPosition(const void * uniqueId)
    {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGPoint position = [bannerAdWrapper position];
        const NSString * xKey = @"x";
        const NSString * yKey = @"y";
        NSString * xValue =  [NSString stringWithFormat:@"%f", position.x];
        NSString * yValue = [NSString stringWithFormat:@"%f", position.y];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:xValue, xKey, yValue, yKey, nil]);
    }
    
    void _CBMBannerAdSetPivot(const void * uniqueId, float x, float y)
    {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        bannerAdWrapper.pivot = CGPointMake(x, y);
    }
    
    const char * _CBMBannerAdGetPivot(const void * uniqueId)
    {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        const NSString * xKey = @"x";
        const NSString * yKey = @"y";
        NSString * xValue =  [NSString stringWithFormat:@"%f", bannerAdWrapper.pivot.x];
        NSString * yValue = [NSString stringWithFormat:@"%f", bannerAdWrapper.pivot.y];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:xValue, xKey, yValue, yKey, nil]);
    }

    const char * _CBMBannerAdGetRequest(const void *uniqueId) {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        const NSString * placementKey = @"placementName";
        const NSString * sizeKey = @"size";
        NSString * placementValue =  [[bannerAdWrapper request] placement];
        NSDictionary * sizeValue = bannerSizeToDictionary([[bannerAdWrapper request] size]);
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:placementValue, placementKey, sizeValue, sizeKey, nil]);
    }

    const char * _CBMBannerAdGetBidInfo(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if([bannerAdWrapper winningBidInfo] != nil)
            return toJSON([bannerAdWrapper winningBidInfo]);
        return NULL;
    }

    const char * _CBMBannerAdGetLoadMetrics(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if([bannerAdWrapper loadMetrics] != nil)
            return toJSON([bannerAdWrapper loadMetrics]);
        return NULL;
    }

    const char * _CBMBannerAdGetBannerSize(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toJSON(bannerSizeToDictionary([[bannerAdWrapper bannerView] size]));
    }

    const char * _CBMBannerAdGetContainerSize(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGSize containerSize = [bannerAdWrapper containerSize];
        
        const NSString * widthKey = @"width";
        const NSString * heightKey = @"height";

        NSString * widthValue = [NSString stringWithFormat:@"%d", (int)containerSize.width];
        NSString * heightValue = [NSString stringWithFormat:@"%d", (int)containerSize.height];

        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:widthValue,widthKey,heightValue,heightKey,nil]);
    }

    void _CBMBannerAdSetContainerSize(const void * uniqueId, float width, float height){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setContainerSize:CGSizeMake(width, height)];
    }

    int _CBMBannerAdGetHorizontalAlignment(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return (int)[bannerAdWrapper  horizontalAlignment];
    }

    void _CBMBannerAdSetHorizontalAlignment(const void* uniqueId, int horizontalAlignment){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setHorizontalAlignment:(CBMBannerHorizontalAlignment)horizontalAlignment];
    }

    int _CBMBannerAdGetVerticalAlignment(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return (int)[bannerAdWrapper verticalAlignment];
    }

    void _CBMBannerAdSetVerticalAlignment(const void* uniqueId, int verticalAlignment){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setVerticalAlignment:(CBMBannerVerticalAlignment)verticalAlignment];
    }

    BOOL _CBMBannerAdGetDraggability(const void* uniqueId) {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return [bannerAdWrapper draggable];
    }

    void _CBMBannerAdSetDraggability(const void* uniqueId, BOOL canDrag){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setDraggable:canDrag];
    }

    BOOL _CBMBannerAdGetVisibility(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return [bannerAdWrapper visible];
    }

    void _CBMBannerAdSetVisibility(const void* uniqueId, BOOL visible){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setVisible:visible];
    }

    void _CBMBannerAdLoadAd(const void *uniqueId, const char *placementName, int sizeType, float width, float height, int hashCode, CBMExternBannerAdLoadResultEvent adLoadResultCallback) {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CBMBannerSize *size = GetBannerSize(sizeType, width, height);
        CBMBannerAdLoadRequest *loadRequest = [[CBMBannerAdLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) size:size];
        
        // Load
        UIViewController* unityViewController = GetAppController().rootViewController;
        [bannerAdWrapper loadWith:loadRequest viewController:unityViewController completion:^(CBMBannerAdLoadResult *adLoadResult) {
            CBMError *error = [adLoadResult error];
            if (error != nil)
            {
                CBMErrorCode codeInt = [error chartboostMediationCode];
                const char *code = toCStringOrNull([NSString stringWithFormat:errorFormat, codeInt]);
                const char *message = toCStringOrNull([error localizedDescription]);
                adLoadResultCallback(hashCode, uniqueId, NULL, NULL, NULL, 0, 0, code, message);
                return;
            }

            const char *loadId = toCStringOrNull([adLoadResult loadID]);
            const char *metricsJson = toJSON([adLoadResult metrics]);
            const char *winningBidInfoJson = toJSON([adLoadResult winningBidInfo]);
            float width = adLoadResult.size.size.width;
            float height = adLoadResult.size.size.height;
            adLoadResultCallback(hashCode, uniqueId, loadId, metricsJson, winningBidInfoJson, width, height, NULL, NULL);
        }];
        
    }

    void _CBMBannerAdReset(const void* uniqueId){
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper reset];
    }

    void _CBMBannerAdDestroy(const void* uniqueId)
    {
        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper destroy];
        [[CBMAdStore sharedStore] releaseAd:(__bridge void*)[bannerAdWrapper bannerView]];
    }
}

