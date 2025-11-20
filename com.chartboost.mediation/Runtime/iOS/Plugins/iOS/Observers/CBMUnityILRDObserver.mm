/**
 * @file CBMUnityILRDObserver.mm
 * @brief Implementation of ILRD observer with thread-safe caching and file persistence.
 *
 * This implementation provides thread-safe access to ILRD cache using a serial
 * dispatch queue. All mutations to internal state are synchronized through this queue.
 */

#import "CBMUnityILRDObserver.h"

/**
 * Tag used for logging operations related to ILRD observer.
 */
static NSString * const kCBMUnityILRDObserverTAG = @"UnityILRDObserver";

/**
 * Filename for persistent ILRD cache in Documents directory.
 */
static NSString * const kILRDCacheFileName = @"ilrd_cache.json";

/**
 * Maximum number of cached ILRD entries to prevent memory issues.
 */
static const NSInteger kMaxCacheSize = 1000;

@implementation CBMUnityILRDObserver {
    /**
     * Thread-safe cache for ILRD data keyed by hash code.
     * Access synchronized via _syncQueue.
     */
    NSMutableDictionary<NSNumber *, NSString *> *_ilrdCache;

    /**
     * Thread-safe storage for completion handlers keyed by hash code.
     * Access synchronized via _syncQueue.
     */
    NSMutableDictionary<NSNumber *, void (^)(int)> *_completers;

    /**
     * Serial dispatch queue for synchronizing access to internal state.
     * All mutations to _ilrdCache and _completers must use this queue.
     */
    dispatch_queue_t _syncQueue;

    /**
     * Serial dispatch queue for file I/O operations.
     * Prevents concurrent file access issues.
     */
    dispatch_queue_t _fileQueue;
}

+ (instancetype)sharedObserver {
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
        _ilrdCache = [[NSMutableDictionary alloc] init];
        _completers = [[NSMutableDictionary alloc] init];
        _syncQueue = dispatch_queue_create("com.chartboost.mediation.ilrd.sync", DISPATCH_QUEUE_SERIAL);
        _fileQueue = dispatch_queue_create("com.chartboost.mediation.ilrd.file", DISPATCH_QUEUE_SERIAL);
        _consumeILRDOnRetrieval = YES;

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:@"ILRD Observer initialized"
                                                 logLevel:CBLLogLevelVerbose];
    }
    return self;
}

#pragma mark - Notification Subscription

- (void)subscribeILRDObserver {
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                  log:@"Subscribing to ILRD notifications"
                                             logLevel:CBLLogLevelVerbose];

    [[NSNotificationCenter defaultCenter] addObserverForName:NSNotification.chartboostMediationDidReceiveILRD
                                                       object:nil
                                                        queue:nil
                                                   usingBlock:^(NSNotification * _Nonnull notification) {
        CBMImpressionData *ilrd = notification.object;
        if (!ilrd) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:@"Received ILRD notification with nil object"
                                                     logLevel:CBLLogLevelWarning];
            return;
        }

        NSString *placement = ilrd.placement;
        NSDictionary *json = ilrd.jsonData;

        // Create Unity-compatible JSON structure
        NSDictionary *unityILRDJSON = @{
            @"placement": placement ?: [NSNull null],
            @"ilrd": json ?: [NSNull null]
        };

        NSString *jsonString = toJSONNSString(unityILRDJSON);
        if (jsonString) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"Received ILRD for placement: %@", placement]
                                                     logLevel:CBLLogLevelVerbose];
            [self cacheImpressionData:jsonString];
        } else {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:@"Failed to serialize ILRD to JSON"
                                                     logLevel:CBLLogLevelError];
        }
    }];
}

#pragma mark - ILRD Cache Management

- (void)cacheImpressionData:(NSString *)unityILRDJson {
    if (!unityILRDJson) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:@"cacheImpressionData: Received nil JSON"
                                                 logLevel:CBLLogLevelError];
        return;
    }

    int hash = hashCode(unityILRDJson);
    NSNumber *key = @(hash);

    dispatch_async(_syncQueue, ^{
        // Check if already cached
        if (self->_ilrdCache[key] != nil) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"ILRD with hash %d already cached, skipping", hash]
                                                     logLevel:CBLLogLevelVerbose];
            return;
        }

        // Check cache size limit
        if (self->_ilrdCache.count >= kMaxCacheSize) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"ILRD cache exceeded maximum size (%ld). Possible memory leak", (long)kMaxCacheSize]
                                                     logLevel:CBLLogLevelError];
        }

        // Add to cache
        self->_ilrdCache[key] = unityILRDJson;
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:[NSString stringWithFormat:@"Cached ILRD with hash %d (Total cached: %lu)", hash, (unsigned long)self->_ilrdCache.count]
                                                 logLevel:CBLLogLevelVerbose];

        // Request Unity consumption for all cached ILRD
        // This ensures delivery even if Unity wasn't ready when first cached
        NSArray<NSNumber *> *allKeys = [self->_ilrdCache allKeys];
        for (NSNumber *cachedKey in allKeys) {
            NSString *cachedJson = self->_ilrdCache[cachedKey];
            if (cachedJson) {
                [self requestUnityILRDConsumption:cachedJson];
            }
        }

        // Save to file asynchronously
        [self saveCacheToFile];
    });
}

