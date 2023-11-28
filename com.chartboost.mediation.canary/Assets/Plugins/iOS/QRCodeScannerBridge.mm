// Copyright 2022-2023 Chartboost, Inc.
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file.

#import <Foundation/Foundation.h>
#import <UnityFramework/Canary-Swift.h>

typedef void (*DidScanQrCodeCallback)(const char*, const char*);

@interface QRCodeScannerBridge : NSObject <QRCodeScannerViewControllerDelegate>
@property (nonatomic) DidScanQrCodeCallback didScanQrCodeCallback;
+ (instancetype) sharedBridge;
@end

@implementation QRCodeScannerBridge

+ (instancetype) sharedBridge {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

- (void) presentQRCodeScanner {
    UIStoryboard *storyboard = [UIStoryboard storyboardWithName:@"QRCodeScanner" bundle:[NSBundle bundleForClass:[self class]]];
    QRCodeScannerViewController *qrCodeViewController = storyboard.instantiateInitialViewController;
    if (qrCodeViewController) {
        qrCodeViewController.delegate = self;
        qrCodeViewController.modalPresentationStyle = UIModalPresentationFullScreen;
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:qrCodeViewController animated:YES completion:nil];
    }
}

- (void) didScanQRCodeWithAppId:(NSString * _Nonnull)appId appSignature:(NSString * _Nonnull)appSignature {
    self.didScanQrCodeCallback(appId.UTF8String, appSignature.UTF8String);
    self.didScanQrCodeCallback = nil;
}

@end

extern "C"
{
    void _presentQRCodeScanner(DidScanQrCodeCallback callback) {
		[QRCodeScannerBridge sharedBridge].didScanQrCodeCallback = callback;
        [[QRCodeScannerBridge sharedBridge] presentQRCodeScanner];
    }
}
