#import "CBMUnityILRDObserver.h"


@implementation CBMUnityILRDObserver

NSString* const CBMUnityILRDObserverTAG = @"UnityILRDObserver";
NSString* const fileName = @"ilrd_cache.json";

+ (instancetype)sharedObserver {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
        [_sharedObject setIlrdCache:[[NSMutableDictionary alloc] init]];
        [_sharedObject setCompleters:[[NSMutableDictionary alloc] init]];
        [_sharedObject setFileLock:[[NSLock alloc] init]];
        [_sharedObject setConsumeILRDOnRetrieval:true];
    });

    return _sharedObject;
}

#pragma mark ChartboostMediationDidReceiveILRD
- (void) subscribeILRDObserver {
    [[NSNotificationCenter defaultCenter] addObserverForName:NSNotification.chartboostMediationDidReceiveILRD object:nil queue:nil usingBlock:^(NSNotification* _Nonnull notification) {

        CBMImpressionData *ilrd = notification.object;
        NSString *placement = ilrd.placement;
        NSDictionary *json = ilrd.jsonData;
        const NSString *placementNameKey = @"placement";
        const NSString *ilrdKey = @"ilrd";
        NSDictionary *unityILRDJSON = [NSDictionary dictionaryWithObjectsAndKeys: placement, placementNameKey, json ? json : [NSNull null], ilrdKey, nil];

        [self cacheImpressionData:toJSONNSString(unityILRDJSON)];
    }];
}

#pragma mark ILRD Cache Handler
- (void) retrieveImpressionData {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        [[self fileLock] lock];

        try {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:@"Attempting to retrieve impression data" logLevel:CBLLogLevelVerbose];
            NSString * filePath = [self filePathWithName:fileName];

            NSFileManager *manager = [NSFileManager defaultManager];

            if (![manager fileExistsAtPath:filePath])
            {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:@"Nothing to retrieve. Cache is clean." logLevel:CBLLogLevelVerbose];
                [[self fileLock] unlock];
                return;
            }

            NSData *jsonData = [NSData dataWithContentsOfFile:filePath];

            if (jsonData)
            {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:[NSString stringWithFormat:@"Read file at %@ with contents: %@", filePath, jsonData] logLevel:CBLLogLevelVerbose];

                NSError* error = nil;
                NSMutableDictionary * stringKeyDictionary = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:&error];

                for (NSString* stringKey in stringKeyDictionary) {
                    NSNumber* numberKey = @([stringKey integerValue]);
                    NSString* ilrdJson = [stringKeyDictionary objectForKey:stringKey];
                    [self->_ilrdCache setObject:ilrdJson forKey:numberKey];
                }

                if (self->_consumeILRDOnRetrieval) {
                    for(NSNumber* key in self->_ilrdCache)
                    {
                        NSString* unityILRDJson = [self->_ilrdCache objectForKey:key];
                        [self requestUnityILRDConsumption:unityILRDJson];
                    }
                }
            }
        } catch (NSException *exception) {
            NSString* logMessage = [NSString stringWithFormat:@"Failed to read File at %@ with exception: %@", fileName, exception];
            [[CBLUnityLoggingBridge sharedLogger] logExceptionWithTag:CBMUnityILRDObserverTAG exception:logMessage];
        }
        [[self fileLock] unlock];
    });
}

- (void) requestUnityILRDConsumption:(NSString*) unityILRDJson{
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:@"Requesting Unity Consumption" logLevel:CBLLogLevelVerbose];
    int hash = hashCode(unityILRDJson);
    NSNumber* key = [NSNumber numberWithInt:hash];
    if (self->_onImpression != nil)
    {
        void (^completer)(int uniqueId) = ^(int hashCode){
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:@"Unity Consumption Completed" logLevel:CBLLogLevelVerbose];
            [self removeImpressionData:hashCode];
        };

        [self->_completers setObject:completer forKey:key];
        self->_onImpression(hash, toCStringOrNull(unityILRDJson));
    }
}

- (void) cacheImpressionData:(NSString*)unityILRDJson {
    int hash = hashCode(unityILRDJson);

    NSNumber* key = [NSNumber numberWithInt:hash];

    if ([_ilrdCache objectForKey:key] != nil)
        return;

    [_ilrdCache setObject:unityILRDJson forKey:key];
    for(NSNumber* key in _ilrdCache)
    {
        NSString* unityILRDJson = [_ilrdCache objectForKey:key];
        [self requestUnityILRDConsumption:unityILRDJson];
    }
    [self saveCacheToFile];
}

