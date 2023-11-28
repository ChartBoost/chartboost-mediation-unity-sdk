#import <StoreKit/StoreKit.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>

typedef void (*ATTResponse)(long code);

extern "C"
{
  bool _ATTSupported() {

    if (@available(iOS 14.0, *))
      return true;
    else
      return false;
  }

  long _getATTStatus() {
    if (@available(iOS 14.0, *)) {
      return [ATTrackingManager trackingAuthorizationStatus];
    }

    return 3; // 3 == ATTrackingManagerAuthorizationStatusAuthorized
  }

  void _requestATT(ATTResponse callback) {
    if (@available(iOS 14.0, *)) {
      [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
        if (callback != nil)
            callback(status);
      }];
    }
    else {
        NSLog(@"ATT Not Supported.");
    }
  }
}
