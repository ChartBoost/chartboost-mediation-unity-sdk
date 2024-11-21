#import <Foundation/Foundation.h>
#import "CBMBannerAdWrapper.h"

NSString* const CBMBannerAdWrapperTAG = @"CBMBannerAdWrapper";

@implementation CBMBannerAdWrapper{
    CGPoint _internalPivot;
    CGPoint _internalPosition;
    CGSize  _internalSize;
    CGSize  _size;
    BOOL _dragging;
}

#pragma mark Ad
- (instancetype)initWithBannerView:(CBMBannerAdView *)bannerView dragListener:(CBMExternBannerAdDragEvent)dragListener{
    self = [super init];
    _dragListener = dragListener;
    _bannerView = bannerView;

    _size = CGSizeMake(-1, -1);
    _internalPivot = CGPointMake(0, 0);
    _internalSize = _bannerView.frame.size;
    _internalPosition = _bannerView.frame.origin;

    // Add it to UnityViewController
    UIViewController *unityVC = GetAppController().rootViewController;
    [bannerView removeFromSuperview];
    [unityVC.view addSubview:bannerView];

    // Add PanGestureRecognizer for drag
    UIPanGestureRecognizer* pgr = [[UIPanGestureRecognizer alloc] initWithTarget:self action:@selector(handlePan:)];
    [_bannerView addGestureRecognizer:pgr];
    [_bannerView layoutIfNeeded];
    return self;
}

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

- (void)setAdBackgroundColor:(UIColor *)adBackgroundColor {
    toMain(^{
        if (self->_bannerView.subviews.count > 0) {
            UIView *partnerAd = self->_bannerView.subviews[0];
            [partnerAd setBackgroundColor:adBackgroundColor];
        }
    });
}

- (void)setAdRelativePosition:(CGPoint)adRelativePosition {
    toMain(^{
        if (self->_bannerView.subviews.count > 0) {
            UIView *partnerAd = self->_bannerView.subviews[0];
            
            CGRect frame = partnerAd.frame;
            frame.origin = adRelativePosition;
            partnerAd.frame = frame;
        }
    });
}

- (CGPoint)adRelativePosition {
    if (self->_bannerView.subviews.count > 0) {
        UIView *partnerAd = self->_bannerView.subviews[0];
        return partnerAd.frame.origin;
    }
    return CGPointMake(0, 0);
}

#pragma mark Container

- (void)setContainerSize:(CGSize)size{

    [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG log:[NSString stringWithFormat:@"setContainerSize : %@", NSStringFromCGSize(size)] logLevel:CBLLogLevelDebug];

    // Fit Both
    if(size.width == -1 && size.height == -1)
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG log:@"Setting container size to wrap content" logLevel:CBLLogLevelDebug];

    // Fit Horizontal
    else if(size.width == -1)
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG log:@"Setting container size to wrap horizontal" logLevel:CBLLogLevelDebug];

    // Fit Vertical
    else if(size.height == -1)
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG log:@"Setting container size to wrap vertical" logLevel:CBLLogLevelDebug];

    // Fixed size
    else
        [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG log:[NSString stringWithFormat:@"Setting container size to fixed size (%f, %f)", size.width, size.height] logLevel:CBLLogLevelDebug];

    _size = size;
    [self updateFrame];
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
    [_bannerView setHorizontalAlignment:horizontalAlignment];
}

- (CBMBannerHorizontalAlignment)horizontalAlignment{
    return [_bannerView horizontalAlignment];
}

- (void)setVerticalAlignment:(CBMBannerVerticalAlignment)verticalAlignment{
    [_bannerView setVerticalAlignment:verticalAlignment];
}

- (CBMBannerVerticalAlignment)verticalAlignment{
    return [_bannerView verticalAlignment];
}

- (void)setVisible:(BOOL)visible{
    [_bannerView setHidden:!visible];
}

- (BOOL)isVisible{
    return [_bannerView isHidden];
}

- (void)loadWith:(CBMBannerAdLoadRequest *)request viewController:(UIViewController *)viewController completion:(void (^)(CBMBannerAdLoadResult *))completion{
   [_bannerView loadWith:request viewController:viewController completion:completion];
}

- (void)reset {
    [_bannerView reset];
}

- (void)destroy {
    toMain(^{
        [self->_bannerView removeFromSuperview];
    });
}

- (void)setContainerBackgroundColor:(UIColor *)containerBackgroundColor {
    toMain(^{
        [self-> _bannerView setBackgroundColor:containerBackgroundColor];
    });
}

#pragma mark Utils

- (void)updateFrame {
    _internalSize = CGSizeMake(_size.width == -1 ? _bannerView.size.size.width : _size.width, _size.height == -1 ? _bannerView.size.size.height : _size.height);
    CGFloat newX = _internalPosition.x - (_internalSize.width * _internalPivot.x);
    CGFloat newY = _internalPosition.y - (_internalSize.height * _internalPivot.y);
    CGRect newFrame = CGRectMake(newX, newY, _internalSize.width, _internalSize.height);
    _bannerView.frame = newFrame;
}

- (void)handlePan:(UIPanGestureRecognizer *)gr {
    if (!_draggable) {
        return;
    }

    CGPoint translation = [gr translationInView:gr.view];
    CGPoint center = gr.view.center;
    float scale = UIScreen.mainScreen.scale;

    switch (gr.state) {
        case UIGestureRecognizerStateBegan: {
            _dragging = YES;  // Start dragging
            float x = gr.view.frame.origin.x * scale;
            float y = gr.view.frame.origin.y * scale;
            _dragListener((__bridge void*)self, (int)BannerAdDragBegin, x, y);
            break;
        }
        case UIGestureRecognizerStateChanged: {
            float newX = center.x + translation.x;
            float newY = center.y + translation.y;

            float left = newX - gr.view.frame.size.width / 2;
            float right = newX + gr.view.frame.size.width / 2;
            float top = newY - gr.view.frame.size.height / 2;
            float bottom = newY + gr.view.frame.size.height / 2;

            UIEdgeInsets safeAreaInsets = UIEdgeInsetsZero;
            if (@available(iOS 11.0, *)) {
                safeAreaInsets = gr.view.superview.safeAreaInsets;
            }

            CGRect safeFrame = UIEdgeInsetsInsetRect(gr.view.superview.bounds, safeAreaInsets);

            if (left < safeFrame.origin.x || right > safeFrame.origin.x + safeFrame.size.width ||
                top < safeFrame.origin.y || bottom > safeFrame.origin.y + safeFrame.size.height) {
                [[CBLUnityLoggingBridge sharedLogger] logWithTag:CBMBannerAdWrapperTAG log:@"Outside safe area" logLevel:CBLLogLevelDebug];
                return;
            }

            center.x = newX;
            center.y = newY;
            gr.view.center = center;
            [gr setTranslation:CGPointZero inView:gr.view];

            float x = gr.view.frame.origin.x * scale;
            float y = gr.view.frame.origin.y * scale;
            _dragListener((__bridge void*)self, (int)BannerAdDrag, x, y);
            break;
        }
        case UIGestureRecognizerStateEnded:
        case UIGestureRecognizerStateCancelled: {
            _dragging = NO;  // End dragging
            float x = gr.view.frame.origin.x * scale;
            float y = gr.view.frame.origin.y * scale;
            _dragListener((__bridge void*)self, (int)BannerAdDragEnd, x, y);
            break;
        }
        default:
            break;
    }
}

void toMain(block block) {
    dispatch_async(dispatch_get_main_queue(), block);
}

@end

