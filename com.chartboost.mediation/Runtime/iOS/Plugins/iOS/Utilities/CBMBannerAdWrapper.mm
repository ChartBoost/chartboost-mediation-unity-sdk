#import <Foundation/Foundation.h>
#import "CBMBannerAdWrapper.h"
#import "CBUBannerContainer.h"         // shared banner host from com.chartboost.unity.utilities

NSString* const CBMBannerAdWrapperTAG = @"CBMBannerAdWrapper";

@implementation CBMBannerAdWrapper{
    BOOL _isMatchAdMode;
    BOOL _isCreativePinned; // set when AdRelativePosition places the creative; cleared by an alignment change
}

#pragma mark - Initialization

- (instancetype)initWithBannerView:(CBMBannerAdView *)bannerView{
    self = [super init];
    if (!self) {
        return nil;
    }

    if (!bannerView) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"initWithBannerView: bannerView is nil"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    _bannerView = bannerView;
    _isMatchAdMode = NO;

    // No touches until the ad appears (CBMUnityObserver re-enables them), so the empty view never blocks Unity.
    toMain(^{
        bannerView.userInteractionEnabled = NO;
    });
    return self;
}

- (void)dealloc {
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                  log:@"dealloc: cleaning up banner wrapper resources"
                                             logLevel:CBLLogLevelVerbose];
    CBMBannerAdView *bannerView = _bannerView;
    _bannerView = nil;
    if (bannerView) {
        toMain(^{
            [bannerView removeFromSuperview];
        });
    }
    _request = nil;
    _loadId = nil;
    _loadMetrics = nil;
    _winningBidInfo = nil;
}

#pragma mark - Keywords and Partner Settings

- (void)setKeywords:(NSDictionary<NSString *,NSString *> *)keywords{
    [_bannerView setKeywords:keywords];
}

- (NSDictionary<NSString *,NSString *> *)keywords{
    return [_bannerView keywords];
}

- (void)setPartnerSettings:(NSDictionary<NSString *, id> *)partnerSettings{
    [_bannerView setPartnerSettings:partnerSettings];
}

- (NSDictionary<NSString *,id> *)partnerSettings{
    return [_bannerView partnerSettings];
}

#pragma mark - Banner Ad Properties

- (void)setAdBackgroundColor:(UIColor *)adBackgroundColor {
    if (!adBackgroundColor) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"setAdBackgroundColor: color is nil"
                                                 logLevel:CBLLogLevelVerbose];
        return;
    }

    toMain(^{
        if (!self->_bannerView) {
            return;
        }

        if (self->_bannerView.subviews.count > 0) {
            UIView *partnerAd = self->_bannerView.subviews[0];
            [partnerAd setBackgroundColor:adBackgroundColor];
        }
    });
}

- (void)setAdRelativePosition:(CGPoint)adRelativePosition {
    _isCreativePinned = YES;
    toMain(^{
        if (!self->_bannerView) {
            return;
        }

        if (self->_bannerView.subviews.count > 0) {
            UIView *partnerAd = self->_bannerView.subviews[0];

            CGRect frame = partnerAd.frame;
            frame.origin = adRelativePosition;
            partnerAd.frame = frame;
        }
    });
}

- (CGPoint)adRelativePosition {
    if (!_bannerView) {
        return CGPointMake(0, 0);
    }

    if (_bannerView.subviews.count > 0) {
        UIView *partnerAd = _bannerView.subviews[0];
        return partnerAd.frame.origin;
    }
    return CGPointMake(0, 0);
}

#pragma mark - Container Properties

- (void)setContainerSize:(CGSize)size{
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                  log:[NSString stringWithFormat:@"setContainerSize: %@", NSStringFromCGSize(size)]
                                             logLevel:CBLLogLevelVerbose];
    BOOL isMatchAd = (size.width == -1 || size.height == -1);
    toMain(^{
        self->_isMatchAdMode = isMatchAd;
        // Match Ad: the creative may draw past the banner view instead of being clipped.
        self->_bannerView.clipsToBounds = !isMatchAd;
    });
    [self updateLayout];
}

// An alignment change hands placement back from AdRelativePosition; re-sets of the same one are no-ops (callers sync per frame).
- (BOOL)takeAlignment:(BOOL)changed{
    if (changed)
        _isCreativePinned = NO;
    return changed;
}

- (void)setHorizontalAlignment:(CBMBannerHorizontalAlignment)horizontalAlignment{
    toMain(^{
        if (![self takeAlignment:self->_bannerView.horizontalAlignment != horizontalAlignment])
            return;
        [self->_bannerView setHorizontalAlignment:horizontalAlignment];

        // For Fixed Size mode, reposition the partner ad to reflect the new alignment
        if (!self->_isMatchAdMode) {
            [self positionPartnerAdInContainer];
        }
    });
}

- (CBMBannerHorizontalAlignment)horizontalAlignment{
    return [_bannerView horizontalAlignment];
}

