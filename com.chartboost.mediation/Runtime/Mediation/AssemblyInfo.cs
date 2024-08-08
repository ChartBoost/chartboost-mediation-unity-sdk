using System.Runtime.CompilerServices;
using Chartboost.Mediation;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationEditorAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationAndroidAssembly)]
[assembly: InternalsVisibleTo(AssemblyInfo.ChartboostMediationIOSAssembly)]

namespace Chartboost.Mediation
{
    internal static class AssemblyInfo
    {
        public const string ChartboostMediationEditorAssembly = "Chartboost.Mediation.Editor";
        public const string ChartboostMediationAndroidAssembly = "Chartboost.Mediation.Android";
        public const string ChartboostMediationIOSAssembly = "Chartboost.Mediation.iOS";
    }
}
