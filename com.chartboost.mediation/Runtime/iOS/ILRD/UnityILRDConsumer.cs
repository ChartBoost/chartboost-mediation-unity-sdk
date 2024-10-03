using System.Runtime.InteropServices;
using AOT;
using Chartboost.Constants;
using Chartboost.Core;
using Chartboost.Core.Initialization;
using UnityEngine;

namespace Chartboost.Mediation.iOS.ILRD
{
    // ReSharper disable once InconsistentNaming
    internal class UnityILRDConsumer
    {
        internal UnityILRDConsumer()
        {
            if (Application.isEditor)
                return;
            _CBMSetUnityILRDProxy(ExternDidReceiveImpressionLevelRevenueData);
            ChartboostCore.ModuleInitializationCompleted += RetrieveILRDEvents;
        }
        
        // ReSharper disable once InconsistentNaming
        private static void RetrieveILRDEvents(ModuleInitializationResult result)
        {
            if (result.ModuleId != Mediation.ChartboostMediation.CoreModuleId)
                return;
            
            _CBMRetrieveImpressionData();
            ChartboostCore.ModuleInitializationCompleted -= RetrieveILRDEvents;
        }

        [MonoPInvokeCallback(typeof(ExternChartboostMediationImpressionLevelRevenueDataEvent))]
        private static void ExternDidReceiveImpressionLevelRevenueData(int hashCode, string impressionDataJson)
        {
            Chartboost.Mediation.ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(impressionDataJson);
            MainThreadDispatcher.Post(_ => _CBMCompleteUnityILRDRequest(hashCode));
        }
        
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMSetUnityILRDProxy(ExternChartboostMediationImpressionLevelRevenueDataEvent didReceivePartnerInitializationChartboostMediationDataCallback);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMRetrieveImpressionData();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMCompleteUnityILRDRequest(int uniqueId);
    }
}