- (void)setVerticalAlignment:(CBMBannerVerticalAlignment)verticalAlignment{
    toMain(^{
        if (![self takeAlignment:self->_bannerView.verticalAlignment != verticalAlignment])
            return;
        [self->_bannerView setVerticalAlignment:verticalAlignment];

        // For Fixed Size mode, reposition the partner ad to reflect the new alignment
        if (!self->_isMatchAdMode) {
            [self positionPartnerAdInContainer];
        }
    });
}

- (CBMBannerVerticalAlignment)verticalAlignment{
    return [_bannerView verticalAlignment];
}

#pragma mark - Ad Loading

- (void)loadWith:(CBMBannerAdLoadRequest *)request viewController:(UIViewController *)viewController completion:(void (^)(CBMBannerAdLoadResult *))completion{
    if (!request || !viewController || !completion) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"loadWith: nil parameter provided"
                                                 logLevel:CBLLogLevelError];
        return;
    }

    _request = request;
    [_bannerView loadWith:request viewController:viewController completion:^(CBMBannerAdLoadResult *result) {
        if (!result) {
            [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                          log:@"loadWith: result is nil"
                                                     logLevel:CBLLogLevelError];
            return;
        }

        // Store load result data
        self->_loadId = result.loadID;
        self->_loadMetrics = result.metrics;
        self->_winningBidInfo = result.winningBidInfo;

        // Call the original completion handler
        completion(result);
    }];
}

#pragma mark - Lifecycle

- (void)reset {
    toMain(^{
        if (self->_bannerView) {
            self->_bannerView.userInteractionEnabled = NO;
            [self->_bannerView reset];
        }
    });
}

- (void)destroy {
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                  log:@"destroy: cleaning up banner ad"
                                             logLevel:CBLLogLevelVerbose];
    toMain(^{
        // The C# side disposed the container first; this only covers a finalizer-path destroy.
        [self->_bannerView removeFromSuperview];
        self->_bannerView = nil;
        self->_request = nil;
    });
}

- (void)setContainerBackgroundColor:(UIColor *)containerBackgroundColor {
    if (!containerBackgroundColor) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"setContainerBackgroundColor: color is nil"
                                                 logLevel:CBLLogLevelVerbose];
        return;
    }

    toMain(^{
        if (self->_bannerView) {
            [self->_bannerView setBackgroundColor:containerBackgroundColor];
        }
    });
}

#pragma mark - Private Utilities

- (void)positionPartnerAdInContainer {
    if (_isCreativePinned || !_bannerView || _bannerView.subviews.count == 0) {
        return;
    }

    UIView *partnerAd = _bannerView.subviews[0];
    CGFloat containerWidth = _bannerView.frame.size.width;
    CGFloat containerHeight = _bannerView.frame.size.height;
    CGFloat partnerWidth = partnerAd.frame.size.width;
    CGFloat partnerHeight = partnerAd.frame.size.height;

    if (partnerWidth <= 0 || containerWidth <= 0) {
        return;
    }

    // Calculate horizontal offset based on alignment
    CGFloat offsetX;
    switch (_bannerView.horizontalAlignment) {
        case CBMBannerHorizontalAlignmentLeft:
            offsetX = 0;
            break;
        case CBMBannerHorizontalAlignmentRight:
            offsetX = containerWidth - partnerWidth;
            break;
        case CBMBannerHorizontalAlignmentCenter:
        default:
            offsetX = (containerWidth - partnerWidth) / 2.0;
            break;
    }

    // Calculate vertical offset based on alignment
    CGFloat offsetY;
    switch (_bannerView.verticalAlignment) {
        case CBMBannerVerticalAlignmentTop:
            offsetY = 0;
            break;
        case CBMBannerVerticalAlignmentBottom:
            offsetY = containerHeight - partnerHeight;
            break;
        case CBMBannerVerticalAlignmentCenter:
        default:
            offsetY = (containerHeight - partnerHeight) / 2.0;
            break;
    }

    CGRect newFrame = partnerAd.frame;
    newFrame.origin.x = offsetX;
    newFrame.origin.y = offsetY;
    partnerAd.frame = newFrame;
}

- (void)creativeWillAppear {
    _isCreativePinned = NO;
}

- (void)updateLayout {
    toMain(^{
        [self updateLayoutAttempt:0];
    });
}

// Main thread. Retries for a few run-loop turns until the creative size is known (parity with Android).
- (void)updateLayoutAttempt:(int)attempt {
    CBMBannerAdView *bannerView = _bannerView;
    if (!bannerView) {
        return;
    }
    CGSize creative = bannerView.size.size;
    if (creative.width <= 0 || creative.height <= 0) {
        if (attempt < 10) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [self updateLayoutAttempt:attempt + 1];
            });
        }
        return;
    }
    CBUBannerContainer *container = [CBUBannerContainer containerOf:bannerView];
    // A wrapping container axis takes the creative's size.
    if (container) {
        container.contentSize = creative;
        [container layoutIfNeeded];
    }
    if (!_isMatchAdMode) {
        [self positionPartnerAdInContainer];
    }
}

@end
