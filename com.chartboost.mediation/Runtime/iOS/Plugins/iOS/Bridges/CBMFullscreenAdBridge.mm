#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"

NSString* const CBMFullscreenAdBridgeTAG = @"CBMFullscreenAdBridge";
NSString* const CBMFullscreenErrorFormat = @"CM_%ld";

#pragma mark - Utility Methods

/**
 * Retrieves a fullscreen ad from a bridge pointer.
 *
 * @param uniqueId Bridge pointer to CBMFullscreenAd instance
 * @return CBMFullscreenAd instance, or nil if uniqueId is NULL
 */
static CBMFullscreenAd * GetFullscreenAd(const void* uniqueId){
    if (!uniqueId) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                      log:@"GetFullscreenAd: uniqueId is NULL"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }
    return (__bridge CBMFullscreenAd*)uniqueId;
}

#pragma mark - Extern Methods

extern "C" {

#pragma mark - Lifecycle

    /**
     * Sets the callback function for fullscreen ad events.
     *
     * @param fullscreenAdEvents Function pointer to handle fullscreen ad events from Unity
     */
    void _CBMFullscreenAdSetCallbacks(CBMExternFullscreenAdEvent fullscreenAdEvents){
        if (!fullscreenAdEvents) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdSetCallbacks: fullscreenAdEvents callback is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }
        [[CBMUnityObserver sharedObserver] setDidReceiveFullscreenAdEvent:fullscreenAdEvents];
    }

#pragma mark - Ad Properties

    /**
     * Gets the ad load request information.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     * @return JSON string with placement name and keywords, or NULL on error
     */
    const char * _CBMFullscreenAdGetRequest(const void* uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdGetRequest: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        if (!ad) {
            return NULL;
        }

        const NSString * placementKey = @"placementName";
        const NSString * keywordsKey = @"keywords";
        NSString * placementValue = [[ad request] placement];
        NSDictionary * keywordsValue = [[ad request] keywords];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:placementValue, placementKey, keywordsValue, keywordsKey, nil]);
    }

    /**
     * Gets the unique identifier for the current ad load.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     * @return C string with load ID, or NULL if not loaded
     */
    const char * _CBMFullscreenAdGetLoadId(const void* uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdGetLoadId: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        if (!ad) {
            return NULL;
        }

        return toCStringOrNull([ad loadID]);
    }

    /**
     * Gets the custom data associated with this fullscreen ad.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     * @return C string with custom data, or NULL if none set
     */
    const char * _CBMFullscreenAdGetCustomData(const void* uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdGetCustomData: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        if (!ad) {
            return NULL;
        }

        return toCStringOrNull([ad customData]);
    }

    /**
     * Sets custom data for this fullscreen ad.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     * @param customData C string containing custom data to associate with the ad
     */
    void _CBMFullscreenAdSetCustomData(const void* uniqueId, const char* customData)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdSetCustomData: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        if (!ad) {
            return;
        }

        [ad setCustomData:toNSStringOrNull(customData)];
    }

    /**
     * Gets the winning bid information from the ad auction.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     * @return JSON string with winning bid info, or NULL if no bid info available
     */
    const char * _CBMFullscreenAdGetBidInfo(const void *uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdGetBidInfo: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        if (!ad) {
            return NULL;
        }

        return toJSON([ad winningBidInfo]);
    }

#pragma mark - Ad Loading

    /**
     * Loads a fullscreen ad asynchronously.
     *
     * @param placementName The ad placement identifier
     * @param keywordsJson JSON string representing keywords dictionary for targeting
     * @param hashCode Hash code for tracking this load request
     * @param callback Callback function invoked when load completes
     */
    void _CBMLoadFullscreenAd(const char *placementName, const char *keywordsJson, int hashCode, CBMExternFullscreenAdLoadResultEvent callback)
    {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMLoadFullscreenAd: Loading fullscreen ad for placement: %s", placementName ? placementName : "null"]
                                                 logLevel:CBLLogLevelVerbose];

        // Validate parameters
        if (!placementName) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMLoadFullscreenAd: placementName is NULL"
                                                     logLevel:CBLLogLevelError];
            if (callback) {
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Placement name is NULL");
                callback(hashCode, NULL, NULL, NULL, NULL, errorCode, errorMessage);
            }
            return;
        }

        if (!callback) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMLoadFullscreenAd: callback is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        NSDictionary *keywords = toObjectFromJson(keywordsJson);
        CBMFullscreenAdLoadRequest *loadRequest = [[CBMFullscreenAdLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) keywords:keywords];

        if (!loadRequest) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMLoadFullscreenAd: Failed to create load request"
                                                     logLevel:CBLLogLevelError];
            const char *errorCode = toCStringOrNull(@"CM_-1");
            const char *errorMessage = toCStringOrNull(@"Failed to create load request");
            callback(hashCode, NULL, NULL, NULL, NULL, errorCode, errorMessage);
            return;
        }

        [CBMFullscreenAd loadWith:loadRequest completion:^(CBMFullscreenAdLoadResult * adLoadResult) {
            if (!adLoadResult) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                              log:@"_CBMLoadFullscreenAd: adLoadResult is nil"
                                                         logLevel:CBLLogLevelError];
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Ad load result is nil");
                callback(hashCode, NULL, NULL, NULL, NULL, errorCode, errorMessage);
                return;
            }

            CBMError *error = [adLoadResult error];
            if (error != nil) {
                CBMErrorCode codeInt = [error chartboostMediationCode];
                const char *code = toCStringOrNull([NSString stringWithFormat:CBMFullscreenErrorFormat, codeInt]);
                const char *message = toCStringOrNull([error localizedDescription]);

                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                              log:[NSString stringWithFormat:@"_CBMLoadFullscreenAd: Failed with error %ld: %@", (long)codeInt, [error localizedDescription]]
                                                         logLevel:CBLLogLevelError];

                callback(hashCode, NULL, NULL, NULL, NULL, code, message);
                return;
            }

            // Success - extract result data
            CBMFullscreenAd * ad = [adLoadResult ad];
            if (!ad) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                              log:@"_CBMLoadFullscreenAd: Loaded ad is nil"
                                                         logLevel:CBLLogLevelError];
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Loaded ad is nil");
                callback(hashCode, NULL, NULL, NULL, NULL, errorCode, errorMessage);
                return;
            }

            [[CBMAdStore sharedStore] trackFullscreenAd:ad];

            const char * loadId = toCStringOrNull([adLoadResult loadID]);
            const char * metricsJson = toJSON([adLoadResult metrics]);
            const char * winningBidJson = toJSON([ad winningBidInfo]);

            [ad setDelegate:[CBMUnityObserver sharedObserver]];

            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMLoadFullscreenAd: Successfully loaded fullscreen ad"
                                                     logLevel:CBLLogLevelVerbose];

            callback(hashCode, (__bridge void*)ad, loadId, metricsJson, winningBidJson, NULL, NULL);
        }];
    }


