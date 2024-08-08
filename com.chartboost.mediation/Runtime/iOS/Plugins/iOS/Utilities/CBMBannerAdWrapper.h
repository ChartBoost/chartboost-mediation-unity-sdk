#import <UIKit/UIKit.h>
#import "CBMDelegates.h"
#import "CBMAdStore.h"
#import "CBMUnityObserver.h"
#import "UnityAppController.h"


static NSString *const TAG = @"CBMBannerAdWrapper";


@interface CBMBannerAdWrapper : NSObject<UIGestureRecognizerDelegate>

@property CBMBannerAdView* _Nonnull bannerView;
@property CBMExternBannerAdDragEvent _Nonnull dragListener;

@property (nonatomic) NSDictionary<NSString *, NSString *> *_Nullable keywords;
@property (nonatomic) NSDictionary<NSString *, id> *_Nullable partnerSettings;

#pragma mark Container
@property (nonatomic) BOOL visible;
@property (nonatomic) BOOL draggable;
@property CGPoint position;
@property (nonatomic) CGPoint pivot;
@property (nonatomic) CGSize containerSize;
@property (nonatomic) CBMBannerHorizontalAlignment horizontalAlignment;
@property (nonatomic) CBMBannerVerticalAlignment verticalAlignment;

#pragma mark Banner
@property CBMBannerSize* _Nullable bannerSize;
@property NSString* _Nullable loadId;
@property NSDictionary<NSString *, id> *_Nullable loadMetrics;
@property NSDictionary<NSString *, id> *_Nullable winningBidInfo;
@property CBMBannerAdLoadRequest* _Nullable request;

- (instancetype _Nonnull )initWithBannerView:(CBMBannerAdView* _Nonnull) bannerView dragListener: (CBMExternBannerAdDragEvent _Nonnull) dragListener;
- (void) loadWith:(CBMBannerAdLoadRequest *) request viewController:(UIViewController *) viewController completion:(void (^)(CBMBannerAdLoadResult * )) completion;- (void)reset;
- (void)destroy;

- (void)updateFrame;
@end
