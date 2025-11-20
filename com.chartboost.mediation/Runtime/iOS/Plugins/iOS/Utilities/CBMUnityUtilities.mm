/**
 * @file CBMUnityUtilities.mm
 * @brief Implementation of utility functions for Chartboost Mediation Unity bridge.
 *
 * This file implements helper functions for converting native iOS SDK objects
 * to formats suitable for transmission to the Unity C# layer.
 */

#import "CBMUnityUtilities.h"

/**
 * Tag used for logging operations related to CBMUnityUtilities.
 */
static NSString * const kCBMUnityUtilitiesTAG = @"CBMUnityUtilities";

/**
 * Standard banner width in points (320x50).
 */
static const int kStandardBannerWidth = 320;

/**
 * Medium rectangle banner width in points (300x250).
 */
static const int kMediumBannerWidth = 300;

/**
 * Leaderboard banner width in points (728x90).
 */
static const int kLeaderboardBannerWidth = 728;

NSDictionary * _Nullable bannerSizeToDictionary(CBMBannerSize* _Nullable size) {
    // Validate input parameter
    if (size == nil) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityUtilitiesTAG
                                                      log:@"bannerSizeToDictionary: Received nil size parameter"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    // Dictionary keys - using const for better performance
    static NSString * const kAspectRatioKey = @"aspectRatio";
    static NSString * const kWidthKey = @"width";
    static NSString * const kHeightKey = @"height";
    static NSString * const kTypeKey = @"type";
    static NSString * const kSizeTypeKey = @"sizeType";

    // Extract values from banner size
    CGFloat aspectRatio = size.aspectRatio;
    CGFloat width = size.size.width;
    CGFloat height = size.size.height;
    NSInteger type = (NSInteger)size.type;

    // Convert values to strings for JSON serialization
    NSString *aspectRatioValue = [NSString stringWithFormat:@"%f", aspectRatio];
    NSString *widthValue = [NSString stringWithFormat:@"%f", width];
    NSString *heightValue = [NSString stringWithFormat:@"%f", height];
    NSString *typeValue = [NSString stringWithFormat:@"%ld", (long)type];

    // Determine size type identifier
    NSString *sizeTypeValue;
    CBMBannerSizeType sizeType;

    if (type == 0) {  // Fixed size banner
        // Detect standard banner sizes based on width
        int widthInt = (int)width;
        switch (widthInt) {
            case kStandardBannerWidth:
                sizeType = CBMBannerSizeTypeStandard;
                break;
            case kMediumBannerWidth:
                sizeType = CBMBannerSizeTypeMedium;
                break;
            case kLeaderboardBannerWidth:
                sizeType = CBMBannerSizeTypeLeaderboard;
                break;
            default:
                sizeType = CBMBannerSizeTypeUnknown;
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityUtilitiesTAG
                                                              log:[NSString stringWithFormat:@"bannerSizeToDictionary: Unknown fixed banner width: %d", widthInt]
                                                         logLevel:CBLLogLevelWarning];
                break;
        }
    } else {  // Adaptive size banner
        sizeType = CBMBannerSizeTypeAdaptive;
    }

    sizeTypeValue = [NSString stringWithFormat:@"%ld", (long)sizeType];

    [[CBLUnityLoggingBridge sharedLogger] logWithTag:kCBMUnityUtilitiesTAG
                                                  log:[NSString stringWithFormat:@"bannerSizeToDictionary: Converting banner size - width: %.1f, height: %.1f, type: %ld, sizeType: %ld",
                                                       width, height, (long)type, (long)sizeType]
                                             logLevel:CBLLogLevelVerbose];

    // Create and return dictionary with all banner size information
    return @{
        kSizeTypeKey: sizeTypeValue,
        kAspectRatioKey: aspectRatioValue,
        kWidthKey: widthValue,
        kHeightKey: heightValue,
        kTypeKey: typeValue
    };
}
