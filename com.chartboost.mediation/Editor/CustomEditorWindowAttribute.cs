using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class CustomEditorWindowAttribute : Attribute
    {
        public readonly Type Type;
        public readonly int Priority;
        
        public CustomEditorWindowAttribute(Type t, int priority)
        {
            Type = t;
            Priority = priority;
        }
    }

    internal struct WindowPriority
    {
        public readonly Type Type;

        public readonly int Priority;

        public WindowPriority(Type t, int priority)
        {
            Type = t;
            Priority = priority;
        }
    }

    public static class ChartboostEditorConfiguration
    {
        private const char ScriptingDefineSymbolsSeparator = ';';
        public const string AdaptersWindowDisableScriptingDefineSymbol = "NO_ADAPTERS_WINDOW";
        public const string SettingsWindowDisableScriptingDefineSymbol = "NO_SETTINGS_WINDOW";
        public const string ConfigurationDisableScriptingDefineSymbol = "CB_NON_CONFIGURABLE";

        public static void EnableConfigurability() => RemoveScriptDefineToAllPlatforms(ConfigurationDisableScriptingDefineSymbol);
        public static void DisableConfigurability() => AddScriptDefineToAllPlatforms(ConfigurationDisableScriptingDefineSymbol);
        
        [MenuItem("Window/Chartboost/Toggle Configurabilty")]
        public static void ToggleConfigurability() => ToggleAllSupportedPlatforms(ConfigurationDisableScriptingDefineSymbol);

        [MenuItem("Window/Chartboost/Toggle Settings Window")]
        public static void ToggleSettingsWindow() => ToggleAllSupportedPlatforms(SettingsWindowDisableScriptingDefineSymbol);
        
        [MenuItem("Window/Chartboost/Toggle Adapters Window")]
        public static void ToggleAdaptersWindow() => ToggleAllSupportedPlatforms(AdaptersWindowDisableScriptingDefineSymbol);


        private static void AddScriptDefineToAllPlatforms(params string[] symbols)
        {
            AddScriptDefineSymbols(BuildTargetGroup.Android, symbols);
            AddScriptDefineSymbols(BuildTargetGroup.iOS, symbols);
            AddScriptDefineSymbols(BuildTargetGroup.Standalone, symbols);
        }
        
        private static void RemoveScriptDefineToAllPlatforms(params string[] symbols)
        {
            RemoveScriptDefineSymbols(BuildTargetGroup.Android, symbols);
            RemoveScriptDefineSymbols(BuildTargetGroup.iOS, symbols);
            RemoveScriptDefineSymbols(BuildTargetGroup.Standalone, symbols);
        }

        private static void ToggleAllSupportedPlatforms(params string[] symbols)
        {
            ToggleScriptingDefineSymbols(BuildTargetGroup.Android, symbols);
            ToggleScriptingDefineSymbols(BuildTargetGroup.iOS, symbols);
            ToggleScriptingDefineSymbols(BuildTargetGroup.Standalone, symbols);
        }

        private static void ToggleScriptingDefineSymbols(BuildTargetGroup group, params string[] symbols)
        {
            var separatedSymbols = SplitScriptDefines(group); 
            foreach (var targetSymbol in symbols)
                if (separatedSymbols.Contains(targetSymbol))
                    separatedSymbols.Remove(targetSymbol);
                else
                    separatedSymbols.Add(targetSymbol);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, separatedSymbols.ToArray());
        }

        private static void AddScriptDefineSymbols(BuildTargetGroup group, params string[] symbols)
        {
            var separatedSymbols = SplitScriptDefines(group); 
            
            foreach (var targetSymbol in symbols)
                if (!separatedSymbols.Contains(targetSymbol))
                    separatedSymbols.Add(targetSymbol);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, separatedSymbols.ToArray());
        }
        
        private static void RemoveScriptDefineSymbols(BuildTargetGroup group, params string[] symbols)
        {
            var separatedSymbols = SplitScriptDefines(group); 
            foreach (var targetSymbol in symbols)
                if (separatedSymbols.Contains(targetSymbol))
                    separatedSymbols.Remove(targetSymbol);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, separatedSymbols.ToArray());
        }

        private static List<string> SplitScriptDefines(BuildTargetGroup group)
        {
            var existingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            return existingSymbols.Split(ScriptingDefineSymbolsSeparator).ToList();
        }
    }

    internal static class WindowPriorityManager
    {
        private static readonly List<WindowPriority> AllCustomWindows = new List<WindowPriority>();
        public static readonly Vector2 MinWindowSize = new Vector2(420, 520);
        
        [InitializeOnLoadMethod]
        public static void FindWindows()
        {
            if (AllCustomWindows.Count > 0)
                return;

            var allAttributes = AllCustomAttributeImplementations();
            
            if (allAttributes == null)
                return;

            var customEditorWindowAttributes = allAttributes as CustomEditorWindowAttribute[] ?? allAttributes.ToArray();
            
            foreach (var attribute in customEditorWindowAttributes)
                AddPriority(new WindowPriority(attribute.Type, attribute.Priority));

            if (AllCustomWindows.Count == 0)
                ChartboostEditorConfiguration.DisableConfigurability();
        }
        
        
        private static IEnumerable<CustomEditorWindowAttribute> AllCustomAttributeImplementations() {
        
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.Where(assembly 
                => assembly.FullName.Contains("com.chartboost.mediation.Editor"));
        
            var allTypes = filteredAssemblies.SelectMany(assembly => assembly.GetTypes());

            foreach(var type in allTypes)
            {
                var customEditorAttribute = type.GetCustomAttribute(typeof(CustomEditorWindowAttribute), true);
                if (customEditorAttribute != null)
                    yield return customEditorAttribute as CustomEditorWindowAttribute;
            }
        }

        public static void AddPriority(WindowPriority priority)
        {
            if (AllCustomWindows.Contains(priority))
                return;
            
            AllCustomWindows.Add(priority);
        }

        public static WindowPriority? GetNextToWindow(int priority)
        {
            if (AllCustomWindows.FindIndex(x => x.Priority < priority) < 0)
                return null;
            return AllCustomWindows[priority];
        }

        #if !CB_NON_CONFIGURABLE
        [MenuItem("Chartboost Mediation/Configure")]
        #endif
        public static async void GetFirstWindowAndConfigure()
        {
            if (AllCustomWindows.Count == 0)
                return;
            
            var methodInfo = typeof(WindowPriorityManager).GetMethod("GetWindow",BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo == null)
                return;
            
            var sortedWindows = AllCustomWindows.OrderBy(x => x.Priority).ToList();
            var firstInstance = sortedWindows[0];
            var firstWindowName = firstInstance.Type.Name.Replace("Window", "");
            
            var genericMethodInfo = methodInfo.MakeGenericMethod(firstInstance.Type);
            var firstWindow = genericMethodInfo.Invoke(null, new object[] { firstWindowName, new Type[] {} }) as EditorWindow;
            if (firstWindow is ICustomWindow windowAsCustomWindow) windowAsCustomWindow.SetInstance(firstWindow);

            if (firstWindow != null)
            {
                firstWindow.minSize = MinWindowSize;
                firstWindow.Show();
            }


            for (var i = 1; i < sortedWindows.Count; i++)
            {
                await Task.Delay(3);
                var type = sortedWindows[i].Type;
                var nextWindowName = type.Name.Replace("Window", "");
                var priority = sortedWindows[i].Priority;
            
                Type nextToType = null;
                var nextTo = GetNextToWindow(priority);
                if (nextTo.HasValue)
                    nextToType = nextTo.Value.Type;
                
                var nextWindowCreate = methodInfo.MakeGenericMethod(type);
                var windowInstance = nextWindowCreate.Invoke(null, new object[] { nextWindowName, new[] { nextToType } }) as EditorWindow;
                if (windowInstance is ICustomWindow windowInstanceAsCustom) windowInstanceAsCustom.SetInstance(firstWindow);
                if (windowInstance != null)
                {
                    windowInstance.minSize = MinWindowSize;
                    windowInstance.Show();
                }
            }
            
            if (firstWindow != null)
                firstWindow.Focus();
        }

        private static T GetWindow<T>(string title, params Type[] desiredDockNextTo ) where T : EditorWindow
        {
            T[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof (T)) as T[];
            T window = objectsOfTypeAll.Length != 0 ? objectsOfTypeAll[0] : default (T);
            if (!((UnityEngine.Object)window != (UnityEngine.Object)null))
                return EditorWindow.CreateWindow<T>(title, desiredDockNextTo);

            window.Focus();
            return window;
        }
    }
}
