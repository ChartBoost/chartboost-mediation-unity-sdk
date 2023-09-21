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
    self.canDrag = false;
    
    return self;
}

- (void)setDraggable: (BOOL) canDrag{
    self.canDrag = canDrag;
    [self.bannerView removeGestureRecognizer:self.panGesture];

    if(self.canDrag)
        [self.bannerView addGestureRecognizer:self.panGesture];
}

- (void) resize:(int)axis pivotX:(float) pivotX pivotY:(float)pivotY {
    CGRect frame = _bannerView.frame;
    CGSize newSize = _bannerView.size.size;
    NSLog(@"Initial Frame => origin : (%f, %f), size : (%f, %f)", frame.origin.x, frame.origin.y, frame.size.width, frame.size.height);
    
    // if container is positioned using constraints then pivot and constraints are pretty much the same
    // so we don't make any adjustments in container's position
    if(self.usesConstraints){
        switch (axis) {
            case 0: // Horizontal
                frame.size.width = newSize.width;
                break;
            case 1: // Vertical
                frame.size.height = newSize.height;
                break;
            default: // both
                frame.size = newSize;
                break;
            }
    }
    // if no constraints are in use then we manually position it by moving it around its pivot
    else
    {
        CGSize size = frame.size;
        CGPoint origin = frame.origin;
        CGPoint pivot = CGPointMake(origin.x + (pivotX * size.width) , origin.y + (pivotY * size.height));
        NSLog(@"pivot : (%f, %f)", pivot.x, pivot.y);
        
        // Find top-left corner of newSize w.r.t pivot
        float left = pivotX * newSize.width;
        float top = pivotY * newSize.height;
        NSLog(@"Left : %f, Top : %f newSize : (%f, %f)", left, top, newSize.width, newSize.height);
        
        // Resize and move container to top-left of new size
        CGPoint topLeft = CGPointMake(pivot.x - left, pivot.y - top);
        NSLog(@"Topleft : (%f, %f)", topLeft.x, topLeft.y);
        
        switch(axis){
            case 0: // Horizontal
                frame.size.width = newSize.width;
                frame.origin.x = topLeft.x;
                break;
            case 1: // Vertical
                frame.size.height = newSize.height;
                frame.origin.y = topLeft.y;
                break;
            default: // both
                frame = CGRectMake(topLeft.x, topLeft.y, newSize.width, newSize.height);
                break;
        }
    }
    NSLog(@"Final Frame => origin : (%f, %f), size : (%f, %f)", frame.origin.x, frame.origin.y, frame.size.width, frame.size.height);
    _bannerView.frame = frame;
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
