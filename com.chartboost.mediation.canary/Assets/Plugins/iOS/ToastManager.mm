/*
 * ToastManager.mm
 * Toast Manager for Canary
 */

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

@interface ToastManager : NSObject
@end

@implementation ToastManager

static ToastManager *_sharedInstance;

+(ToastManager*) sharedManager
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[ToastManager alloc] self];
    });
    
    return _sharedInstance;
}

- (void) showAlert:(NSString*)message
{
    UIAlertController* alert = [UIAlertController alertControllerWithTitle:@"Chartboost Mediation Unity SDK" message:message preferredStyle:UIAlertControllerStyleAlert];
    
    id rootViewController = [UIApplication sharedApplication].delegate.window.rootViewController;
    if([rootViewController isKindOfClass:[UINavigationController class]])
    {
        rootViewController = ((UINavigationController *)rootViewController).viewControllers.firstObject;
    }
    if([rootViewController isKindOfClass:[UITabBarController class]])
    {
        rootViewController = ((UITabBarController *)rootViewController).selectedViewController;
    }
    [rootViewController presentViewController:alert animated:YES completion:nil];
    
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(2 * NSEC_PER_SEC)),     dispatch_get_main_queue(), ^{
        [alert dismissViewControllerAnimated:YES completion:nil];
    });
}

@end

extern "C"
{
    void _showMessage(const char* message) {
        NSString *alertMessage = [NSString stringWithUTF8String:message];
        [[ToastManager sharedManager] showAlert :alertMessage];
    }
}
