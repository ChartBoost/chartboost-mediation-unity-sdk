#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "CBMBannerAdWrapper.h"
#import "CBMUnityUtilities.h"
#import "UnityAppController.h"

NSString* const CBMBannerAdBridgeTAG = @"CBMBannerAdBridge";
NSString* const errorFormat = @"CM_%ld";

#pragma mark - Banner Utility Methods

/**
 * Converts integer size type to CBMBannerSize object.
 *
 * @param sizeType The banner size type: 0=standard, 1=medium, 2=leaderboard, 3=adaptive
 * @param width Width for adaptive banners
 * @param height Max height for adaptive banners
 * @return Configured CBMBannerSize instance
 */
static CBMBannerSize* GetBannerSize(int sizeType, float width, float height){
    CBMBannerSize *size;
    switch(sizeType){
        case 0 : size = [CBMBannerSize standard]; break;
        case 1 : size = [CBMBannerSize medium]; break;
        case 2 : size = [CBMBannerSize leaderboard]; break;
        case 3 : size = [CBMBannerSize adaptiveWithWidth:width maxHeight:height]; break;
        default:
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:[NSString stringWithFormat:@"GetBannerSize: Unknown size type %d, defaulting to adaptive", sizeType]
                                                     logLevel:CBLLogLevelWarning];
            size = [CBMBannerSize adaptiveWithWidth:0 maxHeight:0];
            break;
    }
    return size;
}

#pragma mark - Extern Methods

extern "C" {

#pragma mark - Lifecycle

    /**
     * Sets the callback function for banner ad events.
     *
     * @param bannerAdEvents Function pointer to handle banner ad events from Unity
     */
    void _CBMBannerAdSetCallbacks(CBMExternBannerAdEvent bannerAdEvents){
        if (!bannerAdEvents) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetCallbacks: bannerAdEvents callback is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }
        [[CBMUnityObserver sharedObserver] setDidReceiveBannerAdEvent:bannerAdEvents];
    }

    /**
     * Creates a new banner ad wrapper instance.
     *
     * @param dragListener Function pointer to handle drag events
     * @return Bridge pointer to CBMBannerAdWrapper instance, or NULL on failure
     */
    const void* _CBMGetBannerAd(CBMExternBannerAdDragEvent dragListener) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                      log:@"_CBMGetBannerAd: Creating new banner ad wrapper"
                                                 logLevel:CBLLogLevelVerbose];

        // Validate drag listener
        if (!dragListener) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMGetBannerAd: dragListener is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        // Create BannerView
        CBMBannerAdView* bannerView = [[CBMBannerAdView alloc] init];
        if (!bannerView) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMGetBannerAd: Failed to allocate CBMBannerAdView"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        // Set delegate
        [bannerView setDelegate:[CBMUnityObserver sharedObserver]];

        // Create wrapper
        CBMBannerAdWrapper* wrapper = [[CBMBannerAdWrapper alloc] initWithBannerView:bannerView dragListener:dragListener];
        if (!wrapper) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMGetBannerAd: Failed to create CBMBannerAdWrapper"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        // Store in ad store using bannerView as key (needed for delegate callbacks)
        NSNumber *key = [NSNumber numberWithLong:(long)bannerView];
        [[CBMAdStore sharedStore] trackBannerAd:wrapper withKey:key];

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMGetBannerAd: Successfully created banner ad wrapper %p", wrapper]
                                                 logLevel:CBLLogLevelVerbose];

        return (__bridge void*)wrapper;
    }

#pragma mark - Keywords and Partner Settings

    /**
     * Gets the keywords for ad targeting.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string of keywords dictionary, or NULL if no keywords
     */
    const char * _CBMBannerAdGetKeywords(const void* uniqueId) {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetKeywords: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toJSON([bannerAdWrapper keywords]);
    }

    /**
     * Sets the keywords for ad targeting.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param keywordsJson JSON string representing keywords dictionary, or NULL to clear
     */
    void _CBMBannerAdSetKeywords(const void* uniqueId, const char * keywordsJson){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetKeywords: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if (keywordsJson != NULL) {
            NSMutableDictionary *keywords = toObjectFromJson(keywordsJson);
            [bannerAdWrapper setKeywords:keywords];
        }
        else {
            [bannerAdWrapper setKeywords:nil];
        }
    }

    /**
     * Gets the partner-specific settings for mediation adapters.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string of partner settings dictionary, or NULL if no settings
     */
    const char * _CBMBannerAdGetPartnerSettings(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetPartnerSettings: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toJSON([bannerAdWrapper partnerSettings]);
    }

    /**
     * Sets the partner-specific settings for mediation adapters.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param partnerSettingsJson JSON string representing partner settings dictionary, or NULL to clear
     */
    void _CBMBannerAdSetPartnerSettings(const void* uniqueId, const char * partnerSettingsJson){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetPartnerSettings: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if (partnerSettingsJson != NULL) {
            NSMutableDictionary *partnerSettings = toObjectFromJson(partnerSettingsJson);
            [bannerAdWrapper setPartnerSettings:partnerSettings];
        }
        else {
            [bannerAdWrapper setPartnerSettings:nil];
        }
    }
    