- (void) removeImpressionData:(int)hashCode {

    NSNumber* key = [NSNumber numberWithInt:hashCode];
    if ([_ilrdCache objectForKey:key] == nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:@"Requested ID %@ to be removed from ILRD Cache but no such key is present." logLevel:CBLLogLevelWarning];
        return;
    }

    [_ilrdCache removeObjectForKey:key];
    [self saveCacheToFile];
}

- (void) saveCacheToFile {
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        [[self fileLock] lock];

        try {
            NSString * filePath = [self filePathWithName:fileName];
            NSError * error = nil;

            if([[self ilrdCache] count] <= 0)
            {
                NSFileManager *manager = [NSFileManager defaultManager];
                if ([manager fileExistsAtPath:filePath])
                {
                    [manager removeItemAtPath:filePath error:nil];
                    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:[NSString stringWithFormat:@"Cache empty file at %@ deleted", filePath] logLevel:CBLLogLevelVerbose];
                    [[self fileLock] unlock];
                    return;
                }

                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:[NSString stringWithFormat:@"Cache empty and no file at %@", filePath] logLevel:CBLLogLevelVerbose];
                [[self fileLock] unlock];
                return;
            }

            NSMutableDictionary *stringKeyDictionary = [NSMutableDictionary dictionary];

            for (NSNumber* key in  self->_ilrdCache) {
                NSString *stringKey = [key stringValue];
                NSString *ilrdValue = [self->_ilrdCache objectForKey:key];
                [stringKeyDictionary setObject:ilrdValue forKey:stringKey];
            }

            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:stringKeyDictionary options:0 error:&error];

            if (error)
            {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:[NSString stringWithFormat:@"Failed to parse .json with Error: %@", error] logLevel:CBLLogLevelError];
            }

            if (jsonData && !error)
            {
                [jsonData writeToFile:filePath atomically:YES];
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:[NSString stringWithFormat:@"Saved file at %@ with ilrd: %@", filePath, jsonData] logLevel:CBLLogLevelVerbose];
            }
        } catch (NSException *exception) {
            NSString* logMessage = [NSString stringWithFormat:@"Failed to save ILRD Cache with Exception: %@, %@", fileName, exception];
            [[CBLUnityLoggingBridge sharedLogger] logExceptionWithTag:CBMUnityILRDObserverTAG exception:logMessage];
        }

        [[self fileLock] unlock];
    });
}

- (NSString *) filePathWithName: (NSString *)fileName {
    NSArray* paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString* documentsDirectory = [paths firstObject];
    return [documentsDirectory stringByAppendingPathComponent:fileName];
}
@end

extern "C" {
    void _CBMSetUnityILRDProxy(CBMImpressionLevelRevenueDataEvent ilrdEvent) {
        [[CBMUnityILRDObserver sharedObserver] setOnImpression:ilrdEvent];
        [[CBMUnityILRDObserver sharedObserver] subscribeILRDObserver];
    }

    void _CBMSetConsumeILRDOnRetrieval(bool value) {
        [[CBMUnityILRDObserver sharedObserver] setConsumeILRDOnRetrieval:value];
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMUnityILRDObserverTAG log:[NSString stringWithFormat:@"ILRD Consumed on Cached Retrieval Set to: %d", value] logLevel:CBLLogLevelVerbose];
    }

    void _CBMRetrieveImpressionData() {
        [[CBMUnityILRDObserver sharedObserver] retrieveImpressionData];
    }

    void _CBMCompleteUnityILRDRequest(int hashCode) {
        NSNumber *key = [NSNumber numberWithInt:hashCode];

        if ([[[CBMUnityILRDObserver sharedObserver] completers] objectForKey:key] == nil)
            return;

        void (^completer)(int uniqueId) = [[[CBMUnityILRDObserver sharedObserver] completers] objectForKey:key];

        if (completer != nil)
            completer(hashCode);

        [[[CBMUnityILRDObserver sharedObserver] completers] removeObjectForKey:key];
    }
}
