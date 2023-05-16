//
//  ChaertboostMediationBannerAdWrapper.h
//  Unity-iPhone
//
//  Created by Kushagra Gupta on 5/12/23.
//

#ifndef ChartboostMediationBannerAdWrapper_h
#define ChartboostMediationBannerAdWrapper_h

#import "ChartboostMediationManager.h"

@interface ChartboostMediationBannerAdDragger : NSObject

@property ChartboostMediationBannerDragEvent dragListener;


//- (void) load : (long) screenLocation;
//- (void) load : (float) x y:(float) y width: (float) width height:(float) height;
- (void) handlePan:(UIPanGestureRecognizer*) gr;


@end


#endif /* ChaertboostMediationBannerAdWrapper_h */
