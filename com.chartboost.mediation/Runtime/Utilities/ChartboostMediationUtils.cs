using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Chartboost.Utilities
{
    public static class ChartboostMediationUtils
    {
        private const string ChartboostMediationUPMPackageName = "com.chartboost.mediation";
        private const string ChartboostMediationNugetPackageName = "Chartboost.CSharp.Mediation.Unity";
        
        private const string CanvasName = "Canvas";
        private const string UPMPackagesPath = "Packages";
        private const string NugetPackagesPath = "Assets/Packages";
        private static readonly string ChartboostMediationUPMPackagePath = $"{UPMPackagesPath}/{ChartboostMediationUPMPackageName}";
        private static readonly string ChartboostMediationNugetPackagePath = $"{NugetPackagesPath}/{ChartboostMediationNugetPackageName}.{ChartboostMediation.Version}";

        /// <summary>
        /// Gets the location where Chartboost Mediation Unity SDK Package is installed
        /// </summary>
        public static string ChartboostMediationPackageLocation => Directory.Exists(ChartboostMediationUPMPackagePath) ?
            // UPM
            ChartboostMediationUPMPackagePath :
            // Nuget
            ChartboostMediationNugetPackagePath;


        public static Canvas GetCanvas()
        {
            // Find the root-level canvas with highest sorting order
            var canvas = GetRootLevelCanvasWithHighestSortingOrder();

            // If no root-level canvas is available, look for all the other canvas in scene and find the one with highest sorting order
            canvas ??= GetCanvasWithHighestSortingOrder();

            // If no canvas available anywhere in the scene then create a new canvas
            // ReSharper disable once InvertIf
            if (canvas == null)
            {
                // This canvas is similar to the default canvas that unity creates when we try to add an UI element with no canvas in scene
                canvas = new GameObject(CanvasName).AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            
            return canvas;
        }

        private static Canvas GetCanvasWithHighestSortingOrder()
        {
            Canvas canvas = null;
            foreach (var can in Object.FindObjectsOfType<Canvas>().OrderByDescending(x => x.sortingOrder))
            {
                // Make sure the canvas is not within another canvas
                canvas = can;
                if (!can.GetComponentInParent<Canvas>())
                    break;
            }

            return canvas;
        }
        
        private static Canvas GetRootLevelCanvasWithHighestSortingOrder()
        {
            Canvas canvas = null;
            var canvases = (from go in SceneManager.GetActiveScene().GetRootGameObjects()
                    where go.GetComponent<Canvas>()
                    select go.GetComponent<Canvas>())
                .OrderByDescending(x => x.sortingOrder);

            // ReSharper disable once PossibleMultipleEnumeration
            if (canvases.Any())
                // ReSharper disable once PossibleMultipleEnumeration
                canvas = canvases.First();
            
            return canvas;
        }
    }
}
