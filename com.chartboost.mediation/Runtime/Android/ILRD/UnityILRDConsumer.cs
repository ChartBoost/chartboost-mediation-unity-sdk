using Chartboost.Constants;
using Chartboost.Core;
using Chartboost.Core.Initialization;
using Chartboost.Mediation.Android.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chartboost.Mediation.Android.ILRD
{
    // ReSharper disable once InconsistentNaming
    internal class UnityILRDConsumer : AndroidJavaProxy
    {
        internal UnityILRDConsumer() : base(AndroidConstants.UnityILRDConsumer)
        {
            if (Application.isEditor)
                return;
            
            using var ilrdObserver = new AndroidJavaClass(AndroidConstants.UnityILRDObserver);
            ilrdObserver.CallStatic(AndroidConstants.FunctionSetUnityILRDProxy, this);
            ChartboostCore.ModuleInitializationCompleted += RetrieveILRDEvents;
        }
        
        // ReSharper disable once InconsistentNaming
        private static void RetrieveILRDEvents(ModuleInitializationResult result)
        {
            if (result.ModuleId != Mediation.ChartboostMediation.CoreModuleId)
                return;

            using var unityIlrdObserver = new AndroidJavaClass(AndroidConstants.UnityILRDObserver);
            unityIlrdObserver.CallStatic(AndroidConstants.FunctionRetrieveImpressionData);
            ChartboostCore.ModuleInitializationCompleted -= RetrieveILRDEvents;
        }

        [Preserve]
        // ReSharper disable once InconsistentNaming
        void onImpression(int uniqueId, string ilrdJson, AndroidJavaObject completer)
        {
            Chartboost.Mediation.ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(ilrdJson);
            MainThreadDispatcher.Post(_ => completer.Call(SharedAndroidConstants.FunctionCompleted, uniqueId));
        }
    }
}