#pragma mark - Ad Display

    /**
     * Shows a fullscreen ad on the main thread.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     * @param hashCode Hash code for tracking this show request
     * @param callback Callback function invoked when show operation completes
     */
    void _CBMFullscreenAdShow(const void *uniqueId, int hashCode, CBMExternFullscreenAdShowResultEvent callback)
    {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                      log:@"_CBMFullscreenAdShow: Showing fullscreen ad"
                                                 logLevel:CBLLogLevelVerbose];

        // Validate parameters
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdShow: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            if (callback) {
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Fullscreen ad reference is NULL");
                callback(hashCode, NULL, errorCode, errorMessage);
            }
            return;
        }

        if (!callback) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdShow: callback is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        toMain(^{
            CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
            if (!ad) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                              log:@"_CBMFullscreenAdShow: Failed to get fullscreen ad"
                                                         logLevel:CBLLogLevelError];
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Failed to get fullscreen ad");
                callback(hashCode, NULL, errorCode, errorMessage);
                return;
            }

            UIViewController *viewController = UnityGetGLViewController();
            if (!viewController) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                              log:@"_CBMFullscreenAdShow: Unity view controller is nil"
                                                         logLevel:CBLLogLevelError];
                const char *errorCode = toCStringOrNull(@"CM_-1");
                const char *errorMessage = toCStringOrNull(@"Unity view controller is nil");
                callback(hashCode, NULL, errorCode, errorMessage);
                return;
            }

            [ad showWith:viewController completion:^(CBMAdShowResult *adShowResult) {
                if (!adShowResult) {
                    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                                  log:@"_CBMFullscreenAdShow: adShowResult is nil"
                                                             logLevel:CBLLogLevelError];
                    const char *errorCode = toCStringOrNull(@"CM_-1");
                    const char *errorMessage = toCStringOrNull(@"Ad show result is nil");
                    callback(hashCode, NULL, errorCode, errorMessage);
                    return;
                }

                CBMError *error = [adShowResult error];
                if (error != nil) {
                    CBMErrorCode codeInt = [error chartboostMediationCode];
                    const char *code = toCStringOrNull([NSString stringWithFormat:CBMFullscreenErrorFormat, codeInt]);
                    const char *message = toCStringOrNull([error localizedDescription]);

                    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                                  log:[NSString stringWithFormat:@"_CBMFullscreenAdShow: Failed with error %ld: %@", (long)codeInt, [error localizedDescription]]
                                                             logLevel:CBLLogLevelError];

                    callback(hashCode, NULL, code, message);
                    return;
                }

                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                              log:@"_CBMFullscreenAdShow: Successfully showed fullscreen ad"
                                                         logLevel:CBLLogLevelVerbose];

                UnityPause(true);
                callback(hashCode, toJSON([adShowResult metrics]), NULL, NULL);
            }];
        });
    }

#pragma mark - Lifecycle Management

    /**
     * Invalidates and releases a fullscreen ad.
     *
     * @param uniqueId Bridge pointer to CBMFullscreenAd instance
     */
    void _CBMFullscreenAdInvalidate(const void *uniqueId)
    {
        if (!uniqueId) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                          log:@"_CBMFullscreenAdInvalidate: uniqueId is NULL"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMFullscreenAdBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMFullscreenAdInvalidate: Invalidating fullscreen ad %p", uniqueId]
                                                 logLevel:CBLLogLevelVerbose];

        toMain(^() {
            CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
            if (ad) {
                [ad invalidate];
            }
            [[CBMAdStore sharedStore] releaseFullscreenAd:uniqueId];
        });
    }
}
