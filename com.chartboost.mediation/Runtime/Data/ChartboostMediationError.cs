using System;

namespace Chartboost.Placements
{
    #nullable enable
    [Serializable]
    public struct ChartboostMediationError
    {
        public ChartboostMediationError(string message)
        {
            this.message = message;
            code = null;
        }
        
        public ChartboostMediationError(string code, string message)
        {
            this.code = code;
            this.message = message;
        }

        public string? code;
        public string message;
    }
    #nullable disable
}
