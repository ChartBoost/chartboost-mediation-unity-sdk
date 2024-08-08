using System;
using System.IO;
using System.Linq;
using Chartboost.Logging;
using UnityEditor.Android;

namespace Chartboost.Mediation.Editor.Android
{
    public class GradlePostGenerate : IPostGenerateGradleAndroidProject
    {
        private const string R8Dependency = "        classpath('com.android.tools:r8:8.3.37')";
        private const string BuildGradle = "build.gradle";
        private const string ClassPath = "classpath";
        
        public int callbackOrder => 0;
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            LogController.LoggingLevel = LogLevel.Debug;
            var rootDirectoryInfo = new DirectoryInfo(path);
            if (rootDirectoryInfo.Parent == null) 
                return;
            var rootPath = rootDirectoryInfo.Parent.FullName;
            
            var buildGradleFilePath = Path.Combine(rootPath, BuildGradle);
            
            if (!File.Exists(buildGradleFilePath))
                return;

            var buildGradleContents = File.ReadAllLines(buildGradleFilePath).ToList();
            var indexOfClassPath = buildGradleContents.FindIndex( x => x.Contains(ClassPath, StringComparison.OrdinalIgnoreCase));

            // No classpath exists
            if (indexOfClassPath == -1)
            {
                var contents = $"buildscript {{\n    dependencies {{\n{R8Dependency}\n    }}\n}}\n";
                buildGradleContents.Insert(0, contents);
                LogController.Log($"R8 Dependencies not found, inserting in {BuildGradle}", LogLevel.Debug);
            }
            else
            {
                if (buildGradleContents.Find(x => x.Contains(R8Dependency)) != null)
                {
                    LogController.Log($"R8 Dependencies already present! All OK.", LogLevel.Debug);
                    return;
                }

                LogController.Log($"R8 Dependencies not found but classpath dependencies present, inserting R8 in {BuildGradle}", LogLevel.Debug);
                buildGradleContents.Insert(indexOfClassPath, R8Dependency);
            }
            
            File.WriteAllLines(buildGradleFilePath, buildGradleContents);
        }
    }
}