#pragma mark - Container Positioning

    /**
     * Sets the position of the banner container in screen coordinates.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param x X coordinate in screen space
     * @param y Y coordinate in screen space
     */
    void _CBMBannerAdSetPosition(const void * uniqueId, float x, float y)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetPosition: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGPoint pos = CGPointMake(x, y);
        [bannerAdWrapper setPosition:pos];
    }

    /**
     * Gets the position of the banner container in screen coordinates.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with x and y coordinates, or NULL on error
     */
    const char * _CBMBannerAdGetPosition(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetPosition: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGPoint position = [bannerAdWrapper position];
        const NSString * xKey = @"x";
        const NSString * yKey = @"y";
        NSString * xValue =  [NSString stringWithFormat:@"%f", position.x];
        NSString * yValue = [NSString stringWithFormat:@"%f", position.y];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:xValue, xKey, yValue, yKey, nil]);
    }

    /**
     * Sets the pivot point for banner positioning calculations.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param x Pivot X coordinate (0-1 range, 0=left, 0.5=center, 1=right)
     * @param y Pivot Y coordinate (0-1 range, 0=top, 0.5=center, 1=bottom)
     */
    void _CBMBannerAdSetPivot(const void * uniqueId, float x, float y)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetPivot: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        bannerAdWrapper.pivot = CGPointMake(x, y);
    }

    /**
     * Gets the pivot point for banner positioning calculations.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with x and y pivot coordinates (0-1 range), or NULL on error
     */
    const char * _CBMBannerAdGetPivot(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetPivot: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        const NSString * xKey = @"x";
        const NSString * yKey = @"y";
        NSString * xValue =  [NSString stringWithFormat:@"%f", bannerAdWrapper.pivot.x];
        NSString * yValue = [NSString stringWithFormat:@"%f", bannerAdWrapper.pivot.y];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:xValue, xKey, yValue, yKey, nil]);
    }

#pragma mark - Banner Properties

    /**
     * Gets the ad load request information.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with placement name and size, or NULL on error
     */
    const char * _CBMBannerAdGetRequest(const void *uniqueId) {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetRequest: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        const NSString * placementKey = @"placementName";
        const NSString * sizeKey = @"size";
        NSString * placementValue =  [[bannerAdWrapper request] placement];
        NSDictionary * sizeValue = bannerSizeToDictionary([[bannerAdWrapper request] size]);
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:placementValue, placementKey, sizeValue, sizeKey, nil]);
    }

    /**
     * Gets the winning bid information from the ad auction.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with winning bid info, or NULL if no bid info available
     */
    const char * _CBMBannerAdGetBidInfo(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetBidInfo: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if([bannerAdWrapper winningBidInfo] != nil)
            return toJSON([bannerAdWrapper winningBidInfo]);
        return NULL;
    }

    /**
     * Gets the unique identifier for the current ad load.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return C string with load ID, or NULL if not loaded
     */
    const char * _CBMBannerAdGetLoadId(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetLoadId: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toCStringOrNull([bannerAdWrapper loadId]);
    }

    /**
     * Gets the metrics data from the ad load.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with load metrics, or NULL if no metrics available
     */
    const char * _CBMBannerAdGetLoadMetrics(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetLoadMetrics: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        if([bannerAdWrapper loadMetrics] != nil)
            return toJSON([bannerAdWrapper loadMetrics]);
        return NULL;
    }

    /**
     * Gets the actual size of the loaded banner ad.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with banner size dimensions, or NULL on error
     */
    const char * _CBMBannerAdGetBannerSize(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetBannerSize: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return toJSON(bannerSizeToDictionary([[bannerAdWrapper bannerView] size]));
    }

    /**
     * Gets the size of the banner container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with container width and height, or NULL on error
     */
    const char * _CBMBannerAdGetContainerSize(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetContainerSize: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGSize containerSize = [bannerAdWrapper containerSize];

        const NSString * widthKey = @"width";
        const NSString * heightKey = @"height";

        NSString * widthValue = [NSString stringWithFormat:@"%d", (int)containerSize.width];
        NSString * heightValue = [NSString stringWithFormat:@"%d", (int)containerSize.height];

        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:widthValue,widthKey,heightValue,heightKey,nil]);
    }

    /**
     * Sets the size of the banner container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param width Container width (-1 for wrap content)
     * @param height Container height (-1 for wrap content)
     */
    void _CBMBannerAdSetContainerSize(const void * uniqueId, float width, float height){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetContainerSize: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setContainerSize:CGSizeMake(width, height)];
    }

