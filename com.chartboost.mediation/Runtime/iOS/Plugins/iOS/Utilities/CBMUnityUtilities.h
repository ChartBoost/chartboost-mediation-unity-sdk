/**
 * @file CBMUnityUtilities.h
 * @brief Utility functions for Chartboost Mediation Unity bridge operations.
 *
 * This header declares helper functions used across the Unity-iOS bridge to perform
 * common conversion and serialization tasks.
 *
 * @version 5.0.0
 * @note Requires Unity 2020.3+ and Chartboost Mediation SDK 5.0+
 */

#import "CBMDelegates.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * Banner size type identifiers for Unity layer.
 *
 * These values match the C# enum definitions and are used to identify
 * specific banner size presets in the serialized dictionary.
 */
typedef NS_ENUM(NSInteger, CBMBannerSizeType) {
    /// Standard 320x50 banner
    CBMBannerSizeTypeStandard = 0,

    /// Medium 300x250 rectangle banner
    CBMBannerSizeTypeMedium = 1,

    /// Leaderboard 728x90 banner
    CBMBannerSizeTypeLeaderboard = 2,

    /// Adaptive banner (dynamic sizing)
    CBMBannerSizeTypeAdaptive = 3,

    /// Unknown or custom fixed size
    CBMBannerSizeTypeUnknown = -1
};

/**
 * Converts a Chartboost Mediation banner size to a dictionary for JSON serialization.
 *
 * This function creates an NSDictionary containing all relevant banner size information
 * that can be serialized to JSON and passed to the Unity C# layer. The dictionary
 * includes aspect ratio, dimensions, type, and a size type identifier.
 *
 * Dictionary Keys:
 * - "aspectRatio": String representation of width/height ratio
 * - "width": String representation of banner width in points
 * - "height": String representation of banner height in points
 * - "type": String representation of CBMBannerType (0=Fixed, 1=Adaptive)
 * - "sizeType": String representation of CBMBannerSizeType enum value
 *
 * @param size The banner size to convert. Must not be nil.
 * @return Dictionary containing banner size information, or nil if size parameter is nil
 *
 * @note The function automatically detects standard banner sizes (Standard, Medium, Leaderboard)
 *       based on width for fixed-size banners. Adaptive banners are always marked as
 *       CBMBannerSizeTypeAdaptive regardless of dimensions.
 *
 * @see CBMBannerSizeType for size type identifier values
 * @see CBMBannerSize for Chartboost Mediation banner size class
 */
NSDictionary * _Nullable bannerSizeToDictionary(CBMBannerSize* _Nullable size);

NS_ASSUME_NONNULL_END
