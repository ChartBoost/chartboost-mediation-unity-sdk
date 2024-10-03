#import "CBMDelegates.h"

typedef void (*CBMImpressionLevelRevenueDataEvent)(int hashCode, const char* impressionDataJson);

@interface CBMUnityILRDObserver : NSObject

+ (instancetype) sharedObserver;

@property BOOL consumeILRDOnRetrieval;
@property (nonatomic, strong) NSLock *fileLock;
@property NSMutableDictionary* ilrdCache;
@property NSMutableDictionary* completers;
@property CBMImpressionLevelRevenueDataEvent onImpression;

@end
