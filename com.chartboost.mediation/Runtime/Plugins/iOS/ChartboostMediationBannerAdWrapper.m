//
//  ChartboostMediationBannerAdWrapper.m
//  Unity-iPhone
//
//  Created by Kushagra Gupta on 6/21/23.
//

#import "ChartboostMediationBannerAdWrapper.h"


@implementation ChartboostMediationBannerAdWrapper

- (instancetype)initWithBannerView:(ChartboostMediationBannerView *)bannerView andDragListener:(ChartboostMediationBannerAdDragEvent)dragListener{
    self.bannerView = bannerView;
    
    self.panGesture = [[UIPanGestureRecognizer alloc] initWithTarget:self action:@selector(handlePan:)];
    [self.bannerView addGestureRecognizer:self.panGesture];
        
    self.dragListener = dragListener;
    self.canDrag = true;    // default is true
    
    NSLog(@"Wrapper created");
    return self;
}

- (void)createBannerRequestContainer:(float)width height:(float)height {
    self.bannerRequestContainer = [[ChartboostMediationBannerRequestContainer alloc] init:width height:height andBannerView:self.bannerView];
}

- (void)createBannerRequestContainerWithXY:(float)x y:(float)y width:(float)width height:(float)height {
    self.bannerRequestContainer = [[ChartboostMediationBannerRequestContainer alloc] initWithXY:x y:y width:width height:height andBannerView:self.bannerView];
}

- (void)setDraggable: (BOOL) canDrag{
    self.canDrag = canDrag;
    [self.bannerView removeGestureRecognizer:self.panGesture];

    if(self.canDrag)
        [self.bannerView addGestureRecognizer:self.panGesture];
}

- (void) resize:(int)axis pivotX:(float) pivotX pivotY:(float)pivotY {
    [self.bannerRequestContainer resize:_bannerView.size.size axis:axis pivotX:pivotX pivotY:pivotY];
}

- (void)handlePan:(UIPanGestureRecognizer *)gr{
    if(!self.canDrag)
        return;
        
    CGPoint translation = [gr translationInView:gr.view.superview];
    CGPoint center = gr.view.center;
    center.x += translation.x;
    center.y += translation.y;
    gr.view.center = center;
    [gr setTranslation:CGPointZero inView:gr.view.superview];
        
    float scale = UIScreen.mainScreen.scale;
    float x = gr.view.frame.origin.x * scale;
    float y = gr.view.frame.origin.y * scale;
        
    self.dragListener((__bridge void*)self, x, y);
}
@end
