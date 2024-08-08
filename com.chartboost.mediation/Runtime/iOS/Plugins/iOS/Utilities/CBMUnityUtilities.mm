#import "CBMUnityUtilities.h"

NSDictionary * bannerSizeToDictionary(CBMBannerSize* size) {
    const NSString * aspectRatioKey = @"aspectRatio";
    const NSString * widthKey = @"width";
    const NSString * heightKey = @"height";
    const NSString * typeKey = @"type";
    const NSString *sizeTypeKey = @"sizeType";

    NSString * aspectRatioValue = [NSString stringWithFormat:@"%f", size.aspectRatio];
    NSString * widthValue = [NSString stringWithFormat:@"%f", size.size.width];
    NSString * heightValue = [NSString stringWithFormat:@"%f", size.size.height];
    NSString * typeValue = [NSString stringWithFormat:@"%d", (int)size.type];
    NSString *sizeTypeValue = @"";

    if(size.type == 0) {  // Fixed
        int width = size.size.width;
        switch (width) {
            case 320: sizeTypeValue = [NSString stringWithFormat:@"%d", 0]; break;  // Standard
            case 300: sizeTypeValue = [NSString stringWithFormat:@"%d", 1]; break;  // Medium
            case 728: sizeTypeValue = [NSString stringWithFormat:@"%d", 2]; break;  // Leaderboard
            default: sizeTypeValue = [NSString stringWithFormat:@"%d", -1];break;   // Unknown
        }
    }
    else
        sizeTypeValue = [NSString stringWithFormat:@"%d", 3];   // Adaptive

    return [NSDictionary dictionaryWithObjectsAndKeys:sizeTypeValue,sizeTypeKey,aspectRatioValue,aspectRatioKey,widthValue,widthKey,heightValue,heightKey,typeValue, typeKey, nil];
}
