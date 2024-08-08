#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"

static CBMFullscreenAd * GetFullscreenAd(const void* uniqueId)
{
    return (__bridge CBMFullscreenAd*)uniqueId;
}

template <typename TObj>
TObj toObjectFromJson(const char* jsonString) {
    NSData* jsonData = [[NSString stringWithUTF8String:jsonString] dataUsingEncoding:NSUTF8StringEncoding];
    NSError* error = nil;
    TObj arr = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];

    if (error != nil)
        return nil;

    return arr;
}

#pragma mark Extern Methods
extern "C" {
    void _CBMFullscreenAdSetCallbacks(CBMExternFullscreenAdEvent fullscreenAdEvents){
        [[CBMUnityObserver sharedObserver] setDidReceiveFullscreenAdEvent:fullscreenAdEvents];
    }

    const char * _CBMFullscreenAdGetRequest(const void* uniqueId)
    {
        const NSString * placementKey = @"placementName";
        const NSString * keywordsKey = @"keywords";
        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        NSString * placemnentValue = [[ad request] placement];
        NSDictionary * keywordsValue = [[ad request] keywords];
        return toJSON([NSDictionary dictionaryWithObjectsAndKeys:placemnentValue,placementKey,keywordsValue,keywordsKey, nil]);
    }

    const char * _CBMFullscreenAdGetLoadId(const void* uniqueId)
    {
        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        return toCStringOrNull([ad loadID]);
    }

    const char * _CBMFullscreenAdGetCustomData(const void* uniqueId)
    {
        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        return toCStringOrNull([ad customData]);
    }

    void _CBMFullscreenAdSetCustomData(const void* uniqueId, const char* customData)
    {
        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        [ad setCustomData:toNSStringOrNull(customData)];
    }

    const char * _CBMFullscreenAdGetBidInfo(const void *uniqueId)
    {
        CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
        return toJSON([ad winningBidInfo]);
    }

    void _CBMLoadFullscreenAd(const char *placementName, const char *keywordsJson, int hashCode, CBMExternFullscreenAdLoadResultEvent callback)
    {
        NSDictionary *keywords = toObjectFromJson<NSDictionary *>(keywordsJson);

        CBMFullscreenAdLoadRequest *loadRequest = [[CBMFullscreenAdLoadRequest alloc] initWithPlacement:toNSStringOrEmpty(placementName) keywords:keywords];

        [CBMFullscreenAd loadWith:loadRequest completion:^(CBMFullscreenAdLoadResult * adLoadResult) {
            CBMError *error = [adLoadResult error];
            if (error != nil)
            {
                NSString * errorFormat = @"CM_%ld";
                CBMErrorCode codeInt = [error chartboostMediationCode];
                const char *code = toCStringOrNull([NSString stringWithFormat:errorFormat, codeInt]);
                const char *message = toCStringOrNull([error localizedDescription]);
                callback(hashCode, NULL, NULL, NULL, NULL, code, message);
                return;
            }

            CBMFullscreenAd * ad = [adLoadResult ad];
            [[CBMAdStore sharedStore] storeAd:ad];

            const char * loadId = toCStringOrNull([adLoadResult loadID]);
            const char * metricsJson = toJSON([adLoadResult metrics]);
            const char * winningBidJson = toJSON([ad winningBidInfo]);

            [ad setDelegate:[CBMUnityObserver sharedObserver]];
            callback(hashCode, (__bridge void*)ad, loadId, metricsJson, winningBidJson, NULL, NULL);
        }];
    }


    void _CBMFullscreenAdShow(const void *uniqueId, int hashCode, CBMExternFullscreenAdShowResultEvent callback)
    {
        toMain(^{
            CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
            [ad showWith:UnityGetGLViewController() completion:^(CBMAdShowResult *adShowResult) {
                CBMError *error = [adShowResult error];
                if (error != nil)
                {
                    NSString * errorFormat = @"CM_%ld";
                    CBMErrorCode codeInt = [error chartboostMediationCode];
                    const char *code = toCStringOrNull([NSString stringWithFormat:errorFormat, codeInt]);
                    const char *message = toCStringOrNull([error localizedDescription]);
                    callback(hashCode, NULL, code, message);
                    return;
                }

                UnityPause(true);
                callback(hashCode, toJSON([adShowResult metrics]), NULL, NULL);
            }];
        });
    }

    void _CBMFullscreenAdInvalidate(const void *uniqueId)
    {
        toMain(^() {
            CBMFullscreenAd * ad = GetFullscreenAd(uniqueId);
            [ad invalidate];
            [[CBMAdStore sharedStore] releaseAd:uniqueId];
        });
    }
}
