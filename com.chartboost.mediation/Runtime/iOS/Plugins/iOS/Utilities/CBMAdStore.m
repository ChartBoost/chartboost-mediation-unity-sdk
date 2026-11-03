#import "CBMAdStore.h"
#import "CBAdStore.h"   // shared ad-retention primitive from com.chartboost.unity.utilities

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
     * Thread-safe storage for fullscreen ad instances, keyed by pointer address.
     * Retains references to prevent deallocation while ads are in use.
     */
    CBAdStore *_fullscreenAdStore;

    /**
     * Thread-safe storage for banner ad instances, keyed by their bannerView pointer.
     * Retains references to prevent deallocation while ads are in use.
     */
    CBAdStore *_bannerAdStore;
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
        _fullscreenAdStore = [[CBAdStore alloc] init];
        _bannerAdStore = [[CBAdStore alloc] init];
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

    // Best-effort leak warning; the count read need not be atomic with the insert below.
    if (_fullscreenAdStore.count >= kMaxStoreSize) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Fullscreen ad store exceeded maximum size (%ld). Possible memory leak", (long)kMaxStoreSize]
                                                 logLevel:CBLLogLevelError];
    }

    NSNumber *key = [NSNumber numberWithLong:(long)ad];
    [_fullscreenAdStore trackAd:ad forKey:(long)ad];
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                  log:[NSString stringWithFormat:@"Tracking FullscreenAd with Id: %@ (Total fullscreen ads: %lu)", key, (unsigned long)_fullscreenAdStore.count]
                                             logLevel:CBLLogLevelVerbose];
    return key;
}

- (void)releaseFullscreenAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];

    if ([_fullscreenAdStore removeKey:(long)uniqueId] != nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Released FullscreenAd with Id: %@ (Remaining fullscreen ads: %lu)", key, (unsigned long)_fullscreenAdStore.count]
                                                 logLevel:CBLLogLevelVerbose];
    } else {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Attempted to release non-existent FullscreenAd with Id: %@", key]
                                                 logLevel:CBLLogLevelWarning];
    }
}

- (nullable id)getFullscreenAd:(const void *)uniqueId {
    return [_fullscreenAdStore adForKey:(long)uniqueId];
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

    // Best-effort leak warning; the count read need not be atomic with the insert below.
    if (_bannerAdStore.count >= kMaxStoreSize) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Banner ad store exceeded maximum size (%ld). Possible memory leak", (long)kMaxStoreSize]
                                                 logLevel:CBLLogLevelError];
    }

    [_bannerAdStore trackAd:ad forKey:key.longValue];
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                  log:[NSString stringWithFormat:@"Tracking BannerAd with Id: %@ (Total banner ads: %lu)", key, (unsigned long)_bannerAdStore.count]
                                             logLevel:CBLLogLevelVerbose];
    return key;
}

- (void)releaseBannerAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];

    if ([_bannerAdStore removeKey:(long)uniqueId] != nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Released BannerAd with Id: %@ (Remaining banner ads: %lu)", key, (unsigned long)_bannerAdStore.count]
                                                 logLevel:CBLLogLevelVerbose];
    } else {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                      log:[NSString stringWithFormat:@"Attempted to release non-existent BannerAd with Id: %@", key]
                                                 logLevel:CBLLogLevelWarning];
    }
}

- (nullable id)getBannerAd:(const void *)uniqueId {
    return [_bannerAdStore adForKey:(long)uniqueId];
}

- (void)clearAll {
    NSUInteger fullscreenCount = _fullscreenAdStore.count;
    NSUInteger bannerCount = _bannerAdStore.count;

    [_fullscreenAdStore clear];
    [_bannerAdStore clear];

    [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMAdStoreTag
                                                  log:[NSString stringWithFormat:@"Cleared all ad stores (Released %lu fullscreen ads, %lu banner ads)", (unsigned long)fullscreenCount, (unsigned long)bannerCount]
                                             logLevel:CBLLogLevelInfo];
}

- (NSString *)adStoreInfo {
    return [NSString stringWithFormat:@"%@ Fullscreen AdStore Count: %lu, Banner AdStore Count: %lu",
            kCBMAdStoreTag,
            (unsigned long)_fullscreenAdStore.count,
            (unsigned long)_bannerAdStore.count];
}

@end
