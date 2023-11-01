using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Chartboost.Editor
{
    public static class ChartboostEditorConfiguration
    {
        private const char ScriptingDefineSymbolsSeparator = ';';
        private const string MenuItemEnableConfigurability = "Window/Chartboost/Enable Configurability";
        private const string MenuItemDisableConfigurability = "Window/Chartboost/Disable Configurability";
        private const string MenuItemEnableAdaptersWindow = "Window/Chartboost/Enable Adapters Window";
        private const string MenuItemDisableAdaptersWindow = "Window/Chartboost/Disable Adapters Window";
        private const string MenuItemEnableSettingsWindow = "Window/Chartboost/Enable Settings Window";
        private const string MenuItemDisableSettingsWindow = "Window/Chartboost/Disable Settings Window";
        public const string AdaptersWindowDisableScriptingDefineSymbol = "NO_ADAPTERS_WINDOW";
        public const string SettingsWindowDisableScriptingDefineSymbol = "NO_SETTINGS_WINDOW";
        public const string ConfigurationDisableScriptingDefineSymbol = "CB_NON_CONFIGURABLE";

        [MenuItem(MenuItemEnableConfigurability)]
        public static void EnableConfigurability() =>
            RemoveScriptDefineInAllPlatforms(ConfigurationDisableScriptingDefineSymbol);

        [MenuItem(MenuItemDisableConfigurability)]
        public static void DisableConfigurability() =>
            AddScriptDefineInAllPlatforms(ConfigurationDisableScriptingDefineSymbol);

        [MenuItem(MenuItemEnableAdaptersWindow)]
        public static void EnableAdaptersWindow() =>
            RemoveScriptDefineInAllPlatforms(AdaptersWindowDisableScriptingDefineSymbol);

        [MenuItem(MenuItemDisableAdaptersWindow)]
        public static void DisableAdaptersWindow() =>
            AddScriptDefineInAllPlatforms(AdaptersWindowDisableScriptingDefineSymbol);

        [MenuItem(MenuItemEnableSettingsWindow)]
        public static void EnableSettingsWindow() =>
            RemoveScriptDefineInAllPlatforms(SettingsWindowDisableScriptingDefineSymbol);

        [MenuItem(MenuItemDisableSettingsWindow)]
        public static void DisableSettingssWindow() =>
            AddScriptDefineInAllPlatforms(SettingsWindowDisableScriptingDefineSymbol);

        public static void AddScriptDefineInAllPlatforms(params string[] symbols)
        {
            AddScriptDefineSymbols(BuildTargetGroup.Android, symbols);
            AddScriptDefineSymbols(BuildTargetGroup.iOS, symbols);
            AddScriptDefineSymbols(BuildTargetGroup.Standalone, symbols);
        }

        public static void RemoveScriptDefineInAllPlatforms(params string[] symbols)
        {
            RemoveScriptDefineSymbols(BuildTargetGroup.Android, symbols);
            RemoveScriptDefineSymbols(BuildTargetGroup.iOS, symbols);
            RemoveScriptDefineSymbols(BuildTargetGroup.Standalone, symbols);
        }

        public static void ToggleScriptDefineInAllSupportedPlatforms(params string[] symbols)
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
}
