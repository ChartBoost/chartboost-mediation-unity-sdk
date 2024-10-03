#import "CBMAdStore.h"

NSString* const CBMAdStoreTAG = @"CBMAdStore";

@implementation CBMAdStore

+ (instancetype)sharedStore {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
        [_sharedObject setAdStore:[[NSMutableDictionary alloc] init]];
    });

    return _sharedObject;
}

- (void)storeAd:(id)ad {
    NSNumber *key = [NSNumber numberWithLong:(long)ad];
    [_adStore setObject:ad forKey:key];
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMAdStoreTAG log:[NSString stringWithFormat:@"Stored ad with key: %@", key] logLevel:CBLLogLevelDebug];
}

- (void)storeAd:(id)ad withKey:(NSNumber *)key
{
    [_adStore setObject:ad forKey:key];
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMAdStoreTAG log:[NSString stringWithFormat:@"Stored ad with key: %@", key] logLevel:CBLLogLevelDebug];
}

- (void)releaseAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];
    [_adStore removeObjectForKey:key];
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMAdStoreTAG log:[NSString stringWithFormat:@"Removed ad from store with key: %@", key] logLevel:CBLLogLevelDebug];
}
@end
