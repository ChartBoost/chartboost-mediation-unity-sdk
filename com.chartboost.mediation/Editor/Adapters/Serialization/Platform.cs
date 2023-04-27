using System;

namespace Chartboost.Editor.Adapters.Serialization
{
    /// <summary>
    /// Supported platforms for Adapters. Flag enabled.
    /// </summary>
    [Flags]
    public enum Platform
    {
        None = 0,
        Android = 1,
        IOS = 2
    }
}
