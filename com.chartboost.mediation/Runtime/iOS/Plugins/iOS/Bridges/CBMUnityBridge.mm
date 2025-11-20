enum CBCLogLevel : NSInteger;
#import "CBMDelegates.h"
#import "CBMUnityObserver.h"
#import "CBMAdStore.h"

NSString* const CBMUnityBridgeTAG = @"CBMUnityBridge";

#pragma mark - Extern Methods

extern "C" {

#pragma mark - SDK Information

    /**
     * Gets the Chartboost Mediation core module identifier.
     *
     * @return C string containing the core module ID, or NULL if unavailable
     */
    const char * _CBMCoreModuleId(){
        return toCStringOrNull([ChartboostMediation coreModuleID]);
    }

    /**
     * Gets the Chartboost Mediation SDK version.
     *
     * @return C string containing the SDK version, or NULL if unavailable
     */
    const char * _CBMGetVersion(){
        return toCStringOrNull([ChartboostMediation sdkVersion]);
    }

#pragma mark - Test Mode

    /**
     * Gets the current test mode state.
     *
     * @return true if test mode is enabled, false otherwise
     */
    bool _CBMGetTestMode(){
        bool testMode = [ChartboostMediation isTestModeEnabled];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"TestMode is %d", testMode]
                                                 logLevel:CBLLogLevelVerbose];
        return testMode;
    }

    /**
     * Sets the test mode state for Chartboost Mediation.
     *
     * @param isTestModeEnabled YES to enable test mode, NO to disable
     */
    void _CBMSetTestMode(BOOL isTestModeEnabled){
        [ChartboostMediation setIsTestModeEnabled:isTestModeEnabled];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"TestMode set to %d", isTestModeEnabled]
                                                 logLevel:CBLLogLevelVerbose];
    }

#pragma mark - Logging Configuration

    /**
     * Gets the current log level for Chartboost Mediation.
     *
     * @return Integer value of CBCLogLevel enum
     */
    int _CBMGetLogLevel(){
        int logLevel = (int)[ChartboostMediation logLevel];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"LogLevel is %d", logLevel]
                                                 logLevel:CBLLogLevelVerbose];
        return logLevel;
    }

    /**
     * Sets the log level for Chartboost Mediation.
     *
     * @param logLevel Integer value of CBCLogLevel enum
     */
    void _CBMSetLogLevel(int logLevel){
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"LogLevel set to %d", logLevel]
                                                 logLevel:CBLLogLevelVerbose];
        [ChartboostMediation setLogLevel:(CBCLogLevel)logLevel];
    }

#pragma mark - Ad Configuration

    /**
     * Gets whether oversized ads should be discarded.
     *
     * @return true if oversized ads are discarded, false otherwise
     */
    bool _CBMGetDiscardOverSizedAds(){
        bool discardOversizedAds = [ChartboostMediation discardOversizedAds];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"DiscardOversizedAds is %d", discardOversizedAds]
                                                 logLevel:CBLLogLevelVerbose];
        return discardOversizedAds;
    }

    /**
     * Sets whether oversized ads should be discarded.
     *
     * @param shouldDiscard YES to discard oversized ads, NO to allow them
     */
    void _CBMSetDiscardOverSizedAds(BOOL shouldDiscard){
        [ChartboostMediation setDiscardOversizedAds:shouldDiscard];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"DiscardOversizedAds set to %d", shouldDiscard]
                                                 logLevel:CBLLogLevelVerbose];
    }

#pragma mark - Adapter Information

    /**
     * Gets information about all initialized mediation adapters.
     *
     * @return JSON string containing array of adapter info objects, or NULL if unavailable
     */
    const char * _CBMGetAdaptersInfo(){
        NSMutableArray * jsonArray = [NSMutableArray array];

        const NSString * partnerVersionKey = @"partnerVersion";
        const NSString * adapterVersionKey = @"adapterVersion";
        const NSString * partnerIdentifierKey = @"partnerIdentifier";
        const NSString * partnerDisplayNameKey = @"partnerDisplayName";

        NSArray<CBMPartnerAdapterInfo*> * adapters = [ChartboostMediation initializedAdapterInfo];

        if (!adapters) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                          log:@"_CBMGetAdaptersInfo: No adapters available"
                                                     logLevel:CBLLogLevelWarning];
            return toJSON(jsonArray);
        }

        for (CBMPartnerAdapterInfo *adapter in adapters) {
            if (!adapter) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                              log:@"_CBMGetAdaptersInfo: Encountered nil adapter in array"
                                                         logLevel:CBLLogLevelWarning];
                continue;
            }

            NSString * partnerVersionValue = adapter.partnerVersion;
            NSString * adapterVersionValue = adapter.adapterVersion;
            NSString * partnerIdentifierValue = adapter.partnerID;
            NSString * partnerDisplayNameValue = adapter.partnerDisplayName;

            NSDictionary *adapterDictionary = [NSDictionary dictionaryWithObjectsAndKeys:
                                               partnerVersionValue, partnerVersionKey,
                                               adapterVersionValue, adapterVersionKey,
                                               partnerIdentifierValue, partnerIdentifierKey,
                                               partnerDisplayNameValue, partnerDisplayNameKey,
                                               nil];

            [jsonArray addObject:adapterDictionary];
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMGetAdaptersInfo: Parsed %ld adapters info", [adapters count]]
                                                 logLevel:CBLLogLevelVerbose];
        return toJSON(jsonArray);
    }

