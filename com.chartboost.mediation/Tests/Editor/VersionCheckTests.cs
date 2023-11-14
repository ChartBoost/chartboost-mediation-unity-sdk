using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Chartboost.Tests.Editor
{
    public class VersionCheckTests
    {
        private const string ChartboostMediationUPMPackageName = "com.chartboost.mediation";
        private const string ChartboostMediationNuGetPackageName = "Chartboost.CSharp.Mediation.Unity";

        [SetUp]
        public void Setup()
        {
            Debug.Log($"SDK Version : {ChartboostMediation.Version}");
        }

        [Test]
        public void CompareUPMVersionWithSDKVersion()
        {
            var packageJson = Directory.GetFiles(ChartboostMediationPackageLocation, "*.json").First();
            var upmVersion = GetUPMVersion(packageJson);
            
            Debug.Log($"UPMVersion : {upmVersion}");
            
            Assert.AreEqual(ChartboostMediation.Version, upmVersion);
        }
        
        [Test]
        public void CompareNuGetVersionWithSDKVersion()
        {
            var nuspec = Directory.GetFiles(ChartboostMediationPackageLocation, "*.nuspec").First();
            var nuGetVersion = GetNuGetVersion(nuspec);
            
            Debug.Log($"NuGetVersion : {nuGetVersion}");
            
            Assert.AreEqual(ChartboostMediation.Version, nuGetVersion);
        }
        
        private static string ChartboostMediationPackageLocation => Directory.Exists($"Packages/{ChartboostMediationUPMPackageName}") ?
            // UPM
            $"Packages/{ChartboostMediationUPMPackageName}" :
            // Nuget
            $"Assets/Packages/{ChartboostMediationNuGetPackageName}";
        
        private static string GetUPMVersion(string filePath)
        {
            Debug.Log($"UPM path : {filePath}");
            try
            {
                var jsonContent = System.IO.File.ReadAllText(filePath);
                var jsonData = JsonUtility.FromJson<PackageJsonData>(jsonContent);
                return jsonData.version;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while reading the package.json file: {ex.Message}");
                return null;
            }
        }
        
        private static string GetNuGetVersion(string filePath)
        {
            Debug.Log($"NuGet path : {filePath}");
            var xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.Load(filePath);
                
                // Assuming the version is specified in the metadata section
                var versionNode = xmlDoc.SelectSingleNode("/package/metadata/version");
                
                if (versionNode != null)
                {
                    return versionNode.InnerText.Trim();
                }
                
                Debug.LogError("Version not found in the .nuspec metadata section.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while reading the .nuspec file: {ex.Message}");
                return null;
            }
        }
    }
    
    [Serializable]
    public class PackageJsonData
    {
        public string version;
        // Add other properties as needed
    }
}
