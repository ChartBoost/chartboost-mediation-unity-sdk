enum CBCLogLevel : NSInteger;
#import "CBMDelegates.h"
#import "CBMUnityObserver.h"

extern "C" {

    void _CBMSetLifeCycleCallbacks(CBMExternDataEvent didReceivePartnerInitializationData, CBMExternDataEvent didReceiveImpressionLevelRevenueData){
        [[CBMUnityObserver sharedObserver] setDidReceivePartnerInitializationData:didReceivePartnerInitializationData];
        [[CBMUnityObserver sharedObserver] setDidReceiveImpressionLevelRevenueData:didReceiveImpressionLevelRevenueData];
        [[CBMUnityObserver sharedObserver] subscribeNotificationObservers];
    }

    const char * _CBMCoreModuleId(){
        return toCStringOrNull([ChartboostMediation coreModuleID]);
    }

    const char * _CBMGetVersion(){
        return toCStringOrNull([ChartboostMediation sdkVersion]);
    }

    bool _CBMGetTestMode() {
        return [ChartboostMediation isTestModeEnabled];
    }

    void _CBMSetTestMode(BOOL isTestModeEnabled)
    {
        [ChartboostMediation setIsTestModeEnabled:isTestModeEnabled];
    }

    int _CBMGetLogLevel(){
        return (int)[ChartboostMediation logLevel];
    }

    void _CBMSetLogLevel(int logLevel)
    {
        [ChartboostMediation setLogLevel:(CBCLogLevel)logLevel];
    }

    bool _CBMGetDiscardOverSizedAds(){
        return [ChartboostMediation discardOversizedAds];
    }

    void _CBMSetDiscardOverSizedAds(BOOL shouldDiscard)
    {
        [ChartboostMediation  setDiscardOversizedAds:shouldDiscard];
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
                 return toJSON(errorDictionary);
             }
         }

        return NULL;
    }
}
