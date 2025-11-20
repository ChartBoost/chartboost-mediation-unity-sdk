#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"

NSString* const CBMFullscreenAdQueueBridgeTAG = @"CBMFullscreenAdQueueBridge";

#pragma mark - Utility Methods

/**
 * Retrieves a fullscreen ad queue from a bridge pointer.
 *
 * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
 * @return CBMFullscreenAdQueue instance, or nil if uniqueId is NULL
 */
static CBMFullscreenAdQueue * GetFullScreenAdQueue(const void * uniqueId){
    if (!uniqueId) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                      log:@"GetFullScreenAdQueue: uniqueId is NULL"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }
    return (__bridge CBMFullscreenAdQueue*)uniqueId;
}

#pragma mark - Extern Methods

extern "C" {

#pragma mark - Lifecycle

    /**
     * Sets the callback functions for fullscreen ad queue events.
     *
     * @param fullscreenAdQueueUpdateEvent Function pointer to handle queue update events
     * @param fullscreenAdQueueRemoveExpiredAdEvent Function pointer to handle expired ad removal events
     */
    void _CBMFullscreenAdQueueSetCallbacks(CBMExternFullscreenAdQueueUpdateEvent fullscreenAdQueueUpdateEvent, CBMExternFullscreenAdQueueRemoveExpiredAdEvent fullscreenAdQueueRemoveExpiredAdEvent){
        if (!fullscreenAdQueueUpdateEvent) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueSetCallbacks: fullscreenAdQueueUpdateEvent callback is nil"
                                                     logLevel:CBLLogLevelError];
        }

        if (!fullscreenAdQueueRemoveExpiredAdEvent) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueSetCallbacks: fullscreenAdQueueRemoveExpiredAdEvent callback is nil"
                                                     logLevel:CBLLogLevelError];
        }

        if (fullscreenAdQueueUpdateEvent) {
            [[CBMUnityObserver sharedObserver] setFullscreenAdQueueUpdateEvent:fullscreenAdQueueUpdateEvent];
        }

        if (fullscreenAdQueueRemoveExpiredAdEvent) {
            [[CBMUnityObserver sharedObserver] setFullscreenAdQueueRemoveExpiredAdEvent:fullscreenAdQueueRemoveExpiredAdEvent];
        }
    }

    /**
     * Gets or creates a fullscreen ad queue for the specified placement.
     *
     * @param placementName The placement name for the ad queue
     * @return Bridge pointer to CBMFullscreenAdQueue instance, or NULL on error
     */
    void * _CBMFullscreenAdQueueGetQueue(const char * placementName)
    {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMFullscreenAdQueueGetQueue: Getting queue for placement: %s", placementName ? placementName : "null"]
                                                 logLevel:CBLLogLevelVerbose];

        if (!placementName) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetQueue: placementName is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = [CBMFullscreenAdQueue queueForPlacement:toNSStringOrEmpty(placementName)];

        if (!fullScreenAdQueue) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetQueue: Failed to create queue"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        // Note: This will reset the delegate on the queue with same placement name
        [fullScreenAdQueue setDelegate:[CBMUnityObserver sharedObserver]];

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                      log:@"_CBMFullscreenAdQueueGetQueue: Successfully created/retrieved queue"
                                                 logLevel:CBLLogLevelVerbose];

        return (__bridge void *)fullScreenAdQueue;
    }

#pragma mark - Keywords

    /**
     * Gets the keywords associated with this ad queue.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @return JSON string with keywords, or NULL on error
     */
    const char * _CBMFullscreenAdQueueGetKeywords(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetKeywords: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return NULL;
        }

        return toJSON([fullScreenAdQueue keywords]);
    }

    /**
     * Sets keywords for targeting ads in this queue.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @param keywordsJson JSON string representing keywords dictionary
     */
    void _CBMFullscreenAdQueueSetKeywords(const void * uniqueId, const char * keywordsJson)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueSetKeywords: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return;
        }

        NSMutableDictionary *formattedKeywords = toObjectFromJson(keywordsJson);
        [fullScreenAdQueue setKeywords:formattedKeywords];
    }

#pragma mark - Queue Properties

    /**
     * Gets the maximum capacity of the ad queue.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @return Queue capacity, or 0 on error
     */
    int _CBMFullscreenAdQueueGetQueueCapacity(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetQueueCapacity: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return 0;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return 0;
        }

        return (int)[fullScreenAdQueue queueCapacity];
    }

    /**
     * Gets the number of ads currently ready in the queue.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @return Number of ready ads, or 0 on error
     */
    int _CBMFullscreenAdQueueGetNumberOfAdsReady(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetNumberOfAdsReady: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return 0;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return 0;
        }

        return (int)[fullScreenAdQueue numberOfAdsReady];
    }

    /**
     * Checks if the ad queue is currently running.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @return true if running, false otherwise
     */
    bool _CBMFullscreenAdQueueIsRunning(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueIsRunning: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return false;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return false;
        }

        return [fullScreenAdQueue isRunning];
    }

    /**
     * Sets the maximum capacity of the ad queue.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @param capacity New queue capacity
     */
    void _CBMFullscreenAdQueueSetCapacity(const void * uniqueId, int capacity)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueSetCapacity: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return;
        }

        [fullScreenAdQueue setQueueCapacity:capacity];
    }

#pragma mark - Ad Retrieval

    /**
     * Gets the next ad from the queue.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @return Bridge pointer to CBMFullscreenAd instance, or NULL if no ad available
     */
    void * _CBMFullscreenAdQueueGetNextAd(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetNextAd: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return NULL;
        }

        CBMFullscreenAd* ad = [fullScreenAdQueue getNextAd];
        if (!ad) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueGetNextAd: No ad available in queue"
                                                     logLevel:CBLLogLevelVerbose];
            return NULL;
        }

        [ad setDelegate:[CBMUnityObserver sharedObserver]];
        [[CBMAdStore sharedStore] trackFullscreenAd:ad];

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                      log:@"_CBMFullscreenAdQueueGetNextAd: Successfully retrieved ad from queue"
                                                 logLevel:CBLLogLevelVerbose];

        return (__bridge void*)ad;
    }

    /**
     * Checks if the queue has a next ad available.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     * @return true if ad is available, false otherwise
     */
    bool _CBMFullscreenAdQueueHasNextAd(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueHasNextAd: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return false;
        }

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return false;
        }

        return [fullScreenAdQueue hasNextAd];
    }

#pragma mark - Queue Control

    /**
     * Starts the ad queue to begin loading ads.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     */
    void _CBMFullscreenAdQueueStart(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueStart: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                      log:@"_CBMFullscreenAdQueueStart: Starting ad queue"
                                                 logLevel:CBLLogLevelVerbose];

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return;
        }

        [fullScreenAdQueue start];
    }

    /**
     * Stops the ad queue from loading more ads.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAdQueue instance
     */
    void _CBMFullscreenAdQueueStop(const void * uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                          log:@"_CBMFullscreenAdQueueStop: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdQueueBridgeTAG
                                                      log:@"_CBMFullscreenAdQueueStop: Stopping ad queue"
                                                 logLevel:CBLLogLevelVerbose];

        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        if (!fullScreenAdQueue) {
            return;
        }

        [fullScreenAdQueue stop];
    }
}
