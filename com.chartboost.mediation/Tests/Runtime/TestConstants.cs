namespace Chartboost.Tests.Runtime
{
    /// <summary>
    /// Centralized constants for unit tests to improve maintainability and reduce duplication.
    /// </summary>
    public static class TestConstants
    {
        /// <summary>
        /// Standard placement names used across tests.
        /// </summary>
        public static class Placements
        {
            public const string Standard = "test_placement";
            public const string Secondary = "test_placement_2";
            public const string Empty = "";
            public const string Load = "load_placement";
            public const string New = "new_placement";
            public const string Singleton = "test_placement_singleton";

            // Real placements used for load tests (Android/iOS/Editor)
            public const string Banner = "CBBanner";
            public const string Interstitial = "CBInterstitial";
        }

        /// <summary>
        /// Version strings for SDK, adapters, and partners.
        /// </summary>
        public static class Versions
        {
            public const string Adapter = "1.0.0";
            public const string Partner = "2.0.0";
            public const string PartnerSDK = "1.2.3";
            public const string PartnerAdapter = "4.5.6";
            public const string NativeSDK = "1.2.3";
            public const string Module = "1.0.0";
            public const string ExpectedSDK = "5.3.0";
        }

        /// <summary>
        /// Standard banner dimensions and sizes.
        /// </summary>
        public static class Dimensions
        {
            // Standard Banner (320x50)
            public const float StandardWidth = 320f;
            public const float StandardHeight = 50f;

            // Medium Rectangle (300x250)
            public const float MediumWidth = 300f;
            public const float MediumHeight = 250f;

            // Leaderboard (728x90)
            public const float LeaderboardWidth = 728f;
            public const float LeaderboardHeight = 90f;

            // Positions
            public const float XPosition = 100f;
            public const float YPosition = 200f;
            public const float XPositionPrecise = 100.5f;
            public const float YPositionPrecise = 200.3f;

            // Special values
            public const int WrapContent = -1;
            public const float Zero = 0f;

            // Test vectors for DensityConverter tests
            // Note: Intentionally same values as TestNativeX1/Y1 for consistency
            public const float VectorX = 100f;
            public const float VectorY = 200f;

            // Test coordinate patterns for LayoutParams
            public const float TestX = 10f;
            public const float TestY = 20f;
            public const float TestTopLeftX = 5f;
            public const float TestTopLeftY = 15f;
            public const float TestBottomLeftX = 5f;
            public const float TestBottomLeftY = 65f;
            public const float TestTopRightX = 325f;
            public const float TestTopRightY = 15f;
            public const float TestBottomRightX = 325f;
            public const float TestBottomRightY = 65f;

            // Native coordinate test values for drag testing
            // Note: TestNativeX1/Y1 intentionally match VectorX/Y for consistency
            public const float TestNativeX1 = 100f;
            public const float TestNativeY1 = 200f;
            public const float TestNativeX2 = 150f;
            public const float TestNativeY2 = 250f;
            public const float TestNativeX3 = 180f;
            public const float TestNativeY3 = 220f;
            public const float TestNativeX4 = 120f;
            public const float TestNativeY4 = 80f;
            public const float TestNativeX5 = 50f;
            public const float TestNativeY5 = 50f;
            public const float TestNativeX6 = 200f;
            public const float TestNativeY6 = 300f;

            // Container test sizes for sync validation
            public const float TestContainerWidth = 100f;
            public const float TestContainerHeight = 50f;

            // Test positions for sync validation
            public const float TestPositionX1 = 50f;
            public const float TestPositionY1 = 75f;
            public const float TestPositionX2 = 999f;
            public const float TestPositionY2 = 888f;

            // Relative position test values
            public const float RelativePositionX = 777f;
            public const float RelativePositionY = 666f;

            // Test dimensions for BannerExtensions testing
            public const float TestRectWidth = 400f;
            public const float TestRectHeight = 300f;

            // Test coordinates for extensions
            public const float TestCoordX = 50f;
            public const float TestCoordY = 75f;

            // Screen center calculation divisor
            public const float ScreenCenterDivisor = 2f;
        }

        /// <summary>
        /// Partner identifiers and names.
        /// </summary>
        public static class Partners
        {
            public const string Standard = "test_partner";
            public const string Partner1 = "partner1";
            public const string Partner2 = "partner2";
            public const string Id = "partner_123";
            public const string IdAlt = "partner_456";
            public const string Id1 = "partner_1";
            public const string Id2 = "partner_2";
            public const string Id3 = "partner_3";
            public const string Generic = "partner";
        }

        /// <summary>
        /// Unique identifiers used in tests.
        /// </summary>
        public static class UniqueIds
        {
            // General-purpose test IDs - Using large numbers to avoid collision with native SDK callbacks
            public const long Primary = 0x7FFFFFF000000001L;  // 9223372034707292161
            public const long Secondary = 0x7FFFFFF000000002L;  // 9223372034707292162
            public const long Tertiary = 0x7FFFFFF000000003L;  // 9223372034707292163
            public const long Cache = 0x7FFFFFF000000004L;  // 9223372034707292164

            // Constructor testing IDs
            public const long ConstructorIntPtr = 0x7FFFFFF000000010L;  // 9223372034707292176
            public const long ConstructorLong = 0x7FFFFFF000000011L;  // 9223372034707292177
            public const long Dispose = 0x7FFFFFF000000012L;  // 9223372034707292178

            // Specific test IDs
            public const long Id77777 = 0x7FFFFFF000000020L;  // 9223372034707292192
            public const long Id33333 = 0x7FFFFFF000000022L;  // 9223372034707292194
            public const long Id22222 = 0x7FFFFFF000000023L;  // 9223372034707292195
            public const long Id11111 = 0x7FFFFFF000000024L;  // 9223372034707292196
            public const long Id10101 = 0x7FFFFFF000000025L;  // 9223372034707292197
            public const long Id20202 = 0x7FFFFFF000000026L;  // 9223372034707292198
            public const long Id30303 = 0x7FFFFFF000000027L;  // 9223372034707292199
            public const long Id9999 = 0x7FFFFFF000000028L;  // 9223372034707292200
            public const long Id23456 = 0x7FFFFFF000000029L;  // 9223372034707292201
            public const long Id34567 = 0x7FFFFFF00000002AL;  // 9223372034707292202
            public const long Id45678 = 0x7FFFFFF00000002BL;  // 9223372034707292203
            public const long Id56789 = 0x7FFFFFF00000002CL;  // 9223372034707292204
            public const long Id78901 = 0x7FFFFFF00000002DL;  // 9223372034707292205
            public const long Id89012 = 0x7FFFFFF00000002EL;  // 9223372034707292206
            public const long Id90123 = 0x7FFFFFF00000002FL;  // 9223372034707292207
            public const long Id50001 = 0x7FFFFFF000000030L;  // 9223372034707292208
            public const long Id50002 = 0x7FFFFFF000000031L;  // 9223372034707292209
            public const long Id50003 = 0x7FFFFFF000000032L;  // 9223372034707292210
            public const long Id50004 = 0x7FFFFFF000000033L;  // 9223372034707292211
            public const long Id50005 = 0x7FFFFFF000000034L;  // 9223372034707292212
            public const long Id50006 = 0x7FFFFFF000000035L;  // 9223372034707292213
            public const long Id50007 = 0x7FFFFFF000000036L;  // 9223372034707292214
            public const long Id50008 = 0x7FFFFFF000000037L;  // 9223372034707292215
            public const long Id50009 = 0x7FFFFFF000000038L;  // 9223372034707292216
            public const long Id50010 = 0x7FFFFFF000000039L;  // 9223372034707292217
            public const long Id50011 = 0x7FFFFFF00000003AL;  // 9223372034707292218
        }

        /// <summary>
        /// Error codes and messages.
        /// </summary>
        public static class Errors
        {
            public const string Code = "ERROR_CODE_123";
            public const string CodeShow = "SHOW_ERROR";
            public const string CodeBanner = "BANNER_ERROR";
            public const string CodeGeneric = "ERROR_CODE";
            public const string CodeChartboost = "CM_100";
            public const string Message = "Test error message";
        }

        /// <summary>
        /// Auction and bidding related constants.
        /// </summary>
        public static class Auction
        {
            public const string AuctionId = "auction_123";
            public const string AuctionIdGeneric = "auction";
            public const double Price = 1.23;
            public const double PriceAlt = 1.5;
            public const double PriceAlt2 = 2.5;
            public const double PriceNegative = -1.5;
            public const double PriceLarge = 999999.99;
            public const string LineItemId = "line_789";
            public const string LineItemIdGeneric = "line_item";
            public const string LineItemName = "Line Item";
            public const string LineItemNameTest = "Test Line Item";
        }

        /// <summary>
        /// Load ID constants.
        /// </summary>
        public static class LoadIds
        {
            public const string Primary = "load_id_123";
            public const string Secondary = "load_id_456";
        }

        /// <summary>
        /// Keywords and settings for ad requests.
        /// </summary>
        public static class Keywords
        {
            public const string Key = "test_key";
            public const string Value = "test_value";
            public const string KeyAlt = "keyword_key";
            public const string ValueAlt = "keyword_value";
            public const string SettingKey = "setting_key";
            public const string SettingValue = "setting_value";

            // Generic test keys/values for JSON and dictionary tests
            public const string Key1 = "key1";
            public const string Key2 = "key2";
            public const string Value1 = "value1";
            public const string Value2 = "value2";
        }

        /// <summary>
        /// Test data values used across tests.
        /// </summary>
        public static class TestData
        {
            public const string CustomData = "test_custom_data_12345";
        }

        /// <summary>
        /// Tolerance values for floating point comparisons.
        /// </summary>
        public static class Tolerance
        {
            public const float Standard = 0.001f;
            public const float Assertion = 0.01f;
            public const float Within = 0.009f;
            public const float Outside = 0.011f;
        }

        /// <summary>
        /// Reflection member names (methods, properties, fields) used in tests.
        /// </summary>
        public static class Reflection
        {
            public const string BannerAdField = "_bannerAd";

            // Properties
            public const string BannerAdProperty = "BannerAd";

            // Methods
            public const string OnWillAppearMethod = "OnWillAppear";
            public const string OnClickMethod = "OnClick";
            public const string OnDragMethod = "OnDrag";
            public const string OnDragBeginMethod = "OnDragBegin";
            public const string OnDragEndMethod = "OnDragEnd";
            public const string OnRecordImpressionMethod = "OnRecordImpression";
            public const string OnDetachFromPanelMethod = "OnDetachFromPanel";

            // Error messages for reflection failures
            public const string BannerAdFieldNotFound = "_bannerAd field not found via reflection";
            public const string BannerAdPropertyNotFound = "BannerAd property not found via reflection";
            public const string OnWillAppearMethodNotFound = "OnWillAppear method not found";
            public const string OnClickMethodNotFound = "OnClick method not found";
            public const string OnRecordImpressionMethodNotFound = "OnRecordImpression method not found";
            public const string OnDragBeginMethodNotFound = "OnDragBegin method not found";
            public const string OnDragMethodNotFound = "OnDrag method not found";
            public const string OnDragEndMethodNotFound = "OnDragEnd method not found";
            public const string OnDetachFromPanelMethodNotFound = "OnDetachFromPanel method not found";
        }

        /// <summary>
        /// Canvas and UI-related constants.
        /// </summary>
        public static class Canvas
        {
            // Names
            public const string DefaultName = "Canvas";
            public const string TestName = "TestCanvas";
            public const string RootName = "RootCanvas";
            public const string ChildName = "ChildCanvas";
            public const string NestedName = "NestedCanvas";
            public const string ParentName = "ParentCanvas";
            public const string ParentObjectName = "Parent";

            // Suffixes
            public const string Suffix1 = "1";
            public const string Suffix2 = "2";
            public const string Suffix3 = "3";

            // Sorting Orders
            public const int SortingOrderLow = 0;
            public const int SortingOrderMedium = 10;
            public const int SortingOrderHigh = 100;
            public const int SortingOrderNegativeLow = -100;
            public const int SortingOrderNegativeHigh = -10;
        }

        /// <summary>
        /// Color values for testing.
        /// </summary>
        public static class Colors
        {
            public const float Red = 1.0f;
            public const float Green = 0.5f;
            public const float Blue = 0.25f;
            public const float Alpha = 1.0f;
        }

        /// <summary>
        /// Pivot values for UI positioning.
        /// </summary>
        public static class Pivots
        {
            public const float Center = 0.5f;
            public const float Min = 0f;
            public const float Max = 1f;
        }

        /// <summary>
        /// JSON serialization constants.
        /// </summary>
        public static class Json
        {
            public const string OpenBrace = "{";
            public const string CloseBrace = "}";
            public const string Null = "null";
            public const string EmptyObject = "{}";
            public const string EmptyArray = "[]";

            // Banner property names
            public const string KeywordsProperty = "Keywords";
            public const string PartnerSettingsProperty = "PartnerSettings";
            public const string DraggableProperty = "Draggable";

            // Boolean values
            public const string TrueValue = "true";
            public const string FalseValue = "false";
        }

        /// <summary>
        /// Queue and capacity-related constants.
        /// </summary>
        public static class Queue
        {
            public const int Capacity = 5;
            public const int NumberOfAdsReady = 3;
        }

        /// <summary>
        /// Count related constants.
        /// </summary>
        public static class Counts
        {
            public const int Zero = 0;
            public const int One = 1;
            public const int Two = 2;
            public const int Three = 3;
            public const string ZeroString = "0";
            public const string TwoString = "2";
            public const int ExpectedDragEventCount = 3;
        }

        /// <summary>
        /// Log patterns for assertion in tests using LogAssert.Expect.
        /// All patterns are regex strings for use with System.Text.RegularExpressions.Regex.
        /// </summary>
        public static class LogPatterns
        {
            // Core lifecycle patterns
            public const string Creating = ".*Creating.*";
            public const string Load = ".*Load.*";
            public const string Reset = ".*Reset.*";
            public const string Dispose = ".*Dispose.*";
            public const string Start = ".*Starting.*";
            public const string Stop = ".*Stopping.*";

            // Ad event patterns
            public const string Click = ".*Click.*";
            public const string Close = ".*Close.*";
            public const string RecordImpression = ".*RecordImpression.*";
            public const string Expire = ".*Expire.*";
            public const string Reward = ".*Reward.*";
            public const string WillAppear = ".*WillAppear.*";

            // Drag event patterns
            public const string DragBegin = ".*Drag Begin.*";
            public const string Drag = ".*Drag to.*";
            public const string DragEnd = ".*Drag End.*";
            public const string DragWithoutBegin = ".*The DidDrag event was triggered, but no preceding DidDragBegin event was detected.*";

            // Background/styling patterns
            public const string ContainerBackground = ".*Setting native view's container background color.*";
            public const string AdBackground = ".*Setting native view's ad background color.*";

            // Queue patterns
            public const string GetNextAd = ".*attempting to get next ad.*";
            public const string HasNextAd = ".*checking for next ad.*";

            // Error/Warning patterns
            public const string FailedWeakReference = ".*Failed to get (WeakReference<IAd>|AdLoadRequest) for.*";
            // ReSharper disable once InconsistentNaming
            public const string ILRD = ".*ILRD.*";
            public const string UnexpectedCharacter = ".*Unexpected character.*";
            public const string CannotBeNullOrEmpty = ".*cannot be null or empty.*";
            public const string NotInitialized = ".*Unable to fetch Ads, Chartboost Mediation has not been initialized.*";
            public const string NullPlacement = ".*Unable to fetch Ads, placement cannot be null or empty.*";
            public const string InitializationFailed = ".*Chartboost Mediation Failed to Initialize.*";

            // Event type patterns
            public const string FullscreenEvent = ".*Fullscreen Ad event.*";
            public const string BannerEvent = ".*Banner Ad event.*";
            public const string QueueEvent = ".*Fullscreen Ad Queue event.*";

            // Other patterns
            public const string Tracking = ".*Tracking.*";
            public const string TrackingAdLoadRequest = ".*Tracking AdLoadRequest.*";
            public const string PreInitConfig = ".*SetPreInitializationConfiguration with Configuration.*";
        }

        /// <summary>
        /// AdCache utility testing constants.
        /// </summary>
        public static class AdCache
        {
            public const string AdsFieldName = "Ads";
            public const string AdLoadRequestsFieldName = "AdLoadRequests";
            public const string ExpectedCacheInfoPrefix = "CacheManager : \n";
            public const string ExpectedFullscreenCacheText = "Fullscreen Cache: ";
            public const string ExpectedAdLoadRequestText = "FullscreenAdLoadRequest: ";
        }

        /// <summary>
        /// Density converter testing constants.
        /// </summary>
        public static class DensityConverters
        {
            public const float TestNativeValue = 100f;
            public const float TestPixelsValue = 250f;
            public const float TestUIDocValue = 50f;
            public const float DefaultScaleFactor = 2.5f;
            public const float CustomScaleFactor = 3.0f;
            public const float TestUIDocScaleFactor = 2.0f;
            public const string UIDocScaleFactorFieldName = "_uiDocScaleFactor";
        }

        /// <summary>
        /// Metrics testing constants.
        /// </summary>
        public static class Metrics
        {
            public const string TestResult = "success";
            public const string TestNetworkType = "bidding";
            public const string TestPartnerPlacement = "partner_placement_001";
            public const long TestStart = 1234567890L;
            public const long TestEnd = 1234567900L;
            public const long TestDuration = 10L;
            public const string TestErrorType = "network_error";
            public const string TestErrorDescription = "Connection failed";
            public const string TestErrorData = "Additional error data";
        }

        /// <summary>
        /// ChartboostMediation testing constants.
        /// </summary>
        public static class ChartboostMediation
        {
            public const string InstancePropertyName = "Instance";
            public const string ExpectedCoreModuleId = "chartboost_mediation";
            public const string ExpectedDefaultInstanceName = "ChartboostMediationDefault";
            // ReSharper disable UnusedMember.Global
            public const string ExpectedPlatformInstanceName = "ChartboostMediation";
            public const string ExpectedAndroidNamespace = "Chartboost.Mediation.Android";
            public const string ExpectedIOSNamespace = "Chartboost.Mediation.iOS";
            // ReSharper restore UnusedMember.Global
            public const string TestParentGameObjectName = "TestParent";
            public const string InvalidJsonString = "invalid json";
            public const string ValidJsonWithPlacement = "{\"placement\":\"test_placement\",\"revenue\":1.5}";
            public const string ValidJsonWithoutPlacement = "{\"revenue\":2.5}";
            public const string PlacementKey = "placement";
            public const string TestPartnerData = "partner_initialization_data";
        }

        /// <summary>
        /// BannerVisualElement testing constants.
        /// </summary>
        public static class BannerVisualElement
        {
            public const string JsonKey = "json_key";
            public const string JsonValue = "json_value";
            public const string PartnerKey = "partner";
            public const string PartnerValue = "setting";
            public const string PartnerKeyAlt = "partner_key";
            public const string PartnerValueAlt = "partner_value";
            public const string AdElementName = "Ad";
            public const string ReflectionAdRelativePositionProperty = "AdRelativePosition";
            public const string ReflectionSyncWithNativeContainerMethod = "SyncWithNativeContainer";
            public const string ReflectionIsDraggingField = "_isDragging";
            public const string ExpectedPassKeywordsEmptyDictionary = "Keywords returned an empty dictionary on native platform (platform-specific behavior)";
            public const string ExpectedPassPartnerSettingsAndroidException = "PartnerSettings access threw AndroidJavaException on Android (platform-specific limitation)";
            public const string ExpectedPassPartnerSettingsEmptyDictionary = "PartnerSettings returned an empty dictionary on native platform (platform-specific behavior)";
        }

        /// <summary>
        /// Unity UI and GameObject related constants.
        /// </summary>
        public static class Unity
        {
            // GameObject names
            public const string EventSystemName = "EventSystem";
            public const string SharedTestCanvasName = "SharedTestCanvas";
            public const string TestUIDocumentName = "TestUIDocument";
            public const string TestUnityBannerAdName = "TestUnityBannerAd";

            // Canvas configuration
            public const float ReferenceResolutionWidth = 1920f;
            public const float ReferenceResolutionHeight = 1080f;
            public const float CanvasMatchWidthOrHeight = 0.5f;
        }

        /// <summary>
        /// UI Toolkit specific test constants.
        /// </summary>
        public static class UIToolkit
        {
            // UIDocument names
            public const string SharedTestUIDocumentName = "SharedTestUIDocument";

            // PanelSettings configuration
            public const float ReferenceDpi = 96f;
            public const float FallbackDpi = 96f;

            // Banner dimensions
            public const float BannerWidth = 320f;
            public const float BannerHeight = 50f;

            // Positioning
            public const float InitialPositionLeft = 0f;
            public const float InitialPositionTop = 0f;
            public const float PositionZero = 0f;
        }

        /// <summary>
        /// Timing constants for async operations and waits.
        /// </summary>
        public static class Timing
        {
            public const float WaitForBannerDisplay = 1.5f;
            public const float StandardWaitTime = 1.0f;
        }

        /// <summary>
        /// Common assertion messages used across tests.
        /// </summary>
        public static class AssertionMessages
        {
            // Common assertions
            public const string SharedBannerAdMustExist = "Shared banner ad must be created first";
            public const string SharedBannerAdCreated = "Shared banner ad should be created in OneTimeSetUp";
            public const string SharedBannerAdGameObjectExists = "Shared banner ad GameObject should exist";
            public const string SharedBannerVisualElementMustExist = "Shared banner visual element must be created first";
            public const string SharedBannerVisualElementCreated = "Shared banner visual element should be created in OneTimeSetUp";
            public const string SharedUIDocumentExists = "Shared UIDocument should exist";

            // Load-related assertions
            public const string LoadResultShouldNotBeNull = "Load result should not be null";
            public const string LoadShouldSucceed = "Load should succeed without errors";
            public const string WillAppearShouldTrigger = "WillAppear event should be triggered after successful load";
            public const string WillAppearPassCorrectInstance = "WillAppear should pass the correct banner instance";
            public const string ImpressionShouldTrigger = "DidRecordImpression event should be triggered after banner is displayed";
            public const string ImpressionPassCorrectInstance = "DidRecordImpression should pass the correct banner instance";

            // Post-load property assertions
            public const string RequestAvailableAfterLoad = "Request should be available after load";
            public const string LoadIdAvailableAfterLoad = "LoadId should be available after load";
            public const string LoadMetricsAvailableAfterLoad = "LoadMetrics should be available after load";
            public const string BannerSizeAvailableAfterLoad = "BannerSize should be available after load";
            public const string WinningBidInfoAvailableAfterLoad = "WinningBidInfo should be available after load";

            // Property-specific assertions
            public const string KeywordsShouldNotBeNull = "Keywords should not be null after being set";
            public const string KeywordsShouldContainTestKey = "Keywords should contain the test key";
            public const string PartnerSettingsShouldNotBeNull = "PartnerSettings should not be null after being set";
            public const string PartnerSettingsShouldContainTestKey = "PartnerSettings should contain the test key";

            // RectTransform and positioning assertions
            public const string RectTransformShouldNotBeNull = "RectTransform should not be null after relocation";
            public const string RectTransformComponentRequired = "Banner ad GameObject should have a RectTransform component for UI layout";
            public const string AnchorMinXMatches = "RectTransform anchorMin.x should match expected value";
            public const string AnchorMinYMatches = "RectTransform anchorMin.y should match expected value";
            public const string AnchorMaxXMatches = "RectTransform anchorMax.x should match expected value";
            public const string AnchorMaxYMatches = "RectTransform anchorMax.y should match expected value";

            // VisualElement positioning assertions
            public const string PositionShouldBeAbsolute = "VisualElement position should be absolute";
            public const string LeftPositionMatches = "VisualElement left position should match expected value";
            public const string TopPositionMatches = "VisualElement top position should match expected value";

            // IBannerAd positioning assertions
            public const string PositionXMatches = "Banner position X should match expected value";
            public const string PositionYMatches = "Banner position Y should match expected value";

            // IBannerAd container size assertions
            public const string ContainerSizeShouldMatch = "Container size should match expected value";

            // JSON assertions
            public const string ToStringShouldReturnNonNull = "ToString should return a non-null string";
            public const string JsonShouldContainOpenBrace = "JSON string should contain opening brace";
            public const string JsonShouldContainCloseBrace = "JSON string should contain closing brace";

            // Reset assertions
            public const string ResetShouldNotThrow = "Reset should not throw any exceptions";

            // Platform-specific
            public const string SkippingNonNativePlatform = "Skipping native platform tests - not running on Android or iOS";

            // Test mode assertions
            public const string TestModeShouldBeEnabled = "TestMode should be enabled for native tests";

            // Fullscreen ad shared instance assertions
            public const string SharedFullscreenAdMustExist = "Shared fullscreen ad must exist";
            public const string SharedFullscreenAdShouldExist = "Shared fullscreen ad should exist after load";
            public const string LoadedFullscreenAdShouldNotBeNull = "Loaded fullscreen ad should not be null";
            public const string FullscreenAdLoadShouldSucceed = "Fullscreen ad load should succeed without errors";
            public const string RewardedAdLoadShouldSucceed = "Rewarded ad load should succeed without errors";
            public const string LoadedRewardedAdShouldNotBeNull = "Loaded rewarded ad should not be null";

            // Fullscreen ad property assertions
            public const string RequestContainsCorrectPlacement = "Request should contain correct placement name";
            public const string LoadIdShouldNotBeEmpty = "LoadId should be available and not empty after load";
            public const string RewardedAdLoadIdShouldNotBeEmpty = "Rewarded ad LoadId should not be empty";
            public const string RewardedAdRequestShouldNotBeNull = "Rewarded ad Request should not be null";

            // CustomData assertions
            public const string CustomDataShouldMatch = "CustomData should match the value that was set";
            public const string CustomDataShouldBeNullOrEmpty = "CustomData should be null or empty when set to null";

            // Fullscreen ad event subscription assertions
            public const string SubscribingDidClickShouldNotThrow = "Subscribing and unsubscribing DidClick event should not throw exceptions";
            public const string SubscribingDidCloseShouldNotThrow = "Subscribing and unsubscribing DidClose event should not throw exceptions";
            public const string SubscribingDidExpireShouldNotThrow = "Subscribing and unsubscribing DidExpire event should not throw exceptions";
            public const string SubscribingDidRecordImpressionShouldNotThrow = "Subscribing and unsubscribing DidRecordImpression event should not throw exceptions";
            public const string SubscribingDidRewardShouldNotThrow = "Subscribing and unsubscribing DidReward event should not throw exceptions";

            // Fullscreen ad disposal assertions
            public const string DisposeShouldNotThrow = "Dispose should not throw exceptions";

            // Test environment expected passes
            public const string BannerAdNullExpected = "BannerAd is null in test environment, which is expected";
            public const string DetachFromPanelHandledSuccessfully = "DetachFromPanel handled successfully";
            public const string DetachFromPanelHandledNull = "DetachFromPanel handled null BannerAd gracefully";
        }

        /// <summary>
        /// Debug log prefixes for test event tracking.
        /// </summary>
        public static class DebugLogPrefixes
        {
            // Banner ad event log prefixes
            public const string WillAppearEvent = "[Load Test] WillAppear event triggered";
            public const string ImpressionEvent = "[Load Test] DidRecordImpression event triggered";

            // Fullscreen ad event log prefixes
            public const string FullscreenClickEvent = "[FullscreenAdNativeTests] DidClick event triggered";
            public const string FullscreenCloseEvent = "[FullscreenAdNativeTests] DidClose event triggered";
            public const string FullscreenExpireEvent = "[FullscreenAdNativeTests] DidExpire event triggered";
            public const string FullscreenImpressionEvent = "[FullscreenAdNativeTests] DidRecordImpression event triggered";
            public const string FullscreenRewardEvent = "[FullscreenAdNativeTests] DidReward event triggered";
        }

        /// <summary>
        /// Banner alignment values for positioning tests.
        /// Note: Unity UI and UI Toolkit use different coordinate systems for vertical alignment.
        /// </summary>
        public static class BannerAlignment
        {
            /// <summary>
            /// Horizontal alignment values (consistent across Unity UI and UI Toolkit)
            /// </summary>
            public const float Left = 0f;
            public const float HorizontalCenter = 0.5f;
            public const float Right = 1f;

            /// <summary>
            /// UI Toolkit vertical alignment values (top=0, bottom=1)
            /// </summary>
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static class UIToolkit
            {
                public const float Top = 0f;
                public const float VerticalCenter = 0.5f;
                public const float Bottom = 1f;
            }

            /// <summary>
            /// Unity UI vertical alignment values (bottom=0, top=1)
            /// Unity UI uses inverted Y-axis for anchors compared to UIToolkit
            /// </summary>
            public static class UnityUI
            {
                public const float Bottom = 0f;
                public const float VerticalCenter = 0.5f;
                public const float Top = 1f;
            }
        }
    }
}
