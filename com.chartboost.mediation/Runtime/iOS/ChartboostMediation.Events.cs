using System.Runtime.InteropServices;
using AOT;
using Chartboost.Constants;

namespace Chartboost.Mediation.iOS
{
    internal sealed partial class ChartboostMediation
    {
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMSetPartnerAdapterInitializationResultsCallback(ExternChartboostMediationDataEvent didReceivePartnerInitializationChartboostMediationDataCallback);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern float _CBMGetUIScaleFactor();
        


        [MonoPInvokeCallback(typeof(ExternChartboostMediationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationDataJson)
            => Chartboost.Mediation.ChartboostMediation.OnDidReceivePartnerAdapterInitializationData(partnerInitializationDataJson);
    }
}
