using Chartboost.Editor;
using Chartboost.Mediation;
using NUnit.Framework;

namespace Chartboost.Tests.Editor
{
    public class VersionValidator
    {
        private const string UnityPackageManagerPackageName = "com.chartboost.mediation";
        private const string NuGetPackageName = "Chartboost.CSharp.Mediation.Unity";
        
        [Test]
        public void ValidateVersion() 
            => VersionCheck.ValidateVersions(UnityPackageManagerPackageName, NuGetPackageName, Mediation.ChartboostMediation.SDKVersion);
    }
}
