using Chartboost.Editor.Adapters;
using Chartboost.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
    public class Constants
    {
        public static readonly Vector2 MinWindowSize = new Vector2(420, 520);
        
        [MenuItem("Chartboost Mediation/Configure")]
        private static void MenuWindow()
        {
            // Create and Focus Adapters Window Instance
            AdaptersWindow.Instance.Focus();
            // Create Settings Window and Dock it into Adapters Window.
            SettingsWindow.Instance.Show();
            // Re-focus Adapters Window
            AdaptersWindow.Instance.Focus(); 
        }

    }
}
