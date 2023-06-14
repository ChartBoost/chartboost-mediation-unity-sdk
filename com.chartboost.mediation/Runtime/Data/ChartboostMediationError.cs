using System;

namespace Chartboost
{
    #nullable enable
    /// <summary>
    /// Error wrapper for Chartboost Mediation errors
    /// </summary>
    [Serializable]
    public struct ChartboostMediationError
    {
        /// <summary>
        /// Constructor for custom error messages
        /// </summary>
        /// <param name="message">custom error message</param>
        public ChartboostMediationError(string message)
        {
            Message = message;
            Code = null;
        }
        
        /// <summary>
        /// Constructor for native errors
        /// </summary>
        /// <param name="code">error code</param>
        /// <param name="message">error message</param>
        public ChartboostMediationError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string? Code { get; }
        public string Message { get; }
    }
    #nullable disable
}
