#import "CBMDelegates.h"

/// Allocates a NSDictionary from a CBMBannerSize*
///
/// Use to allocate a NSDictionary reference when converting CBMBannerSize to C String.
///
/// - Parameter size: CBMBannerSize to use as a base for the NSDictionary.
/// - Returns: Allocated NSDictionary or Null.
NSDictionary * bannerSizeToDictionary(CBMBannerSize* size);
