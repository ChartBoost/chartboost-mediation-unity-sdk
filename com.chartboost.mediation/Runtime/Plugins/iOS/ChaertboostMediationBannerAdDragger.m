//
//  ChaertboostMediationBannerAdWrapper.m
//  UnityFramework
//
//  Created by Kushagra Gupta on 5/12/23.
//

#import <Foundation/Foundation.h>

#import "ChartboostMediationManager.h"
#import "ChaertboostMediationBannerAdDragger.h"

@implementation ChartboostMediationBannerAdDragger

- (instancetype)init
{
    self = [super init];
    NSLog(@"Creating dragger");
    return self;
}

- (void)handlePan:(UIPanGestureRecognizer *)gr
{
    NSLog(@"Dragging");
    CGPoint translation = [gr translationInView:gr.view.superview];
    CGPoint center = gr.view.center;
    center.x += translation.x;
    center.y += translation.y;
    gr.view.center = center;
    [gr setTranslation:CGPointZero inView:gr.view.superview];
    
    NSLog(@"BannerView frame (Drag): %f, %f, %f, %f", gr.view.frame.origin.x, gr.view.frame.origin.y, gr.view.frame.size.width, gr.view.frame.size.height );
    
    float scale = UIScreen.mainScreen.scale;
    float x = gr.view.frame.origin.x * scale;
    float y = gr.view.frame.origin.y * scale;
    
    if(self.dragListener != nil)
    {
        NSLog(@"Draglistener not empty");
        HeliumBannerView* bannerView =(HeliumBannerView*)gr.view;
        
        self.dragListener((__bridge void*)bannerView, x, y);
    }
}

@end
