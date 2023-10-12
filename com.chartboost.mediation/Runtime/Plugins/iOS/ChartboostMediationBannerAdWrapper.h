//
//  ChartboostMediationBannerAdWrapper.h
//  Unity-iPhone
//
//  Created by Kushagra Gupta on 6/21/23.
//

#ifndef ChartboostMediationBannerAdWrapper_h
#define ChartboostMediationBannerAdWrapper_h



#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>


typedef void (*ChartboostMediationBannerAdDragEvent)(void* uniqueId, float x, float y);

@interface ChartboostMediationBannerAdWrapper : NSObject<UIGestureRecognizerDelegate>

@property ChartboostMediationBannerView* bannerView;
@property ChartboostMediationBannerAdDragEvent dragListener;
@property UIPanGestureRecognizer *panGesture;
@property BOOL canDrag;
@property BOOL usesConstraints;

- (instancetype)initWithBannerView: (ChartboostMediationBannerView*) bannerView andDragListener:(ChartboostMediationBannerAdDragEvent) dragListener;
- (void)setDraggable:(BOOL)canDrag;
- (void) resize:(int)axis pivotX:(float) pivotX pivotY:(float)pivotY;

@end
#endif /* ChartboostMediationBannerAdWrapper_h */
