using System.Runtime.CompilerServices;
using Chartboost.Mediation;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationEditorAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationAndroidAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationIOSAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationEditorTestsAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationRuntimeTestsAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.CanaryAutomationBannerHostAssembly)]

namespace Chartboost.Mediation
{
    internal static class AssemblyInfo
    {
        public const string ChartboostMediationEditorAssembly = "Chartboost.Mediation.Editor";
        public const string ChartboostMediationAndroidAssembly = "Chartboost.Mediation.Android";
        public const string ChartboostMediationIOSAssembly = "Chartboost.Mediation.iOS";
        public const string ChartboostMediationEditorTestsAssembly = "Chartboost.Mediation.Tests.Editor";
        public const string ChartboostMediationRuntimeTestsAssembly = "Chartboost.Mediation.Tests.Runtime";
        public const string ChartboostMediationAndroidRuntimeTestsAssembly = "Chartboost.Mediation.Tests.Runtime.Android";
        public const string CanaryAutomationBannerHostAssembly = "Chartboost.Canary.AutomationBannerHost";
        }
}
