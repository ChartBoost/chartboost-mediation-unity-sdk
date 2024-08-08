#import <Foundation/Foundation.h>
#import "CBMBannerAdWrapper.h"


@implementation CBMBannerAdWrapper{
    CGPoint _internalPivot;
    CGPoint _internalPosition;
    CGSize  _internalSize;
    CGSize  _size;
}

#pragma mark Ad
- (instancetype)initWithBannerView:(CBMBannerAdView *)bannerView dragListener:(CBMExternBannerAdDragEvent)dragListener{
    self = [super init];
    self.dragListener = dragListener;
    self.bannerView = bannerView;
    
    _size = CGSizeMake(-1, -1);
    _internalPivot = CGPointMake(0, 0);
    _internalSize = self.bannerView.frame.size;
    _internalPosition = self.bannerView.frame.origin;
    
    // Add it to UnityViewController
    UIViewController *unityVC = GetAppController().rootViewController;
    [bannerView removeFromSuperview];
    [unityVC.view addSubview:bannerView];
    
    // Add PanGestureRecognizer for drag
    UIPanGestureRecognizer* pgr = [[UIPanGestureRecognizer alloc] initWithTarget:self action:@selector(handlePan:)];
    [self.bannerView addGestureRecognizer:pgr];
    
    [self.bannerView layoutIfNeeded];
    
    return self;
}

- (void)setKeywords:(NSDictionary<NSString *,NSString *> *)keywords{
    [self.bannerView setKeywords:keywords];
}

- (NSDictionary<NSString *,NSString *> *)keywords{
    return [self.bannerView keywords];
}

- (void)setPartnerSettings:(NSDictionary<NSString *,id> *)partnerSettings{
    [self.bannerView setPartnerSettings:partnerSettings];
}

- (NSDictionary<NSString *,id> *)partnerSettings{
    return [self.bannerView partnerSettings];
}

- (CBMBannerSize *)bannerSize{
    return [self.bannerView size];
}

#pragma mark Container

- (void)setContainerSize:(CGSize)size{
    NSLog(@"%@ setContainerSize : %@", TAG, NSStringFromCGSize(size));
    
    // Fit Both
    if(size.width == -1 && size.height == -1) 
        NSLog(@"%@ Setting container size to wrap content", TAG);
    
    // Fit Horizontal
    else if(size.width == -1) 
        NSLog(@"%@ Setting container size to wrap horizontal", TAG);
    
    // Fit Vertical
    else if(size.height == -1) 
        NSLog(@"%@ Setting container size to wrap vertical", TAG);
    
    // Fixed size
    else 
        NSLog(@"%@ Setting container size to fixed size (%f, %f)", TAG, size.width, size.height);
    
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
    [self.bannerView setHorizontalAlignment:horizontalAlignment];
}

- (CBMBannerHorizontalAlignment)horizontalAlignment{
    return [self.bannerView horizontalAlignment];
}

- (void)setVerticalAlignment:(CBMBannerVerticalAlignment)verticalAlignment{
    [self.bannerView setVerticalAlignment:verticalAlignment];
}

- (CBMBannerVerticalAlignment)verticalAlignment{
    return [self.bannerView verticalAlignment];
}

- (void)setVisible:(BOOL)visible{
    [self.bannerView setHidden:!visible];
}

- (BOOL)isVisible{
    return [self.bannerView isHidden];
}

- (void)loadWith:(CBMBannerAdLoadRequest *)request viewController:(UIViewController *)viewController completion:(void (^)(CBMBannerAdLoadResult *))completion{
   [self.bannerView loadWith:request viewController:viewController completion:completion];
}

- (void)reset {
    [self.bannerView reset];
}

- (void)destroy {
    toMain(^{
        [self.bannerView removeFromSuperview];
    });
}

#pragma mark Utils

- (void)updateFrame {
    
    _internalSize = CGSizeMake(_size.width == -1 ? self.bannerView.size.size.width : _size.width, _size.height == -1 ? self.bannerView.size.size.height : _size.height);
    
    CGFloat newX = _internalPosition.x - (_internalSize.width * _internalPivot.x);
    CGFloat newY = _internalPosition.y - (_internalSize.height * _internalPivot.y);
    CGRect newFrame = CGRectMake(newX, newY, _internalSize.width, _internalSize.height);
    self.bannerView.frame = newFrame;
    
    NSLog(@"%@ Updated Frame => %@", TAG, NSStringFromCGRect(self.bannerView.frame));
}

- (void)handlePan:(UIPanGestureRecognizer *)gr{
    if(!self.draggable)
            return;

    CGPoint translation = [gr translationInView:gr.view];
    CGPoint center = gr.view.center;

    float newX = center.x + translation.x;
    float newY = center.y + translation.y;

    float left = newX - gr.view.frame.size.width/2;
    float right = newX + gr.view.frame.size.width/2;
    float top = newY - gr.view.frame.size.height/2;
    float bottom = newY + gr.view.frame.size.height/2;

    // Get safe area insets
    UIEdgeInsets safeAreaInsets;
    if (@available(iOS 11.0, *)) {
        safeAreaInsets = gr.view.superview.safeAreaInsets;
    } else {
        safeAreaInsets = UIEdgeInsetsZero;
    }

    CGRect safeFrame = UIEdgeInsetsInsetRect(gr.view.superview.bounds, safeAreaInsets);

    // do not move any part of the banner out of the safe area
    if(left < safeFrame.origin.x || right > safeFrame.origin.x + safeFrame.size.width ||
       top < safeFrame.origin.y || bottom > safeFrame.origin.y + safeFrame.size.height)
    {
        NSLog(@"outside safe area");
        return;
    }

    center.x = newX;
    center.y = newY;

    gr.view.center = center;
    [gr setTranslation:CGPointZero inView:gr.view];

    float scale = UIScreen.mainScreen.scale;
    float x = gr.view.frame.origin.x * scale;
    float y = gr.view.frame.origin.y * scale;

    self.dragListener((__bridge void*)self, x, y);
}

void toMain(block block) {
    dispatch_async(dispatch_get_main_queue(), block);
}

@end

