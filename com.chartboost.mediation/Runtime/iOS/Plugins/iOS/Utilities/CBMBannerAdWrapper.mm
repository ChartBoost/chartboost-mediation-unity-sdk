#import <Foundation/Foundation.h>
#import "CBMBannerAdWrapper.h"

NSString* const CBMBannerAdWrapperTAG = @"CBMBannerAdWrapper";

@implementation CBMBannerAdWrapper{
    CGPoint _internalPivot;
    CGPoint _internalPosition;
    CGSize  _internalSize;
    CGSize  _size;
    BOOL _dragging;
    BOOL _isMatchAdMode;
    BOOL _pendingVisible;
    UIPanGestureRecognizer* _panGestureRecognizer;
}

#pragma mark - Initialization

- (instancetype)initWithBannerView:(CBMBannerAdView *)bannerView dragListener:(CBMExternBannerAdDragEvent)dragListener{
    self = [super init];
    if (!self) {
        return nil;
    }

    // Validate required parameters
    if (!bannerView || !dragListener) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"initWithBannerView: bannerView or dragListener is nil"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    _dragListener = dragListener;
    _bannerView = bannerView;

    _size = CGSizeMake(-1, -1);
    _internalPivot = CGPointMake(0, 0);
    _internalSize = _bannerView.frame.size;
    _internalPosition = _bannerView.frame.origin;
    _isMatchAdMode = NO;
    _pendingVisible = !bannerView.isHidden;

    // Add it to UnityViewController with null check
    UIViewController *unityVC = GetAppController().rootViewController;
    if (!unityVC) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"initWithBannerView: Unity root view controller is nil"
                                                 logLevel:CBLLogLevelError];
        return nil;
    }

    // All UI operations must be performed on the main thread
    dispatch_async(dispatch_get_main_queue(), ^{
        [bannerView removeFromSuperview];
        [unityVC.view addSubview:bannerView];

        // Disable user interaction until ad content loads (willAppear).
        // Without this, the empty banner view's gesture recognizer intercepts
        // touches after a load failure, causing input offset issues.
        bannerView.userInteractionEnabled = NO;

        // Add PanGestureRecognizer for drag
        UIPanGestureRecognizer *panGesture = [[UIPanGestureRecognizer alloc] initWithTarget:self action:@selector(handlePan:)];
        self->_panGestureRecognizer = panGesture;
        [self->_bannerView addGestureRecognizer:panGesture];
        [self->_bannerView layoutIfNeeded];
    });
    return self;
}

#pragma mark - Hosting

- (void)setHostView:(UIView *)hostView {
    toMain(^{
        if (!self->_bannerView) {
            return;
        }

        UIView *target = hostView ?: GetAppController().rootViewController.view;
        if (!target || self->_bannerView.superview == target) {
            return;
        }

        [self->_bannerView removeFromSuperview];
        [target addSubview:self->_bannerView];
        [self updateFrame];
    });
}

- (void)dealloc {
    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                  log:@"dealloc: cleaning up banner wrapper resources"
                                             logLevel:CBLLogLevelVerbose];

    // UI operations must be performed on main thread
    // Use dispatch_sync to ensure cleanup completes before dealloc finishes
    if ([NSThread isMainThread]) {
        // Already on main thread, perform cleanup directly
        if (_panGestureRecognizer && _bannerView) {
            [_bannerView removeGestureRecognizer:_panGestureRecognizer];
            _panGestureRecognizer = nil;
        }

        if (_bannerView) {
            [_bannerView removeFromSuperview];
            _bannerView = nil;
        }
    } else {
        // Not on main thread, dispatch synchronously to ensure cleanup completes
        dispatch_sync(dispatch_get_main_queue(), ^{
            if (self->_panGestureRecognizer && self->_bannerView) {
                [self->_bannerView removeGestureRecognizer:self->_panGestureRecognizer];
                self->_panGestureRecognizer = nil;
            }

            if (self->_bannerView) {
                [self->_bannerView removeFromSuperview];
                self->_bannerView = nil;
            }
        });
    }

    // Clear all references
    _dragListener = nil;
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

    // Log the sizing mode for debugging
    if (size.width == -1 && size.height == -1) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"Setting container size to wrap content"
                                                 logLevel:CBLLogLevelVerbose];
    } else if (size.width == -1) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"Setting container size to wrap horizontal"
                                                 logLevel:CBLLogLevelVerbose];
    } else if (size.height == -1) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"Setting container size to wrap vertical"
                                                 logLevel:CBLLogLevelVerbose];
    } else {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:[NSString stringWithFormat:@"Setting container size to fixed size (%f, %f)", size.width, size.height]
                                                 logLevel:CBLLogLevelVerbose];
    }

    // Capture values for main thread block
    BOOL isWrapContent = (size.width == -1 || size.height == -1);

    _size = size;

    // Update _isMatchAdMode on main thread to ensure thread-safe access
    // This flag is read by alignment setters which also run on main thread
    toMain(^{
        self->_isMatchAdMode = isWrapContent;
    });

    [self updateFrame];
    [self updateClipSettings];
}

