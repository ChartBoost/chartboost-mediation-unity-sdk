#import <UIKit/UIKit.h>
#import "CBMDelegates.h"
#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"

/**
 * Wrapper class for Chartboost Mediation banner ads in Unity iOS.
 *
 * This class bridges Unity C# banner ad functionality to the native iOS SDK. The banner view lives in
 * a shared CBUBannerContainer that owns placement, visibility, drag and hosting; this wrapper owns the
 * ad and how the partner creative sits inside the banner view.
 * Instances are created and managed from the Unity C# layer via P/Invoke.
 */
@interface CBMBannerAdWrapper : NSObject

/// The underlying Chartboost Mediation banner ad view
@property (nonatomic, strong, nonnull) CBMBannerAdView* bannerView;

/// Keywords for ad targeting
@property (nonatomic, copy, nullable) NSDictionary<NSString *, NSString *> *keywords;

/// Partner-specific settings for mediation adapters
@property (nonatomic, copy, nullable) NSDictionary<NSString *, id> *partnerSettings;

#pragma mark Container Properties

/// Sizes the banner view inside its container (-1 on an axis wraps the creative and lets it overflow)
- (void)setContainerSize:(CGSize)size;

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
 * Initializes a banner ad wrapper with the provided banner view.
 *
 * @param bannerView The native banner ad view to wrap
 * @return Initialized banner wrapper instance
 */
- (instancetype _Nonnull)initWithBannerView:(CBMBannerAdView* _Nonnull)bannerView;

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
 * Gives the container the loaded creative's size and aligns the creative inside the banner view.
 */
- (void)updateLayout;

/// A new creative is about to show: it starts from the alignment until AdRelativePosition pins it again.
- (void)creativeWillAppear;

@end
