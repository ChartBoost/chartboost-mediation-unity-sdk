#import "CBMAdStore.h"

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
}

- (void)storeAd:(id)ad withKey:(NSNumber *)key
{
    [_adStore setObject:ad forKey:key];
}

- (void)releaseAd:(const void *)uniqueId {
    NSNumber *key = [NSNumber numberWithLong:(long)uniqueId];
    [_adStore removeObjectForKey:key];
}
@end
