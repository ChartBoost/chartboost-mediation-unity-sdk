enum CBCLogLevel : NSInteger;
#import "CBMDelegates.h"
#import "CBMUnityObserver.h"

NSString* const CBMUnityBridgeTAG = @"CBMUnityBridge";

extern "C" {
    const char * _CBMCoreModuleId(){
        return toCStringOrNull([ChartboostMediation coreModuleID]);
    }

    const char * _CBMGetVersion(){
        return toCStringOrNull([ChartboostMediation sdkVersion]);
    }

    bool _CBMGetTestMode(){
        bool testMode = [ChartboostMediation isTestModeEnabled];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"TestMode is %d", testMode] logLevel:CBLLogLevelDebug];
        return testMode;
    }

    void _CBMSetTestMode(BOOL isTestModeEnabled){
        [ChartboostMediation setIsTestModeEnabled:isTestModeEnabled];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"TestMode set to %d", isTestModeEnabled] logLevel:CBLLogLevelDebug];
    }

    int _CBMGetLogLevel(){
        int logLevel = (int)[ChartboostMediation logLevel];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"LogLevel is %d", logLevel] logLevel:CBLLogLevelDebug];
        return logLevel;
    }

    void _CBMSetLogLevel(int logLevel){
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"LogLevel set to %d", logLevel] logLevel:CBLLogLevelDebug];
        [ChartboostMediation setLogLevel:(CBCLogLevel)logLevel];
    }

    bool _CBMGetDiscardOverSizedAds(){
        bool discardOversizedAds = [ChartboostMediation discardOversizedAds];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"DiscardOversizedAds is %d", discardOversizedAds] logLevel:CBLLogLevelDebug];
        return discardOversizedAds;
    }

    void _CBMSetDiscardOverSizedAds(BOOL shouldDiscard){
        [ChartboostMediation  setDiscardOversizedAds:shouldDiscard];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"DiscardOversizedAds set to %d", shouldDiscard] logLevel:CBLLogLevelDebug];
    }

    const char * _CBMGetAdaptersInfo(){
        NSMutableArray * jsonArray = [NSMutableArray array];

        const NSString * partnerVersionKey = @"partnerVersion";
        const NSString * adapterVersionKey = @"adapterVersion";
        const NSString * partnerIdentifierKey = @"partnerIdentifier";
        const NSString * partnerDisplayNameKey = @"partnerDisplayName";

        NSArray<CBMPartnerAdapterInfo*> * adapters =  [ChartboostMediation initializedAdapterInfo];
        for (CBMPartnerAdapterInfo *adapter in adapters) {
            NSString * partnerVersionValue = adapter.partnerVersion;
            NSString * adapterVersionValue = adapter.adapterVersion;
            NSString * partnerIdentifierValue = adapter.partnerID;
            NSString * partnerDisplayNameValue = adapter.partnerDisplayName;

            NSDictionary *adapterDictionary = [NSDictionary dictionaryWithObjectsAndKeys:partnerVersionValue,partnerVersionKey, adapterVersionValue,adapterVersionKey,partnerIdentifierValue,partnerIdentifierKey,partnerDisplayNameValue,partnerDisplayNameKey, nil];

            [jsonArray addObject:adapterDictionary];
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"Parsed %ld adapters info", [adapters count]] logLevel:CBLLogLevelDebug];
        return toJSON(jsonArray);
    }

    const char * _CMBSetPreInitializationConfiguration(const char** skippedPartnerIds, int skippedPartnerIdsSize){
        CBMPreinitializationConfiguration* mediationPreinitializationConfiguration = nil;

         if (skippedPartnerIdsSize > 0) {
             mediationPreinitializationConfiguration = [[CBMPreinitializationConfiguration alloc] initWithSkippedPartnerIDs:toNSMutableArray(skippedPartnerIds, skippedPartnerIdsSize)];
             CBMError* _Nullable error  = [ChartboostMediation setPreinitializationConfiguration:mediationPreinitializationConfiguration];

             if (error == nil)
                 return NULL;
             else
             {
                 CBMErrorCode codeInt = [error chartboostMediationCode];
                 NSString * keyCode = @"code";
                 NSString * keyMessage = @"message";
                 NSString *code = [NSString stringWithFormat:@"CM_%ld", codeInt];
                 NSString *message = [error localizedDescription];
                 NSDictionary * errorDictionary = [NSDictionary dictionaryWithObjectsAndKeys:code, keyCode, message, keyMessage, nil];
                 [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityBridgeTAG log:[NSString stringWithFormat:@"Failed to set PreInitializationConfiguration with error code %ld", codeInt] logLevel:CBLLogLevelDebug];
                 return toJSON(errorDictionary);
             }
         }

        return NULL;
    }
}
