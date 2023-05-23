//
//  ChaertboostMediationBannerAdWrapper.m
//  UnityFramework
//
//  Created by Kushagra Gupta on 5/12/23.
//

#import <Foundation/Foundation.h>

#import "ChartboostMediationManager.h"
#import "ChartboostMediationBannerAdDragger.h"

@implementation ChartboostMediationBannerAdDragger

- (void)handlePan:(UIPanGestureRecognizer *)gr
{
    CGPoint translation = [gr translationInView:gr.view.superview];
    CGPoint center = gr.view.center;
    center.x += translation.x;
    center.y += translation.y;
    gr.view.center = center;
    [gr setTranslation:CGPointZero inView:gr.view.superview];
        
    float scale = UIScreen.mainScreen.scale;
    float x = gr.view.frame.origin.x * scale;
    float y = gr.view.frame.origin.y * scale;
    
    if(self.dragListener != nil)
    {
        HeliumBannerView* bannerView =(HeliumBannerView*)gr.view;
        self.dragListener((__bridge void*)bannerView, x, y);
    }
}

@end
