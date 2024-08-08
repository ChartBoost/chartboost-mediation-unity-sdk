using System.Runtime.InteropServices;
using AOT;
using Chartboost.Constants;

namespace Chartboost.Mediation.iOS
{
    internal sealed partial class ChartboostMediation
    {
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMSetLifeCycleCallbacks(ExternChartboostMediationDataEvent didReceivePartnerInitializationChartboostMediationDataCallback, ExternChartboostMediationDataEvent didReceiveImpressionLevelRevenueChartboostMediationData);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern float _CBMGetUIScaleFactor();
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationDataEvent))]
        private static void ExternDidReceiveImpressionLevelRevenueData(string impressionDataJson) 
            => Chartboost.Mediation.ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(impressionDataJson);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationDataJson)
            => Chartboost.Mediation.ChartboostMediation.OnDidReceivePartnerAdapterInitializationData(partnerInitializationDataJson);
    }
}
