#import "CBMDelegates.h"

@interface CBMAdStore: NSObject

+ (instancetype) sharedStore;

@property NSMutableDictionary* adStore;

- (void)storeAd:(id)ad;
- (void)storeAd:(id)ad withKey:(NSNumber*)key;
- (void)releaseAd:(const void*)uniqueId;

@end
