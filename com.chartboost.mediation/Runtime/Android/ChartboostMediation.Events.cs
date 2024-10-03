using Chartboost.Constants;
using Chartboost.Mediation.Android.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chartboost.Mediation.Android
{
    internal partial class ChartboostMediation 
    {
        internal class PartnerAdapterInitializationResultsObserver : AndroidJavaProxy
        {
            public PartnerAdapterInitializationResultsObserver() : base(AndroidConstants.ClassPartnerAdapterInitializationResultsObserver) { }
            
            // ReSharper disable once InconsistentNaming
            [Preserve]
            private void onPartnerAdapterInitializationResultsReady(AndroidJavaObject data) 
                => MainThreadDispatcher.Post(_ => Chartboost.Mediation.ChartboostMediation.OnDidReceivePartnerAdapterInitializationData(data.Get<AndroidJavaObject>(AndroidConstants.PropertyData).Call<string>(SharedAndroidConstants.FunctionToString)));
        }
    }
}
