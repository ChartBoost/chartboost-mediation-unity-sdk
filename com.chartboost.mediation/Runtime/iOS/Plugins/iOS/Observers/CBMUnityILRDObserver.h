/**
 * @file CBMUnityILRDObserver.h
 * @brief Observer for Impression-Level Revenue Data (ILRD) notifications from Chartboost Mediation.
 *
 * This class manages the collection, caching, and delivery of ILRD data from the native
 * Chartboost Mediation SDK to the Unity C# layer. ILRD provides granular revenue information
 * for each ad impression, enabling detailed analytics and revenue tracking.
 *
 * Features:
 * - Observes SDK ILRD notifications and caches them persistently
 * - Delivers cached ILRD to Unity on app restart (if enabled)
 * - Thread-safe access to ILRD cache and completion handlers
 * - Automatic file persistence to survive app restarts
 *
 * Event Flow:
 * 1. SDK fires ILRD notification → subscribeILRDObserver receives it
 * 2. Data is cached in memory and persisted to file
 * 3. Unity is notified via callback (if available)
 * 4. Unity processes ILRD and calls completion handler
 * 5. Data is removed from cache upon completion
 *
 * Thread Safety: All operations are thread-safe using serial dispatch queues.
 * File Operations: ILRD cache is persisted to Documents directory as JSON.
 *
 * @see CBMImpressionData for native ILRD data structure
 * @version 5.0.0
 * @note Requires Unity 2020.3+ and Chartboost Mediation SDK 5.0+
 */

#import "CBMDelegates.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * Callback function pointer for delivering ILRD data to Unity.
 *
 * This callback is invoked when new ILRD data is available and needs to be
 * delivered to the Unity C# layer for processing.
 *
 * @param hashCode Unique hash code identifying this ILRD instance
 * @param impressionDataJson JSON string containing impression data (placement, revenue, etc.)
 */
typedef void (*CBMImpressionLevelRevenueDataEvent)(int hashCode, const char* _Nullable impressionDataJson);

/**
 * Singleton observer class for managing Impression-Level Revenue Data (ILRD).
 *
 * This class handles the lifecycle of ILRD from the Chartboost Mediation SDK:
 * - Subscribes to SDK ILRD notifications
 * - Caches ILRD data both in-memory and on disk
 * - Delivers ILRD to Unity via callback
 * - Manages completion workflow to remove processed ILRD
 *
 * Usage Pattern:
 * 1. Unity sets callback via _CBMSetUnityILRDProxy()
 * 2. Observer caches incoming ILRD and notifies Unity
 * 3. Unity processes ILRD and calls _CBMCompleteUnityILRDRequest()
 * 4. Observer removes ILRD from cache
 *
 * Thread Safety:
 * All public methods and properties are thread-safe. Internal state is
 * protected by a serial dispatch queue.
 */
@interface CBMUnityILRDObserver : NSObject

/**
 * Returns the shared singleton instance of the ILRD observer.
 *
 * @return The shared CBMUnityILRDObserver instance
 */
+ (instancetype)sharedObserver;

/**
 * Controls whether cached ILRD should be automatically delivered to Unity on retrieval.
 *
 * When enabled (default), cached ILRD from previous app sessions will be delivered
 * to Unity immediately when retrieveImpressionData is called. When disabled, cached
 * ILRD remains in storage but is not automatically delivered.
 *
 * Thread-safe: Can be accessed from any thread.
 *
 * @note This property is typically set once during initialization.
 */
@property (atomic, assign) BOOL consumeILRDOnRetrieval;

/**
 * Subscribes to Chartboost Mediation ILRD notifications.
 *
 * Registers an observer for NSNotification.chartboostMediationDidReceiveILRD
 * notifications. When ILRD is received, it is automatically cached and delivered
 * to Unity (if callback is set).
 *
 * Thread-safe: Can be called from any thread.
 *
 * @note This method should be called once after setting the Unity callback.
 */
- (void)subscribeILRDObserver;

/**
 * Retrieves and optionally delivers cached ILRD from persistent storage.
 *
 * This method loads ILRD data that was cached from previous app sessions.
 * If consumeILRDOnRetrieval is enabled, all cached ILRD will be delivered
 * to Unity for processing.
 *
 * Thread-safe: Performs file I/O on a background queue.
 *
 * @note Typically called during app initialization to recover cached ILRD.
 */
- (void)retrieveImpressionData;

/**
 * Caches impression data and delivers it to Unity.
 *
 * This method is called internally when new ILRD is received from the SDK.
 * It stores the data in memory and on disk, then delivers it to Unity.
 *
 * Thread-safe: Can be called from any thread.
 *
 * @param unityILRDJson JSON string containing ILRD data in Unity-compatible format
 */
- (void)cacheImpressionData:(NSString *)unityILRDJson;

/**
 * Removes impression data from cache after Unity has processed it.
 *
 * This method should be called after Unity successfully processes ILRD data.
 * It removes the data from both memory and disk cache.
 *
 * Thread-safe: Can be called from any thread.
 *
 * @param hashCode The unique hash code identifying the ILRD to remove
 */
- (void)removeImpressionData:(int)hashCode;

/**
 * Requests Unity to consume and process ILRD data.
 *
 * This method delivers ILRD to Unity via the registered callback and sets up
 * a completion handler to remove the data when Unity finishes processing.
 *
 * Thread-safe: Can be called from any thread.
 *
 * @param unityILRDJson JSON string containing ILRD data to deliver
 */
- (void)requestUnityILRDConsumption:(NSString *)unityILRDJson;

/**
 * Completes the processing of an ILRD request from Unity.
 *
 * This method should be called by Unity after it has successfully processed
 * an ILRD data entry. It triggers the completion handler which removes the
 * ILRD from the cache.
 *
 * Thread-safe: Can be called from any thread.
 *
 * @param hashCode The hash code identifying the ILRD that was processed
 */
- (void)completeILRDRequest:(int)hashCode;

/**
 * Sets the callback function for delivering ILRD to Unity.
 *
 * This property stores the function pointer that will be invoked when
 * ILRD data needs to be delivered to the Unity C# layer.
 *
 * Thread-safe: Can be set from any thread.
 *
 * @note Must be set before subscribeILRDObserver is called.
 */
@property (atomic, assign, nullable) CBMImpressionLevelRevenueDataEvent onImpression;

@end

NS_ASSUME_NONNULL_END
