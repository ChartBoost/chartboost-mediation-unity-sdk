#import "CBMAdStore.h"

/**
 * Tag used for logging operations related to CBMAdStore.
 */
static NSString * const kCBMAdStoreTag = @"CBMAdStore";

/**
 * Maximum number of ads allowed in each store to prevent memory leaks.
 */
static const NSInteger kMaxStoreSize = 100;

@implementation CBMAdStore {
    /**
     * Thread-safe storage for fullscreen ad instances keyed by pointer addresses.
     * Stores references to prevent deallocation while ads are in use.
     */
    NSMutableDictionary *_fullscreenAdStore;

    /**
     * Thread-safe storage for banner ad instances keyed by pointer addresses.
     * Stores references to prevent deallocation while ads are in use.
     */
    NSMutableDictionary *_bannerAdStore;

    /**
     * Dispatch queue for synchronizing access to the ad stores.
     * Uses a concurrent queue with barrier for write operations.
     */
    dispatch_queue_t _syncQueue;
}

+ (instancetype)sharedStore {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        _fullscreenAdStore = [[NSMutableDictionary alloc] init];
        _bannerAdStore = [[NSMutableDictionary alloc] init];
        _syncQueue = dispatch_queue_create("com.chartboost.mediation.adstore", DISPATCH_QUEUE_CONCURRENT);
    }
    return self;
}

- (NSNumber *)trackFullscreenAd:(id)ad {
    if (ad == nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:@"Cannot track nil fullscreen ad"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    __block NSNumber *key = nil;
    dispatch_barrier_sync(_syncQueue, ^{
        if (self->_fullscreenAdStore.count >= kMaxStoreSize) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                          log:[NSString stringWithFormat:@"Fullscreen ad store exceeded maximum size (%ld). Possible memory leak", (long)kMaxStoreSize]
                                                     logLevel:CBLLogLevelError];
        }

        key = [NSNumber numberWithLong:(long)ad];
        self->_fullscreenAdStore[key] = ad;
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Tracking FullscreenAd with Id: %@ (Total fullscreen ads: %lu)", key, (unsigned long)self->_fullscreenAdStore.count]
                                                 logLevel:CBLLogLevelVerbose];
    });

    return key;
}

- (void)releaseFullscreenAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];

    dispatch_barrier_async(_syncQueue, ^{
        id ad = self->_fullscreenAdStore[key];
        if (ad != nil) {
            [self->_fullscreenAdStore removeObjectForKey:key];
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                          log:[NSString stringWithFormat:@"Released FullscreenAd with Id: %@ (Remaining fullscreen ads: %lu)", key, (unsigned long)self->_fullscreenAdStore.count]
                                                     logLevel:CBLLogLevelVerbose];
        } else {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                          log:[NSString stringWithFormat:@"Attempted to release non-existent FullscreenAd with Id: %@", key]
                                                     logLevel:CBLLogLevelWarning];
        }
    });
}

- (nullable id)getFullscreenAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];
    __block id ad = nil;

    dispatch_sync(_syncQueue, ^{
        ad = self->_fullscreenAdStore[key];
    });

    return ad;
}

- (NSNumber *)trackBannerAd:(id)ad withKey:(NSNumber *)key {
    if (ad == nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:@"Cannot track nil banner ad"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    if (key == nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:@"Cannot track banner ad with nil key"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    dispatch_barrier_sync(_syncQueue, ^{
        if (self->_bannerAdStore.count >= kMaxStoreSize) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                          log:[NSString stringWithFormat:@"Banner ad store exceeded maximum size (%ld). Possible memory leak", (long)kMaxStoreSize]
                                                     logLevel:CBLLogLevelError];
        }

        self->_bannerAdStore[key] = ad;
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Tracking BannerAd with Id: %@ (Total banner ads: %lu)", key, (unsigned long)self->_bannerAdStore.count]
                                                 logLevel:CBLLogLevelVerbose];
    });

    return key;
}

- (void)releaseBannerAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];

    dispatch_barrier_async(_syncQueue, ^{
        id ad = self->_bannerAdStore[key];
        if (ad != nil) {
            [self->_bannerAdStore removeObjectForKey:key];
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                          log:[NSString stringWithFormat:@"Released BannerAd with Id: %@ (Remaining banner ads: %lu)", key, (unsigned long)self->_bannerAdStore.count]
                                                     logLevel:CBLLogLevelVerbose];
        } else {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                          log:[NSString stringWithFormat:@"Attempted to release non-existent BannerAd with Id: %@", key]
                                                     logLevel:CBLLogLevelWarning];
        }
    });
}

- (nullable id)getBannerAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];
    __block id ad = nil;

    dispatch_sync(_syncQueue, ^{
        ad = self->_bannerAdStore[key];
    });

    return ad;
}

- (void)clearAll {
    dispatch_barrier_async(_syncQueue, ^{
        NSUInteger fullscreenCount = self->_fullscreenAdStore.count;
        NSUInteger bannerCount = self->_bannerAdStore.count;

        [self->_fullscreenAdStore removeAllObjects];
        [self->_bannerAdStore removeAllObjects];

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Cleared all ad stores (Released %lu fullscreen ads, %lu banner ads)", (unsigned long)fullscreenCount, (unsigned long)bannerCount]
                                                 logLevel:CBLLogLevelInfo];
    });
}

- (NSString *)adStoreInfo {
    __block NSString *info = nil;

    dispatch_sync(_syncQueue, ^{
        info = [NSString stringWithFormat:@"%@ Fullscreen AdStore Count: %lu, Banner AdStore Count: %lu",
                kCBMAdStoreTag,
                (unsigned long)self->_fullscreenAdStore.count,
                (unsigned long)self->_bannerAdStore.count];
    });

    return info;
}

@end
