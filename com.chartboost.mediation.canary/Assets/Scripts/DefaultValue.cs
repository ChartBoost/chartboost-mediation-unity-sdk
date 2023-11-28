/// <summary>
/// A structure containing default settings values for the Canary app.
/// </summary>
struct DefaultValue
{
    /// <summary>
    /// The default values for the application identifier and signature.
    /// </summary>
#if UNITY_ANDROID
    public const string AppIdentifier = AndroidAppId;
    public const string AppSignature = "d29d75ce6213c746ba986f464e2b4a510be40399";
#else
    public const string AppIdentifier = IOSAppId;
    public const string AppSignature = "6deb8e06616569af9306393f2ce1c9f8eefb405c";
#endif
    
    public const string AndroidAppId = "5a4e797538a5f00cf60738d6";
    public const string IOSAppId = "59c04299d989d60fc5d2c782";

    /// <summary>
    /// The default backend endpoints.
    /// </summary>
    public const string Backend = "production";

    /// <summary>
    /// The default initialization behavior
    /// </summary>
    public const bool AutomaticInitialization = true;
}