- (void)removeImpressionData:(int)hashCode {
    NSNumber *key = @(hashCode);

    dispatch_async(_syncQueue, ^{
        if (self->_ilrdCache[key] == nil) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"Requested ILRD with hash %d to be removed but not found in cache", hashCode]
                                                     logLevel:CBLLogLevelWarning];
            return;
        }

        [self->_ilrdCache removeObjectForKey:key];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:[NSString stringWithFormat:@"Removed ILRD with hash %d (Remaining: %lu)", hashCode, (unsigned long)self->_ilrdCache.count]
                                                 logLevel:CBLLogLevelVerbose];

        // Save updated cache to file
        [self saveCacheToFile];
    });
}

- (void)requestUnityILRDConsumption:(NSString *)unityILRDJson {
    if (!unityILRDJson) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:@"requestUnityILRDConsumption: Received nil JSON"
                                                 logLevel:CBLLogLevelError];
        return;
    }

    int hash = hashCode(unityILRDJson);
    NSNumber *key = @(hash);

    dispatch_async(_syncQueue, ^{
        if (self->_onImpression == nil) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:@"Unity callback not set, ILRD will remain cached until callback is available"
                                                     logLevel:CBLLogLevelWarning];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:[NSString stringWithFormat:@"Requesting Unity consumption for ILRD hash %d", hash]
                                                 logLevel:CBLLogLevelVerbose];

        // Create completion handler
        void (^completer)(int) = ^(int completedHashCode) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"Unity consumption completed for hash %d", completedHashCode]
                                                     logLevel:CBLLogLevelVerbose];
            [self removeImpressionData:completedHashCode];
        };

        self->_completers[key] = completer;

        // Deliver to Unity
        self->_onImpression(hash, toCStringOrNull(unityILRDJson));
    });
}

#pragma mark - File Persistence

- (void)retrieveImpressionData {
    dispatch_async(_fileQueue, ^{
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:@"Attempting to retrieve cached ILRD from file"
                                                 logLevel:CBLLogLevelVerbose];

        NSString *filePath = [self filePathWithName:kILRDCacheFileName];
        NSFileManager *manager = [NSFileManager defaultManager];

        if (![manager fileExistsAtPath:filePath]) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:@"No cached ILRD file found, cache is clean"
                                                     logLevel:CBLLogLevelVerbose];
            return;
        }

        NSError *readError = nil;
        NSData *jsonData = [NSData dataWithContentsOfFile:filePath options:0 error:&readError];

        if (readError || !jsonData) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"Failed to read ILRD cache file: %@", readError.localizedDescription]
                                                     logLevel:CBLLogLevelError];
            return;
        }

        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:[NSString stringWithFormat:@"Read ILRD cache file (%lu bytes)", (unsigned long)jsonData.length]
                                                 logLevel:CBLLogLevelVerbose];

        NSError *parseError = nil;
        NSDictionary<NSString *, NSString *> *stringKeyDictionary = [NSJSONSerialization JSONObjectWithData:jsonData
                                                                                                      options:NSJSONReadingMutableContainers
                                                                                                        error:&parseError];

        if (parseError || !stringKeyDictionary) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"Failed to parse ILRD cache JSON: %@", parseError.localizedDescription]
                                                     logLevel:CBLLogLevelError];
            return;
        }

        // Load into memory cache
        dispatch_async(self->_syncQueue, ^{
            for (NSString *stringKey in stringKeyDictionary) {
                NSNumber *numberKey = @([stringKey integerValue]);
                NSString *ilrdJson = stringKeyDictionary[stringKey];
                self->_ilrdCache[numberKey] = ilrdJson;
            }

            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"Loaded %lu ILRD entries from cache", (unsigned long)self->_ilrdCache.count]
                                                     logLevel:CBLLogLevelVerbose];

            // Deliver to Unity if enabled
            if (self->_consumeILRDOnRetrieval) {
                NSArray<NSNumber *> *allKeys = [self->_ilrdCache allKeys];
                for (NSNumber *key in allKeys) {
                    NSString *unityILRDJson = self->_ilrdCache[key];
                    if (unityILRDJson) {
                        [self requestUnityILRDConsumption:unityILRDJson];
                    }
                }
            }
        });
    });
}

