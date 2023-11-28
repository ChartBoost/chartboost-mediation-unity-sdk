#import <objc/runtime.h>
#import <DTBiOSSDK/DTBAds.h>
#import <FBAudienceNetwork/FBAdSettings.h>
//#import <HeliumAdapterAppLovin/HeliumAdapterAppLovin.h>

#define GetStringParam(_x_) (_x_ != NULL) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

@interface CanaryUnityBridge : NSObject
+ (instancetype) sharedBridge;
@end

@implementation CanaryUnityBridge

+ (instancetype) sharedBridge {
    static dispatch_once_t pred = 0;
    static id _sharedObject = nil;
    dispatch_once(&pred, ^{
        _sharedObject = [[self alloc] init];
    });
    return _sharedObject;
}

struct Implementation {
    SEL selector;
    IMP imp;
};

-(Implementation)getImplementationFromClassNamed:(NSString*)className selectorName:(NSString*)selectorName
{
    Class cls = NSClassFromString(className);
    SEL selector = NSSelectorFromString(selectorName);
    Method method = class_getClassMethod(cls, selector);
    IMP imp = method_getImplementation(method);
    struct Implementation implementation;
    implementation.selector = selector;
    implementation.imp = imp;
    return implementation;
}

-(void)setSdkDomainName:(NSString*)sdkDomainName
{
    Implementation implementation = [self getImplementationFromClassNamed:@"CHBHTestModeHelper" selectorName:@"setSdkAPIHostOverride:"];
    typedef void (*Signature)(id, SEL, NSString*);
    Signature function = (Signature)implementation.imp;
    function(self, implementation.selector, sdkDomainName);
}

-(void)setRtbDomainName:(NSString*)rtbDomainName
{
    Implementation implementation = [self getImplementationFromClassNamed:@"CHBHTestModeHelper" selectorName:@"setRtbAPIHostOverride:"];
    typedef void (*Signature)(id, SEL, NSString*);
    Signature function = (Signature)implementation.imp;
    function(self, implementation.selector, rtbDomainName);
}

-(void)setAmazonPublisherServicesTestMode:(BOOL)value
{
    [DTBAds sharedInstance].testMode = value;
}

-(void)setMetaAudienceNetworkTestMode:(BOOL)value
{
    if (value)
        [FBAdSettings addTestDevice:[FBAdSettings testDeviceHash]];
    else
        [FBAdSettings clearTestDevices];
}

-(void)setAppLovinTestMode:(BOOL)value
{
    //CHBHAppLovinClient.testMode = value
}
@end

extern "C" {

void _setSdkDomainName(const char* sdkDomainName)
{
    [[CanaryUnityBridge sharedBridge] setSdkDomainName:GetStringParam(sdkDomainName)];
}

void _setRtbDomainName(const char* rtbDomainName)
{
    [[CanaryUnityBridge sharedBridge] setRtbDomainName:GetStringParam(rtbDomainName)];
}

void _setAmazonPublisherServicesTestMode(BOOL value)
{
    [[CanaryUnityBridge sharedBridge] setAmazonPublisherServicesTestMode:value];
}

void _setMetaAudienceNetworkTestMode(BOOL value)
{
    [[CanaryUnityBridge sharedBridge] setMetaAudienceNetworkTestMode:value];
}

void _setAppLovinTestMode(BOOL value)
{
    [[CanaryUnityBridge sharedBridge] setAppLovinTestMode:value];
}
}
