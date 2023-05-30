using System;

namespace Chartboost.Placements
{
    #nullable enable
    [Serializable]
    public struct ChartboostMediationAdShowResult
    {
        public ChartboostMediationAdShowResult(Metrics? metrics, ChartboostMediationError? error = null)
        {
            this.metrics = metrics;
            this.error = error;
        }
        
        public ChartboostMediationAdShowResult(ChartboostMediationError error)
        {
            metrics = null;
            this.error = error;
        }
        
        public Metrics? metrics;

        public ChartboostMediationError? error;
    }
    #nullable disable
}
