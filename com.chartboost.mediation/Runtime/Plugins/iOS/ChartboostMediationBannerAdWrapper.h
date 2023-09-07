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


@interface ChartboostMediationBannerAdWrapper : NSObject

@property ChartboostMediationBannerView* bannerView;
@property ChartboostMediationBannerAdDragEvent dragListener;
@property UIPanGestureRecognizer *panGesture;
@property BOOL canDrag;

- (instancetype)initWithBannerView: (ChartboostMediationBannerView*) bannerView andDragListener:(ChartboostMediationBannerAdDragEvent) dragListener;
- (void)setDraggable:(BOOL)canDrag;

@end
#endif /* ChartboostMediationBannerAdWrapper_h */
