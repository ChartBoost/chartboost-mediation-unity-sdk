/**
 * @file CBMDelegates.h
 * @brief Native-to-Unity callback delegate definitions for Chartboost Mediation SDK.
 *
 * This header defines the C function pointer types used to bridge native iOS events
 * back to Unity C#. These delegates enable the native SDK to communicate ad lifecycle
 * events, load results, and user interactions to the managed C# layer.
 *
 * Event Flow: Native SDK → C callback → Unity P/Invoke → C# event handlers
 * Thread Safety: Callbacks are invoked on the main thread unless otherwise noted.
 * Memory Management: String parameters must be copied by callers; pointers are temporary.
 *
 * @see CBMUnityBridge.mm, CBMFullscreenAdBridge.mm, CBMBannerAdBridge.mm for usage
 * @version 5.0.0
 * @note Requires Unity 2020.3+ and Chartboost Mediation SDK 5.0+
 */

#import <Foundation/Foundation.h>
#import <objc/runtime.h>
#import "ChartboostUnityUtilities.h"
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <ChartboostCoreSDK/ChartboostCoreSDK-Swift.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>
#import "CBLUnityLoggingBridge.h"

NS_ASSUME_NONNULL_BEGIN

#pragma mark - Generic Data Delegates

/**
 * Generic callback for passing JSON data from native to Unity.
 *
 * This callback is used for simple data transfer operations where only
 * serialized data needs to be communicated without additional context.
 *
 * @param data JSON-serialized string data (may be NULL). Caller must copy if persistence
 *             is needed. String memory is managed by the native layer and may be
 *             deallocated immediately after callback returns.
 */
typedef void (*CBMExternDataEvent)(const char* _Nullable data);

#pragma mark - Fullscreen Ad Events

/**
 * Event types for fullscreen ad lifecycle callbacks.
 *
 * These values are passed to CBMExternFullscreenAdEvent to indicate which
 * lifecycle event occurred for a fullscreen ad.
 */
typedef NS_ENUM(NSInteger, CBMFullscreenAdEventType) {
    /// Ad impression was recorded by the mediation system
    CBMFullscreenAdEventRecordImpression = 0,

    /// User clicked on the ad
    CBMFullscreenAdEventClick = 1,

    /// User earned a reward (rewarded ads only)
    CBMFullscreenAdEventReward = 2,

    /// Ad was closed by the user or system
    CBMFullscreenAdEventClose = 3,

    /// Ad expired and can no longer be shown
    CBMFullscreenAdEventExpire = 4
};

#pragma mark - Fullscreen Ad Delegates

/**
 * Callback invoked when a fullscreen ad load operation completes (success or failure).
 *
 * This callback provides comprehensive information about the load result, including
 * metrics, bid information, and error details if the load failed.
 *
 * @param hashCode Unity-side identifier for the fullscreen ad queue/instance
 * @param adHashCode Native pointer to the loaded CBMFullscreenAd instance (NULL on failure)
 * @param loadId Unique identifier for this load operation (for tracking and metrics, may be NULL)
 * @param metricsJson JSON string containing load performance metrics (may be NULL)
 * @param bidInfoJson JSON string containing winning bid information (may be NULL)
 * @param code Error code string if load failed, NULL on success (format: "CM_XXX")
 * @param message Human-readable error message if load failed, NULL on success
 */
typedef void (*CBMExternFullscreenAdLoadResultEvent)(
    intptr_t hashCode,
    const void* _Nullable adHashCode,
    const char * _Nullable loadId,
    const char * _Nullable metricsJson,
    const char * _Nullable bidInfoJson,
    const char * _Nullable code,
    const char * _Nullable message
);

/**
 * Callback invoked when a fullscreen ad show operation completes (success or failure).
 *
 * This callback indicates whether the ad was successfully presented to the user
 * or if an error occurred during the show operation.
 *
 * @param hashCode Unity-side identifier for the fullscreen ad instance
 * @param metricsJson JSON string containing show performance metrics (may be NULL)
 * @param code Error code string if show failed, NULL on success (format: "CM_XXX")
 * @param message Human-readable error message if show failed, NULL on success
 */
typedef void (*CBMExternFullscreenAdShowResultEvent)(
    intptr_t hashCode,
    const char * _Nullable metricsJson,
    const char * _Nullable code,
    const char * _Nullable message
);

/**
 * Callback invoked for fullscreen ad lifecycle events.
 *
 * This callback handles various ad events like impressions, clicks, rewards, closes,
 * and expirations. The eventType parameter determines which event occurred.
 *
 * @param hashCode Unity-side identifier for the fullscreen ad instance
 * @param eventType Type of event that occurred (see CBMFullscreenAdEventType)
 * @param code Optional event-specific code or error code (may be NULL)
 * @param message Optional event-specific message or error description (may be NULL)
 *
 * @see CBMFullscreenAdEventType for available event types
 */
typedef void (*CBMExternFullscreenAdEvent)(
    intptr_t hashCode,
    CBMFullscreenAdEventType eventType,
    const char * _Nullable code,
    const char * _Nullable message
);

#pragma mark - Fullscreen Ad Queue Events

/**
 * Event types for fullscreen ad queue callbacks.
 *
 * These values distinguish between different queue-related events when using
 * queue-based ad loading strategies.
 */
