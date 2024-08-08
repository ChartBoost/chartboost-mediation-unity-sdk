using System;
using Newtonsoft.Json;

namespace Chartboost.Mediation.Error
{
    #nullable enable
    /// <summary>
    /// Error wrapper for Chartboost Mediation errors.
    /// </summary>
    [Serializable]
    public struct ChartboostMediationError
    {
        /// <summary>
        /// Constructor for custom error messages.
        /// </summary>
        /// <param name="message">custom error message.</param>
        public ChartboostMediationError(string message)
        {
            Message = message;
            Code = null;
        }
        
        /// <summary>
        /// Constructor for native errors.
        /// </summary>
        /// <param name="code">error code.</param>
        /// <param name="message">error message.</param>
        public ChartboostMediationError(string code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// The underlying <b>Code</b> for this error.
        /// </summary>
        [JsonProperty("code")]
        public string? Code { get; }
        
        /// <summary>
        /// A human-readable, description of this error.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; }
    }
}
