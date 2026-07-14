#import <UIKit/UIKit.h>
#import "CBMDelegates.h"
#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"

/**
 * Wrapper class for Chartboost Mediation banner ads in Unity iOS.
 *
 * This class bridges Unity C# banner ad functionality to the native iOS SDK,
 * managing banner view lifecycle, positioning, dragging, and configuration.
 * Instances are created and managed from the Unity C# layer via P/Invoke.
 */
@interface CBMBannerAdWrapper : NSObject<UIGestureRecognizerDelegate>

/// The underlying Chartboost Mediation banner ad view
@property (nonatomic, strong, nonnull) CBMBannerAdView* bannerView;

/// Callback for drag events, invoked during banner drag gestures (C function pointer)
@property (nonatomic, nonnull) CBMExternBannerAdDragEvent dragListener;

/// Keywords for ad targeting
@property (nonatomic, copy, nullable) NSDictionary<NSString *, NSString *> *keywords;

/// Partner-specific settings for mediation adapters
@property (nonatomic, copy, nullable) NSDictionary<NSString *, id> *partnerSettings;

#pragma mark Container Properties

/// Controls banner visibility (hidden state)
@property (nonatomic) BOOL visible;

/// Enables or disables drag gestures on the banner
@property (nonatomic) BOOL draggable;

/// Position of the banner in screen coordinates
@property (nonatomic) CGPoint position;

/// Pivot point for positioning calculations (0-1 range)
@property (nonatomic) CGPoint pivot;

/// Size of the banner container (-1 for wrap content)
@property (nonatomic) CGSize containerSize;

/// Horizontal alignment of the ad within the container
@property (nonatomic) CBMBannerHorizontalAlignment horizontalAlignment;

/// Vertical alignment of the ad within the container
@property (nonatomic) CBMBannerVerticalAlignment verticalAlignment;

/// Background color of the banner container view
@property (nonatomic, copy, nonnull) UIColor* containerBackgroundColor;

#pragma mark Banner Properties

/// Unique identifier for the current ad load
@property (nonatomic, copy, nullable) NSString* loadId;

/// Metrics data from the ad load
@property (nonatomic, copy, nullable) NSDictionary<NSString *, id> *loadMetrics;

/// Winning bid information from the auction
@property (nonatomic, copy, nullable) NSDictionary<NSString *, id> *winningBidInfo;

/// The request used to load the current ad
@property (nonatomic, strong, nullable) CBMBannerAdLoadRequest* request;

/// Background color of the partner ad view
@property (nonatomic, copy, nonnull) UIColor* adBackgroundColor;

/// Relative position of the partner ad within the container
@property (nonatomic) CGPoint adRelativePosition;

#pragma mark Initialization

/**
 * Initializes a banner ad wrapper with the provided banner view and drag listener.
 *
 * @param bannerView The native banner ad view to wrap
 * @param dragListener Callback for drag gesture events
 * @return Initialized banner wrapper instance
 */
- (instancetype _Nonnull)initWithBannerView:(CBMBannerAdView* _Nonnull)bannerView
                               dragListener:(CBMExternBannerAdDragEvent _Nonnull)dragListener;

#pragma mark Ad Loading

/**
 * Loads a banner ad with the specified request.
 *
 * @param request The ad load request with placement and configuration
 * @param viewController The view controller for presenting the ad
 * @param completion Completion handler called when load finishes
 */
- (void)loadWith:(CBMBannerAdLoadRequest *_Nonnull)request
  viewController:(UIViewController *_Nonnull)viewController
      completion:(void (^_Nonnull)(CBMBannerAdLoadResult * _Nonnull))completion;

#pragma mark Hosting

/**
 * Reparents the banner into the given host view. Pass nil to restore the default
 * Unity root view. The banner's screen-space position is resolved relative to the
 * host, so it renders correctly regardless of where it is embedded.
 *
 * @param hostView The view to host the banner in, or nil for the Unity root view
 */
- (void)setHostView:(UIView * _Nullable)hostView;

#pragma mark Lifecycle

/**
 * Resets the banner ad to allow loading a new ad.
 */
- (void)reset;

/**
 * Destroys the banner ad and releases resources.
 * The wrapper should not be used after calling this method.
 */
- (void)destroy;

#pragma mark Private

/**
 * Updates the banner frame based on position, pivot, and size.
 */
- (void)updateFrame;

@end
