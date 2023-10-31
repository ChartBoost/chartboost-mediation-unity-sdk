using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor.EditorWindows
{
    internal static class WindowPriorityManager
    {
        private static readonly List<WindowPriority> AllCustomWindows = new List<WindowPriority>();
        private static readonly Vector2 MinWindowSize = new Vector2(420, 520);
        private const string TargetAssembly = "com.chartboost.mediation.Editor";
        private const string TargetMethod = "GetWindow";
        private const string NameSuffix = "Window";
        private const string MenuItemConfigure = "Chartboost Mediation/Configure";

        #if !CB_NON_CONFIGURABLE
        [MenuItem(MenuItemConfigure)]
        #endif
        public static async void Show()
        {
            if (AllCustomWindows.Count == 0)
                return;
            
            var methodInfo = typeof(WindowPriorityManager).GetMethod(TargetMethod,BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo == null)
                return;
            
            var sortedWindows = AllCustomWindows.OrderBy(x => x.Priority).ToList();
            var firstInstance = sortedWindows[0];
            var firstWindowName = ExtractWindowName(firstInstance.Type.Name);
            
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
                // delaying in order to properly dock a window next to another, otherwise they are all created at once and can't be docked.
                await Task.Delay(3);
                var type = sortedWindows[i].Type;
                var nextWindowName = ExtractWindowName(type.Name);
                var priority = sortedWindows[i].Priority;
            
                Type nextToType = null;
                var nextTo = GetNextToWindow(priority);
                if (nextTo.HasValue)
                    nextToType = nextTo.Value.Type;
                
                var nextWindowCreate = methodInfo.MakeGenericMethod(type);
                var windowInstance = nextWindowCreate.Invoke(null, new object[] { nextWindowName, new[] { nextToType } }) as EditorWindow;
                if (windowInstance is ICustomWindow windowInstanceAsCustom) windowInstanceAsCustom.SetInstance(firstWindow);
                if (windowInstance == null)
                    continue;
                
                windowInstance.minSize = MinWindowSize;
                windowInstance.Show();
            }
            
            // focus back on the first window.
            if (firstWindow != null)
                firstWindow.Focus();
        }
        
        [InitializeOnLoadMethod]
        private static void FindAvailableWindows()
        {
            if (AllCustomWindows.Count > 0)
                return;

            var allAttributes = FindAllWindowsImplementations();
            
            if (allAttributes == null)
                return;

            var customEditorWindowAttributes = allAttributes as CustomEditorWindowAttribute[] ?? allAttributes.ToArray();
            
            foreach (var attribute in customEditorWindowAttributes)
                AddPriority(new WindowPriority(attribute.Type, attribute.Priority));

            if (AllCustomWindows.Count == 0)
                ChartboostEditorConfiguration.DisableConfigurability();
        }
        
        private static IEnumerable<CustomEditorWindowAttribute> FindAllWindowsImplementations() {
        
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filteredAssemblies = assemblies.Where(assembly 
                => assembly.FullName.Contains(TargetAssembly));
        
            var allTypes = filteredAssemblies.SelectMany(assembly => assembly.GetTypes());

            foreach(var type in allTypes)
            {
                var customEditorAttribute = type.GetCustomAttribute(typeof(CustomEditorWindowAttribute), true);
                if (customEditorAttribute != null)
                    yield return customEditorAttribute as CustomEditorWindowAttribute;
            }
        }

        private static void AddPriority(WindowPriority priority)
        {
            if (AllCustomWindows.Contains(priority))
                return;
            
            AllCustomWindows.Add(priority);
        }

        private static WindowPriority? GetNextToWindow(int priority)
        {
            if (AllCustomWindows.FindIndex(x => x.Priority < priority) < 0)
                return null;
            return AllCustomWindows[priority];
        }

        private static string ExtractWindowName(string window)
        {
            return window.Replace(NameSuffix, string.Empty);
        }

        // Replica from EditorWindow.GetWindow, using this until we figure out how to get this method through reflection from EditorWindow.
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
