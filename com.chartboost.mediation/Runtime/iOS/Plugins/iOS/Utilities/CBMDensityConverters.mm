#pragma mark Extern Methods
extern "C" {
    float _CBMGetUIScaleFactor() {
        // `UIScreen.main` was deprecated in iOS 16. Apple doc:
        //   https://developer.apple.com/documentation/uikit/uiscreen/1617815-main
        // Since `UIScreen.main` has been working correctly at least up to iOS 16, the custom
        // implementation only targets iOS 17+, not iOS 13+.
        if (@available(iOS 17.0, *)) {
            NSSet<UIScene*> *connectedScenes = UIApplication.sharedApplication.connectedScenes;
            NSArray *activationStates = @[@(UISceneActivationStateForegroundActive), @(UISceneActivationStateForegroundInactive), @(UISceneActivationStateBackground), @(UISceneActivationStateUnattached)];
            for (NSNumber *activationState in activationStates) {
                UISceneActivationState state = (UISceneActivationState)activationState.integerValue;
                for (UIScene* connectedScene in connectedScenes) {
                    UIWindowScene *windowScene = (UIWindowScene*)connectedScene;
                    if (windowScene) {
                        if (windowScene.activationState == state) {
                            return windowScene.screen.scale;
                        }
                    }
                }
            }
        }

        // fallback
        return UIScreen.mainScreen.scale;
    }
}