typedef NS_ENUM(NSInteger, CBMFullscreenAdQueueEventType) {
    /// Queue was updated with a new ad or encountered a load error
    CBMFullscreenAdQueueEventUpdate = 0,

    /// An expired ad was automatically removed from the queue
    CBMFullscreenAdQueueEventRemoveExpiredAd = 1
};

#pragma mark - Fullscreen Ad Queue Delegates

/**
 * Callback invoked when a fullscreen ad queue updates (ad loaded or error occurred).
 *
 * This callback is used for queue-based ad loading, notifying Unity when new ads
 * are added to the queue or when load attempts fail.
 *
 * @param hashCode Unity-side identifier for the fullscreen ad queue
 * @param loadId Unique identifier for the load operation that triggered this update (may be NULL)
 * @param metricsJson JSON string containing load performance metrics (may be NULL)
 * @param bidInfoJson JSON string containing winning bid information (may be NULL)
 * @param code Error code string if load failed, NULL on success (format: "CM_XXX")
 * @param message Human-readable error message if load failed, NULL on success
 * @param numberOfAdsReady Current count of ready-to-show ads in the queue
 */
typedef void (*CBMExternFullscreenAdQueueUpdateEvent)(
    intptr_t hashCode,
    const char * _Nullable loadId,
    const char * _Nullable metricsJson,
    const char * _Nullable bidInfoJson,
    const char * _Nullable code,
    const char * _Nullable message,
    int numberOfAdsReady
);

/**
 * Callback invoked when an expired ad is automatically removed from the queue.
 *
 * Ads in the queue have expiration times, and this callback notifies Unity when
 * an ad expires and is removed, allowing the queue to request a replacement.
 *
 * @param hashCode Unity-side identifier for the fullscreen ad queue
 * @param numberOfAdsReady Updated count of ready-to-show ads remaining in the queue
 */
typedef void (*CBMExternFullscreenAdQueueRemoveExpiredAdEvent)(
    intptr_t hashCode,
    int numberOfAdsReady
);

#pragma mark - Banner Ad Events

/**
 * Event types for banner ad lifecycle and interaction callbacks.
 *
 * These values are passed to banner ad callbacks to indicate which type of
 * event occurred (lifecycle events or drag gesture events).
 */
typedef NS_ENUM(NSInteger, CBMBannerAdEventType) {
    /// Banner appeared in the view hierarchy and became visible
    CBMBannerAdEventAppear = 0,

    /// User clicked on the banner ad
    CBMBannerAdEventClick = 1,

    /// Banner impression was recorded by the mediation system
    CBMBannerAdEventRecordImpression = 2,

    /// User began dragging the banner (drag gesture started)
    CBMBannerAdEventDragBegin = 3,

    /// Banner is being dragged (continuous position updates)
    CBMBannerAdEventDrag = 4,

    /// User finished dragging the banner (drag gesture ended)
    CBMBannerAdEventDragEnd = 5
};

#pragma mark - Banner Ad Delegates

/**
 * Callback invoked when a banner ad load operation completes (success or failure).
 *
 * This callback provides comprehensive information about the banner load result,
 * including the loaded banner dimensions, metrics, and error details if applicable.
 *
 * @param hashCode Unity-side identifier for the banner ad wrapper
 * @param adHashCode Native pointer to the loaded banner ad wrapper instance (NULL on failure)
 * @param loadId Unique identifier for this load operation (for tracking and metrics, may be NULL)
 * @param metricsJson JSON string containing load performance metrics (may be NULL)
 * @param bidInfoJson JSON string containing winning bid information (may be NULL)
 * @param width Width of the loaded banner in screen coordinates (0.0f on failure)
 * @param height Height of the loaded banner in screen coordinates (0.0f on failure)
 * @param code Error code string if load failed, NULL on success (format: "CM_XXX")
 * @param message Human-readable error message if load failed, NULL on success
 */
typedef void (*CBMExternBannerAdLoadResultEvent)(
    intptr_t hashCode,
    const void* _Nullable adHashCode,
    const char * _Nullable loadId,
    const char * _Nullable metricsJson,
    const char * _Nullable bidInfoJson,
    float width,
    float height,
    const char * _Nullable code,
    const char * _Nullable message
);

/**
 * Callback invoked for banner ad lifecycle events.
 *
 * This callback handles various banner events like appear, click, impression recording,
 * and drag state changes. The eventType parameter determines which event occurred.
 *
 * @param hashCode Unity-side identifier for the banner ad wrapper
 * @param eventType Type of event that occurred (see CBMBannerAdEventType)
 *
 * @see CBMBannerAdEventType for available event types
 */
typedef void (*CBMExternBannerAdEvent)(
    intptr_t hashCode,
    CBMBannerAdEventType eventType
);

/**
 * Callback invoked for banner ad drag gesture events.
 *
 * This callback provides continuous position updates during banner drag operations,
 * allowing Unity to synchronize banner position state with native UI gestures.
 *
 * @param hashCode Unity-side identifier for the banner ad wrapper
 * @param eventType Type of drag event (CBMBannerAdEventDragBegin, CBMBannerAdEventDrag, or CBMBannerAdEventDragEnd)
 * @param x Current X position of the banner in screen coordinates
 * @param y Current Y position of the banner in screen coordinates
 *
 * @see CBMBannerAdEventType for drag event types (DragBegin, Drag, DragEnd)
 */
typedef void (*CBMExternBannerAdDragEvent)(
    intptr_t hashCode,
    CBMBannerAdEventType eventType,
    float x,
    float y
);

NS_ASSUME_NONNULL_END
