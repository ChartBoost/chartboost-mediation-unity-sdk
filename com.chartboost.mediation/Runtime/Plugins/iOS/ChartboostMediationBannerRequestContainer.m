//
//  ChartboostMediationBannerRequestContainer.m
//  UnityFramework
//
//  Created by Kushagra Gupta on 9/12/23.
//

#import <Foundation/Foundation.h>
#import "ChartboostMediationBannerRequestContainer.h"

@implementation ChartboostMediationBannerRequestContainer

- (instancetype) init:(float)width height:(float)height andBannerView:(ChartboostMediationBannerView*) bannerView {
    self.usesConstraints = true;
    self.width = width;
    self.height = height;
    self.bannerView = bannerView;
    
    NSLog(@"BannerRequestContainer created with width : %f, height:%f, usesConstraints:%d",_width, _height, _usesConstraints);
    return self;
}

- (instancetype) initWithXY: (float)x y:(float)y width:(float)width height:(float)height andBannerView:(ChartboostMediationBannerView*) bannerView{
    self.usesConstraints = false;
    self.x = x;
    self.y = y;
    self.bannerView = bannerView;
    self.width = width;
    self.height = height;
    NSLog(@"BannerRequestContainer created with x:%f, y:%f, width : %f, height:%f, usesConstraints:%d", _x,_y,_width, _height, _usesConstraints);
    return self;
}

- (void)resize:(CGSize)newSize axis:(int)axis pivotX:(float)pivotX pivotY:(float)pivotY {
        
    CGRect frame = self.bannerView.frame;
    
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
        // Find pivot point on screen
        CGPoint containerPivot = CGPointMake(self.x + self.width*pivotX, self.y + self.height * pivotY);
        NSLog(@"Container pivot : (%f, %f)", containerPivot.x, containerPivot.y);
        
        // Find top-left corner of newSize w.r.t pivot
        float left = pivotX * newSize.width;
        float top = pivotY * newSize.height;
        NSLog(@"Left : %f, Top : %f newSize : (%f, %f)", left, top, newSize.width, newSize.height);
        
        // Resize and move container to top-left of new size
        CGPoint topLeft = CGPointMake(containerPivot.x - left, containerPivot.y - top);
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
    
    self.bannerView.frame = frame;
}

@end