#pragma mark - Alignment and Display

    /**
     * Gets the horizontal alignment of the ad within the container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return Integer value of CBMBannerHorizontalAlignment, or 0 on error
     */
    int _CBMBannerAdGetHorizontalAlignment(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetHorizontalAlignment: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return 0;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return (int)[bannerAdWrapper  horizontalAlignment];
    }

    /**
     * Sets the horizontal alignment of the ad within the container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param horizontalAlignment CBMBannerHorizontalAlignment enum value
     */
    void _CBMBannerAdSetHorizontalAlignment(const void* uniqueId, int horizontalAlignment){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetHorizontalAlignment: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setHorizontalAlignment:(CBMBannerHorizontalAlignment)horizontalAlignment];
    }

    /**
     * Gets the vertical alignment of the ad within the container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return Integer value of CBMBannerVerticalAlignment, or 0 on error
     */
    int _CBMBannerAdGetVerticalAlignment(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetVerticalAlignment: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return 0;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return (int)[bannerAdWrapper verticalAlignment];
    }

    /**
     * Sets the vertical alignment of the ad within the container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param verticalAlignment CBMBannerVerticalAlignment enum value
     */
    void _CBMBannerAdSetVerticalAlignment(const void* uniqueId, int verticalAlignment){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetVerticalAlignment: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setVerticalAlignment:(CBMBannerVerticalAlignment)verticalAlignment];
    }

    /**
     * Gets whether the banner is draggable.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return YES if draggable, NO otherwise
     */
    BOOL _CBMBannerAdGetDraggability(const void* uniqueId) {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetDraggability: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NO;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return [bannerAdWrapper draggable];
    }

    /**
     * Sets whether the banner is draggable.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param canDrag YES to enable dragging, NO to disable
     */
    void _CBMBannerAdSetDraggability(const void* uniqueId, BOOL canDrag){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetDraggability: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setDraggable:canDrag];
    }

    /**
     * Gets whether the banner is visible.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return YES if visible, NO otherwise
     */
    BOOL _CBMBannerAdGetVisibility(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetVisibility: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NO;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        return [bannerAdWrapper visible];
    }

    /**
     * Sets whether the banner is visible.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param visible YES to show banner, NO to hide
     */
    void _CBMBannerAdSetVisibility(const void* uniqueId, BOOL visible){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetVisibility: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setVisible:visible];
    }

#pragma mark - Ad Loading

    /**
     * Loads a banner ad with the specified configuration.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param placementName The ad placement identifier
     * @param sizeType Banner size type (0=standard, 1=medium, 2=leaderboard, 3=adaptive)
     * @param width Width for adaptive banners
     * @param height Max height for adaptive banners
     * @param hashCode Hash code for tracking this load request
     * @param adLoadResultCallback Callback function invoked when load completes
     */
    void _CBMBannerAdLoadAd(const void *uniqueId, const char *placementName, int sizeType, float width, float height, int hashCode, CBMExternBannerAdLoadResultEvent adLoadResultCallback) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMBannerAdLoadAd: Loading banner ad for placement: %s", placementName ? placementName : "null"]
                                                 logLevel:CBLLogLevelVerbose];

        // Validate parameters
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdLoadAd: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            if (adLoadResultCallback) {
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Banner ad wrapper is nil");
                adLoadResultCallback(hashCode, uniqueId, NULL, NULL, NULL, 0, 0, errorCode, errorMessage);
            }
            return;
        }

        if (!placementName) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdLoadAd: placementName is nil"
                                                     logLevel:CBLLogLevelError];
            if (adLoadResultCallback) {
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Placement name is nil");
                adLoadResultCallback(hashCode, uniqueId, NULL, NULL, NULL, 0, 0, errorCode, errorMessage);
            }
            return;
        }

        if (!adLoadResultCallback) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdLoadAd: adLoadResultCallback is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CBMBannerSize *size = GetBannerSize(sizeType, width, height);
        CBMBannerAdLoadRequest *loadRequest = [[CBMBannerAdLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) size:size];

        // Get Unity view controller
        UIViewController* unityViewController = GetAppController().rootViewController;
        if (!unityViewController) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdLoadAd: Unity root view controller is nil"
                                                     logLevel:CBLLogLevelError];
            const char *errorCode = toCStringOrNull(@"CM_-1");
            const char *errorMessage = toCStringOrNull(@"Unity root view controller is nil");
            adLoadResultCallback(hashCode, uniqueId, NULL, NULL, NULL, 0, 0, errorCode, errorMessage);
            return;
        }

        // Load banner ad
        [bannerAdWrapper loadWith:loadRequest viewController:unityViewController completion:^(CBMBannerAdLoadResult *adLoadResult) {
            if (!adLoadResult) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                              log:@"_CBMBannerAdLoadAd: adLoadResult is nil"
                                                         logLevel:CBLLogLevelError];
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Ad load result is nil");
                adLoadResultCallback(hashCode, uniqueId, NULL, NULL, NULL, 0, 0, errorCode, errorMessage);
                return;
            }

            CBMError *error = [adLoadResult error];
            if (error != nil) {
                CBMErrorCode codeInt = [error chartboostMediationCode];
                const char *code = toCStringOrNull([NSString stringWithFormat:errorFormat, codeInt]);
                const char *message = toCStringOrNull([error localizedDescription]);

                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                              log:[NSString stringWithFormat:@"_CBMBannerAdLoadAd: Failed with error %ld: %@", (long)codeInt, [error localizedDescription]]
                                                         logLevel:CBLLogLevelError];

                adLoadResultCallback(hashCode, uniqueId, NULL, NULL, NULL, 0, 0, code, message);
                return;
            }

            // Success - extract result data
            const char *loadId = toCStringOrNull([adLoadResult loadID]);
            const char *metricsJson = toJSON([adLoadResult metrics]);
            const char *winningBidInfoJson = toJSON([adLoadResult winningBidInfo]);
            float resultWidth = adLoadResult.size.size.width;
            float resultHeight = adLoadResult.size.size.height;

            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:[NSString stringWithFormat:@"_CBMBannerAdLoadAd: Successfully loaded banner ad (%.0fx%.0f)", resultWidth, resultHeight]
                                                     logLevel:CBLLogLevelVerbose];

            adLoadResultCallback(hashCode, uniqueId, loadId, metricsJson, winningBidInfoJson, resultWidth, resultHeight, NULL, NULL);
        }];
    }

