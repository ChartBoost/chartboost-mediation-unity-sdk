#import "CBMDelegates.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * Centralized storage for managing ad instances across the Unity-iOS bridge.
 *
 * This class maintains references to both fullscreen and banner ads to prevent premature
 * deallocation and enable lifecycle management from the Unity C# layer.
 *
 * Thread Safety: All operations are thread-safe using synchronized access.
 * Memory Management: Callers must explicitly call release methods to prevent memory leaks.
 * Use clearAll during application shutdown to release all tracked ads.
 */
@interface CBMAdStore : NSObject

/**
 * Returns the shared singleton instance of the ad store.
 *
 * @return The shared CBMAdStore instance
 */
+ (instancetype)sharedStore;

/**
 * Tracks a fullscreen ad instance in the store.
 *
 * This method stores a reference to the fullscreen ad to prevent deallocation
 * and enables retrieval via the ad's pointer address from Unity.
 *
 * @param ad The fullscreen ad instance to track
 * @return The unique key (pointer address) assigned to this ad for future reference
 */
- (NSNumber *)trackFullscreenAd:(id)ad;

/**
 * Releases a tracked fullscreen ad and cleans up its resources.
 *
 * This method removes the ad from the store, allowing deallocation.
 * Should be called when the ad is no longer needed.
 *
 * @param uniqueId The pointer address identifying the fullscreen ad to release
 */
- (void)releaseFullscreenAd:(const void *)uniqueId;

/**
 * Retrieves a fullscreen ad from the store without releasing it.
 *
 * @param uniqueId The pointer address identifying the fullscreen ad
 * @return The fullscreen ad instance, or nil if not found
 */
- (nullable id)getFullscreenAd:(const void *)uniqueId;

/**
 * Tracks a banner ad wrapper instance in the store.
 *
 * Banner ad wrappers must be stored using their underlying bannerView pointer as
 * the key, since delegate callbacks receive the bannerView, not the wrapper.
 *
 * @param ad The banner ad wrapper instance to track
 * @param key The key to use for storage (the bannerView pointer)
 * @return The key used for storage
 */
- (NSNumber *)trackBannerAd:(id)ad withKey:(NSNumber *)key;

/**
 * Releases a tracked banner ad and cleans up its resources.
 *
 * This method removes the ad from the store, allowing deallocation.
 * Should be called when the ad is no longer needed.
 *
 * @param uniqueId The pointer address identifying the banner ad to release
 */
- (void)releaseBannerAd:(const void *)uniqueId;

/**
 * Retrieves a banner ad from the store without releasing it.
 *
 * @param uniqueId The pointer address identifying the banner ad
 * @return The banner ad instance, or nil if not found
 */
- (nullable id)getBannerAd:(const void *)uniqueId;

/**
 * Clears all tracked ads and releases their resources.
 *
 * This method should be called during application shutdown or reset to prevent
 * memory leaks. It removes all ads from both stores.
 */
- (void)clearAll;

/**
 * Returns diagnostic information about the current state of the ad store.
 *
 * Provides counts of tracked fullscreen and banner ads for debugging and monitoring.
 *
 * @return A formatted string containing ad store statistics
 */
- (NSString *)adStoreInfo;

@end

NS_ASSUME_NONNULL_END
