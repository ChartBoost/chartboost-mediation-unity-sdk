using System.Runtime.CompilerServices;
using Chartboost.Mediation;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationEditorAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationAndroidAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationIOSAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationEditorTestsAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationRuntimeTestsAssembly)]

namespace Chartboost.Mediation
{
    internal static class AssemblyInfo
    {
        public const string ChartboostMediationEditorAssembly = "Chartboost.Mediation.Editor";
        public const string ChartboostMediationAndroidAssembly = "Chartboost.Mediation.Android";
        public const string ChartboostMediationIOSAssembly = "Chartboost.Mediation.iOS";
        public const string ChartboostMediationEditorTestsAssembly = "Chartboost.Mediation.Test.Editor";
        public const string ChartboostMediationRuntimeTestsAssembly = "Chartboost.Mediation.Tests.Runtime";
    }
}