#pragma mark - Lifecycle Management

    /**
     * Resets the banner ad to allow loading a new ad.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     */
    void _CBMBannerAdReset(const void* uniqueId){
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdReset: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                      log:@"_CBMBannerAdReset: Resetting banner ad"
                                                 logLevel:CBLLogLevelVerbose];

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper reset];
    }

    /**
     * Destroys the banner ad and releases resources.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     */
    void _CBMBannerAdDestroy(const void* uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdDestroy: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMBannerAdDestroy: Destroying banner ad %p", uniqueId]
                                                 logLevel:CBLLogLevelVerbose];

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper destroy];
        [[CBMAdStore sharedStore] releaseBannerAd:(__bridge void*)[bannerAdWrapper bannerView]];
    }
    
#pragma mark - Partner Ad Positioning

    /**
     * Sets the relative position of the partner ad within the container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param x X coordinate relative to container
     * @param y Y coordinate relative to container
     */
    void _CBMBannerAdSetAdRelativePosition(const void* uniqueId, float x, float y)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetAdRelativePosition: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setAdRelativePosition:CGPointMake(x, y)];
    }

    /**
     * Gets the relative position of the partner ad within the container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @return JSON string with x and y coordinates, or NULL on error
     */
    const char* _CBMBannerAdGetAdRelativePosition(const void* uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdGetAdRelativePosition: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        CGPoint position = [bannerAdWrapper adRelativePosition];
        const NSString * xKey = @"x";
        const NSString * yKey = @"y";
        NSString * xValue =  [NSString stringWithFormat:@"%f", position.x];
        NSString * yValue = [NSString stringWithFormat:@"%f", position.y];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:xValue, xKey, yValue, yKey, nil]);
    }

#pragma mark - Styling

    /**
     * Sets the background color of the banner container.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param r Red component (0-1)
     * @param g Green component (0-1)
     * @param b Blue component (0-1)
     * @param a Alpha component (0-1)
     */
    void _CBMBannerAdSetContainerBackgroundColor(const void* uniqueId, float r, float g, float b, float a)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetContainerBackgroundColor: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setContainerBackgroundColor:[UIColor colorWithRed:r green:g blue:b alpha:a]];
    }

    /**
     * Sets the background color of the partner ad view.
     *
     * @param uniqueId Bridge pointer to CBMBannerAdWrapper instance
     * @param r Red component (0-1)
     * @param g Green component (0-1)
     * @param b Blue component (0-1)
     * @param a Alpha component (0-1)
     */
    void _CBMBannerAdSetAdBackgroundColor(const void* uniqueId, float r, float g, float b, float a)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdBridgeTAG
                                                          log:@"_CBMBannerAdSetAdBackgroundColor: uniqueId is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMBannerAdWrapper *bannerAdWrapper = (__bridge CBMBannerAdWrapper*)uniqueId;
        [bannerAdWrapper setAdBackgroundColor:[UIColor colorWithRed:r green:g blue:b alpha:a]];
    }
}