- (CGSize)containerSize{
    return _internalSize;
}

- (void)setPosition:(CGPoint)position{
    _internalPosition = position;
    [self updateFrame];
}

- (CGPoint)position{
    return _internalPosition;
}

- (void)setPivot:(CGPoint)pivot{
    _internalPivot = pivot;
    [self updateFrame];
}

- (CGPoint)pivot{
    return _internalPivot;
}

- (void)setHorizontalAlignment:(CBMBannerHorizontalAlignment)horizontalAlignment{
    toMain(^{
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

- (void)setVisible:(BOOL)visible{
    // Track intended state synchronously so the getter always returns the
    // most recently requested value. The UIKit update is dispatched async
    // to the main thread, but callers on the Unity thread see the new state
    // immediately — preventing the race condition where rapid toggles cause
    // stale reads between async dispatches.
    _pendingVisible = visible;
    toMain(^{
        if (self->_bannerView) {
            [self->_bannerView setHidden:!visible];
        }
    });
}

- (BOOL)visible{
    return _pendingVisible;
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
        self->_isMatchAdMode = NO;  // Reset mode for next load (set on main thread for thread safety)
        if (self->_bannerView) {
            // Disable user interaction when ad content is cleared.
            // Will be re-enabled in willAppear when new content loads.
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
        // Remove gesture recognizer
        if (self->_panGestureRecognizer && self->_bannerView) {
            [self->_bannerView removeGestureRecognizer:self->_panGestureRecognizer];
            self->_panGestureRecognizer = nil;
        }

        // Remove from superview
        if (self->_bannerView) {
            [self->_bannerView removeFromSuperview];
            self->_bannerView = nil;
        }

        // Clear references
        self->_dragListener = nil;
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

- (void)updateClipSettings {
    toMain(^{
        if (!self->_bannerView) {
            return;
        }

        self->_bannerView.clipsToBounds = !self->_isMatchAdMode;
    });
}

- (void)positionPartnerAdInContainer {
    if (!_bannerView || _bannerView.subviews.count == 0) {
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

- (void)updateFrame {
    if (!_bannerView) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"updateFrame: bannerView is nil"
                                                 logLevel:CBLLogLevelVerbose];
        return;
    }

    toMain(^{
        if (!self->_bannerView) {
            return;
        }

        // Calculate internal size based on wrap content or fixed size
        CGFloat width = self->_size.width == -1 ? self->_bannerView.size.size.width : self->_size.width;
        CGFloat height = self->_size.height == -1 ? self->_bannerView.size.size.height : self->_size.height;
        self->_internalSize = CGSizeMake(width, height);

        // Calculate position with pivot offset
        CGFloat newX = self->_internalPosition.x - (self->_internalSize.width * self->_internalPivot.x);
        CGFloat newY = self->_internalPosition.y - (self->_internalSize.height * self->_internalPivot.y);

        CGRect newFrame = CGRectMake(newX, newY, self->_internalSize.width, self->_internalSize.height);

        // Resolve the screen-space position into the host view: no-op for the full-screen
        // root, slot-relative when hosted in the automation slot.
        UIView *host = self->_bannerView.superview;
        if (host) {
            CGPoint hostOrigin = [host convertPoint:CGPointZero toView:nil];
            newFrame.origin.x -= hostOrigin.x;
            newFrame.origin.y -= hostOrigin.y;
        }
        self->_bannerView.frame = newFrame;

        if (!self->_isMatchAdMode) {
            [self positionPartnerAdInContainer];
        }
    });
}

- (void)handlePan:(UIPanGestureRecognizer *)gr {
    // Check if dragging is enabled
    if (!_draggable) {
        return;
    }

    // Validate gesture recognizer and view
    if (!gr || !gr.view) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"handlePan: gesture recognizer or view is nil"
                                                 logLevel:CBLLogLevelVerbose];
        return;
    }

    // Validate superview for safe area calculations
    if (!gr.view.superview) {
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                      log:@"handlePan: superview is nil"
                                                 logLevel:CBLLogLevelVerbose];
        return;
    }

    CGPoint translation = [gr translationInView:gr.view];
    CGPoint center = gr.view.center;
    float scale = UIScreen.mainScreen.scale;

    switch (gr.state) {
        case UIGestureRecognizerStateBegan: {
            _dragging = YES;
            float x = gr.view.frame.origin.x * scale;
            float y = gr.view.frame.origin.y * scale;

            if (_dragListener) {
                _dragListener((intptr_t)(__bridge void*)self, CBMBannerAdEventDragBegin, x, y);
            }
            break;
        }
        case UIGestureRecognizerStateChanged: {
            float newX = center.x + translation.x;
            float newY = center.y + translation.y;

            float bannerWidth = gr.view.frame.size.width;
            float bannerHeight = gr.view.frame.size.height;

            float left = newX - bannerWidth / 2;
            float right = newX + bannerWidth / 2;
            float top = newY - bannerHeight / 2;
            float bottom = newY + bannerHeight / 2;

            // Get safe area insets (iOS 11+)
            UIEdgeInsets safeAreaInsets = UIEdgeInsetsZero;
            if (@available(iOS 11.0, *)) {
                safeAreaInsets = gr.view.superview.safeAreaInsets;
            }

            CGRect safeFrame = UIEdgeInsetsInsetRect(gr.view.superview.bounds, safeAreaInsets);

            BOOL outsideBounds;

            if (bannerWidth > safeFrame.size.width) {
                // Banner is wider than screen: allow dragging beyond borders but require
                // at least 60% of the banner to remain visible horizontally.
                float minVisibleH = fminf(bannerWidth * 0.6f, safeFrame.size.width);
                float leftBound = safeFrame.origin.x - (bannerWidth - minVisibleH);
                float rightBound = safeFrame.origin.x + safeFrame.size.width - minVisibleH;
                outsideBounds = left < leftBound || left > rightBound ||
                                top < safeFrame.origin.y || bottom > safeFrame.origin.y + safeFrame.size.height;
            } else {
                // Banner fits within screen: keep fully within safe area.
                outsideBounds = left < safeFrame.origin.x || right > safeFrame.origin.x + safeFrame.size.width ||
                                top < safeFrame.origin.y || bottom > safeFrame.origin.y + safeFrame.size.height;
            }

            if (outsideBounds) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG
                                                              log:@"handlePan: position outside allowed bounds"
                                                         logLevel:CBLLogLevelVerbose];
                return;
            }

            // Update position
            center.x = newX;
            center.y = newY;
            gr.view.center = center;
            [gr setTranslation:CGPointZero inView:gr.view];

            float x = gr.view.frame.origin.x * scale;
            float y = gr.view.frame.origin.y * scale;

            if (_dragListener) {
                _dragListener((intptr_t)(__bridge void*)self, CBMBannerAdEventDrag, x, y);
            }
            break;
        }
        case UIGestureRecognizerStateEnded:
        case UIGestureRecognizerStateCancelled: {
            _dragging = NO;
            float x = gr.view.frame.origin.x * scale;
            float y = gr.view.frame.origin.y * scale;

            // Persist the dragged position so it survives ad refreshes (updateFrame)
            // _internalPosition includes pivot offset, so add it back from the frame origin
            _internalPosition = CGPointMake(
                gr.view.frame.origin.x + (_internalSize.width * _internalPivot.x),
                gr.view.frame.origin.y + (_internalSize.height * _internalPivot.y)
            );

            if (_dragListener) {
                _dragListener((intptr_t)(__bridge void*)self, CBMBannerAdEventDragEnd, x, y);
            }
            break;
        }
        default:
            break;
    }
}

@end