- (void)saveCacheToFile {
    dispatch_async(_fileQueue, ^{
        dispatch_sync(self->_syncQueue, ^{
            NSString *filePath = [self filePathWithName:kILRDCacheFileName];
            NSFileManager *manager = [NSFileManager defaultManager];

            // If cache is empty, delete the file
            if (self->_ilrdCache.count == 0) {
                if ([manager fileExistsAtPath:filePath]) {
                    NSError *deleteError = nil;
                    [manager removeItemAtPath:filePath error:&deleteError];

                    if (deleteError) {
                        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                                      log:[NSString stringWithFormat:@"Failed to delete empty cache file: %@", deleteError.localizedDescription]
                                                                 logLevel:CBLLogLevelError];
                    } else {
                        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                                      log:@"Deleted empty ILRD cache file"
                                                                 logLevel:CBLLogLevelVerbose];
                    }
                }
                return;
            }

            // Convert number keys to string keys for JSON serialization
            NSMutableDictionary<NSString *, NSString *> *stringKeyDictionary = [NSMutableDictionary dictionary];
            for (NSNumber *key in self->_ilrdCache) {
                NSString *stringKey = [key stringValue];
                NSString *ilrdValue = self->_ilrdCache[key];
                stringKeyDictionary[stringKey] = ilrdValue;
            }

            NSError *serializeError = nil;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:stringKeyDictionary
                                                                options:0
                                                                  error:&serializeError];

            if (serializeError || !jsonData) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                              log:[NSString stringWithFormat:@"Failed to serialize ILRD cache to JSON: %@", serializeError.localizedDescription]
                                                         logLevel:CBLLogLevelError];
                return;
            }

            NSError *writeError = nil;
            [jsonData writeToFile:filePath options:NSDataWritingAtomic error:&writeError];

            if (writeError) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                              log:[NSString stringWithFormat:@"Failed to write ILRD cache file: %@", writeError.localizedDescription]
                                                         logLevel:CBLLogLevelError];
            } else {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                              log:[NSString stringWithFormat:@"Saved ILRD cache to file (%lu entries, %lu bytes)", (unsigned long)self->_ilrdCache.count, (unsigned long)jsonData.length]
                                                         logLevel:CBLLogLevelVerbose];
            }
        });
    });
}

- (NSString *)filePathWithName:(NSString *)fileName {
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths firstObject];
    return [documentsDirectory stringByAppendingPathComponent:fileName];
}

- (void)completeILRDRequest:(int)hashCode {
    NSNumber *key = @(hashCode);

    dispatch_async(_syncQueue, ^{
        void (^completer)(int) = self->_completers[key];

        if (completer == nil) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                          log:[NSString stringWithFormat:@"No completer found for ILRD hash %d", hashCode]
                                                     logLevel:CBLLogLevelWarning];
            return;
        }

        // Execute completer (which removes from cache)
        completer(hashCode);

        // Remove completer
        [self->_completers removeObjectForKey:key];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:[NSString stringWithFormat:@"Completed and cleaned up ILRD hash %d", hashCode]
                                                 logLevel:CBLLogLevelVerbose];
    });
}

@end

#pragma mark - C Bridge Functions

extern "C" {
    /**
     * Sets the Unity callback for ILRD delivery and subscribes to notifications.
     *
     * @param ilrdEvent Function pointer to Unity callback
     */
    void _CBMSetUnityILRDProxy(CBMImpressionLevelRevenueDataEvent ilrdEvent) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:@"Setting Unity ILRD callback"
                                                 logLevel:CBLLogLevelVerbose];
        [[CBMUnityILRDObserver sharedObserver] setOnImpression:ilrdEvent];
        [[CBMUnityILRDObserver sharedObserver] subscribeILRDObserver];
    }

    /**
     * Sets whether cached ILRD should be consumed on retrieval.
     *
     * @param value YES to consume on retrieval, NO to keep cached
     */
    void _CBMSetConsumeILRDOnRetrieval(bool value) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:[NSString stringWithFormat:@"ILRD consume on retrieval set to: %d", value]
                                                 logLevel:CBLLogLevelVerbose];
        [[CBMUnityILRDObserver sharedObserver] setConsumeILRDOnRetrieval:value];
    }

    /**
     * Retrieves cached ILRD from persistent storage.
     */
    void _CBMRetrieveImpressionData() {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityILRDObserverTAG
                                                      log:@"Retrieving cached ILRD"
                                                 logLevel:CBLLogLevelVerbose];
        [[CBMUnityILRDObserver sharedObserver] retrieveImpressionData];
    }

    /**
     * Completes Unity ILRD processing and triggers cleanup.
     *
     * @param hashCode The hash code of the ILRD that was processed
     */
    void _CBMCompleteUnityILRDRequest(int hashCode) {
        [[CBMUnityILRDObserver sharedObserver] completeILRDRequest:hashCode];
    }
}
