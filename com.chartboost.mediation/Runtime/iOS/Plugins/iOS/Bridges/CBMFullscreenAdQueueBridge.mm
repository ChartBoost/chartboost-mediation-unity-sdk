#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"

static CBMFullscreenAdQueue * GetFullScreenAdQueue(const void * uniqueId){
    return (__bridge CBMFullscreenAdQueue*)uniqueId;
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
    void _CBMFullscreenAdQueueSetCallbacks(CBMExternFullscreenAdQueueUpdateEvent fullscreenAdQueueUpdateEvent, CBMExternFullscreenAdQueueRemoveExpiredAdEvent fullscreenAdQueueRemoveExpiredAdEvent){
        [[CBMUnityObserver sharedObserver] setFullscreenAdQueueUpdateEvent:fullscreenAdQueueUpdateEvent];
        [[CBMUnityObserver sharedObserver] setFullscreenAdQueueRemoveExpiredAdEvent:fullscreenAdQueueRemoveExpiredAdEvent];
    }

    void * _CBMFullscreenAdQueueGetQueue(const char * placementName)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = [CBMFullscreenAdQueue queueForPlacement:toNSStringOrEmpty(placementName)];
        // TODO: This will reset the delegate on the queue with same placement name. Should be fine though ?
        [fullScreenAdQueue setDelegate:[CBMUnityObserver sharedObserver]];
        return (__bridge void *)fullScreenAdQueue;
    }

    const char * _CBMFullscreenAdQueueGetKeywords(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        return toJSON([fullScreenAdQueue keywords]);
    }

    void _CBMFullscreenAdQueueSetKeywords(const void * uniqueId, const char * keywordsJson)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        NSMutableDictionary *formattedKeywords = toObjectFromJson<NSMutableDictionary *>(keywordsJson);
        [fullScreenAdQueue setKeywords:formattedKeywords];
    }

    int _CBMFullscreenAdQueueGetQueueCapacity(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        return (int)[fullScreenAdQueue queueCapacity];
    }

    int _CBMFullscreenAdQueueGetNumberOfAdsReady(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        return (int)[fullScreenAdQueue numberOfAdsReady];
    }

    bool _CBMFullscreenAdQueueIsRunning(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        return [fullScreenAdQueue isRunning];
    }

    void * _CBMFullscreenAdQueueGetNextAd(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        CBMFullscreenAd* ad = [fullScreenAdQueue getNextAd];
        [ad setDelegate:[CBMUnityObserver sharedObserver]];
        [[CBMAdStore sharedStore] storeAd:ad];
        return (__bridge void*)ad;
    }

    bool _CBMFullscreenAdQueueHasNextAd(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        return [fullScreenAdQueue hasNextAd];
    }

    void _CBMFullscreenAdQueueStart(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        [fullScreenAdQueue start];
    }

    void _CBMFullscreenAdQueueStop(const void * uniqueId)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        [fullScreenAdQueue stop];
    }

    void _CBMFullscreenAdQueueSetCapacity(const void * uniqueId, int capacity)
    {
        CBMFullscreenAdQueue * fullScreenAdQueue = GetFullScreenAdQueue(uniqueId);
        [fullScreenAdQueue setQueueCapacity:capacity];
    }
}
