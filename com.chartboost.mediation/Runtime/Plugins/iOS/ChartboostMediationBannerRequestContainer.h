//
//  ChartboostMediationBannerRequestContainer.h
//  Unity-iPhone
//
//  Created by Kushagra Gupta on 9/12/23.
//

#ifndef ChartboostMediationBannerRequestContainer_h
#define ChartboostMediationBannerRequestContainer_h

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <ChartboostMediationSDK/ChartboostMediationSDK-Swift.h>


@interface ChartboostMediationBannerRequestContainer : NSObject

@property float x;
@property float y;
@property float width;
@property float height;
@property BOOL usesConstraints;
@property ChartboostMediationBannerView* bannerView;

- (instancetype) initWithXY: (float)x y:(float)y andbannerView:(ChartboostMediationBannerView*) bannerView;
- (instancetype) initWithBannerView:(ChartboostMediationBannerView*) bannerView;
- (void) resize:(CGSize)newSize axis: (int) axis pivotX:(float) pivotX pivotY:(float) pivotY;


@end

#endif /* ChartboostMediationBannerRequestContainer_h */