#pragma mark - Initialization Configuration

    /**
     * Sets the pre-initialization configuration for Chartboost Mediation.
     *
     * @param skippedPartnerIds Array of partner IDs to skip during initialization
     * @param skippedPartnerIdsSize Number of partner IDs in the array
     * @return JSON string containing error information if failed, or NULL if successful
     */
    const char * _CMBSetPreInitializationConfiguration(const char** skippedPartnerIds, int skippedPartnerIdsSize){
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CMBSetPreInitializationConfiguration: Configuring with %d skipped partners", skippedPartnerIdsSize]
                                                 logLevel:CBLLogLevelVerbose];

        // Validate parameters
        if (skippedPartnerIdsSize > 0 && !skippedPartnerIds) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                          log:@"_CMBSetPreInitializationConfiguration: skippedPartnerIds is NULL but size is > 0"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        if (skippedPartnerIdsSize <= 0) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                          log:@"_CMBSetPreInitializationConfiguration: No partners to skip"
                                                     logLevel:CBLLogLevelVerbose];
            return NULL;
        }

        CBMPreinitializationConfiguration* mediationPreinitializationConfiguration = nil;

        // Create configuration with skipped partner IDs
        mediationPreinitializationConfiguration = [[CBMPreinitializationConfiguration alloc] initWithSkippedPartnerIDs:toNSMutableArray(skippedPartnerIds, skippedPartnerIdsSize)];

        if (!mediationPreinitializationConfiguration) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                          log:@"_CMBSetPreInitializationConfiguration: Failed to create configuration"
                                                     logLevel:CBLLogLevelError];
            return NULL;
        }

        // Apply configuration
        CBMError* _Nullable error = [ChartboostMediation setPreinitializationConfiguration:mediationPreinitializationConfiguration];

        if (error == nil) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                          log:@"_CMBSetPreInitializationConfiguration: Successfully set configuration"
                                                     logLevel:CBLLogLevelVerbose];
            return NULL;
        }

        // Handle error
        CBMErrorCode codeInt = [error chartboostMediationCode];
        NSString * keyCode = @"code";
        NSString * keyMessage = @"message";
        NSString *code = [NSString stringWithFormat:@"CM_%ld", codeInt];
        NSString *message = [error localizedDescription];
        NSDictionary * errorDictionary = [NSDictionary dictionaryWithObjectsAndKeys:code, keyCode, message, keyMessage, nil];

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CMBSetPreInitializationConfiguration: Failed with error code %ld: %@", codeInt, message]
                                                 logLevel:CBLLogLevelError];

        return toJSON(errorDictionary);
    }

#pragma mark - UI Scale Factor

    /**
     * Gets the UI scale factor (display density) for the current device.
     *
     * This value represents the device's screen scale and is used to convert between
     * points and pixels. On modern iOS devices, this is typically 2.0 (Retina) or 3.0 (Retina HD).
     *
     * The implementation handles the deprecation of UIScreen.main in iOS 16+ by using
     * UIWindowScene when available (iOS 17+), falling back to UIScreen.mainScreen for
     * older iOS versions.
     *
     * @return The screen scale factor (e.g., 1.0, 2.0, or 3.0)
     */
    float _CBMGetUIScaleFactor() {
        // `UIScreen.main` was deprecated in iOS 16. Apple documentation:
        //   https://developer.apple.com/documentation/uikit/uiscreen/1617815-main
        // Since `UIScreen.main` has been working correctly at least up to iOS 16, the custom
        // implementation only targets iOS 17+, not iOS 13+.
        if (@available(iOS 17.0, *)) {
            NSSet<UIScene*> *connectedScenes = UIApplication.sharedApplication.connectedScenes;

            // Check scenes in priority order: ForegroundActive > ForegroundInactive > Background > Unattached
            NSArray *activationStates = @[
                @(UISceneActivationStateForegroundActive),
                @(UISceneActivationStateForegroundInactive),
                @(UISceneActivationStateBackground),
                @(UISceneActivationStateUnattached)
            ];

            for (NSNumber *activationState in activationStates) {
                UISceneActivationState state = (UISceneActivationState)activationState.integerValue;
                for (UIScene* connectedScene in connectedScenes) {
                    if ([connectedScene isKindOfClass:[UIWindowScene class]]) {
                        UIWindowScene *windowScene = (UIWindowScene*)connectedScene;
                        if (windowScene.activationState == state) {
                            float scale = windowScene.screen.scale;
                            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                                          log:[NSString stringWithFormat:@"_CBMGetUIScaleFactor: Returning scale %.1f from windowScene", scale]
                                                                     logLevel:CBLLogLevelVerbose];
                            return scale;
                        }
                    }
                }
            }
        }

        // Fallback for iOS 16 and earlier
        float scale = UIScreen.mainScreen.scale;
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:[NSString stringWithFormat:@"_CBMGetUIScaleFactor: Returning scale %.1f from mainScreen (fallback)", scale]
                                                 logLevel:CBLLogLevelVerbose];
        return scale;
    }

#pragma mark - Lifecycle Management

    /**
     * Cleans up resources during application shutdown.
     *
     * This method releases all tracked ads from the ad store to prevent memory leaks.
     * Should be called when the application is quitting.
     */
    void _CBMCleanup(){
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:@"_CBMCleanup: Cleaning up resources"
                                                 logLevel:CBLLogLevelVerbose];
        [[CBMAdStore sharedStore] clearAll];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG
                                                      log:@"_CBMCleanup: Cleanup completed"
                                                 logLevel:CBLLogLevelVerbose];
    }
}
