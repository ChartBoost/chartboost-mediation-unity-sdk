using Chartboost.Editor;
using NUnit.Framework;

namespace Chartboost.Tests.Editor
{
    public class VersionCheckTests
    {
        private const string UnityPackageManagerPackageName = "com.chartboost.mediation";
        private const string NuGetPackageName = "Chartboost.CSharp.Mediation.Unity";
        
        [Test]
        public void ValidateVersion() 
            => VersionCheck.ValidateVersions(UnityPackageManagerPackageName, NuGetPackageName, ChartboostMediation.Version);
    }
}
