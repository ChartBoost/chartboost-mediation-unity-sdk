using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

namespace Dependencies
{
    /// <summary>
    /// Helper class that allows installation of any additional dependencies that are available to install using Unity Package Manager(UPM)
    /// This would allow installation of UPM based packages even when sdk is not installed using UPM
    /// </summary>
    [InitializeOnLoad]
    public static class ChartboostMediationDependencyPackageInstaller
    {
        private const string NewtonSoftJsonDependency = "com.unity.nuget.newtonsoft-json";
        private const string NewtonSoftJsonVersion = "3.2.1";

        private static readonly string[] DependencyPackages = {
            $"{NewtonSoftJsonDependency}@{NewtonSoftJsonVersion}"
        };
        
        static ChartboostMediationDependencyPackageInstaller()
        {
            foreach (var package in DependencyPackages)
            {
                Client.Add(package);
            }
        }
    }
}
