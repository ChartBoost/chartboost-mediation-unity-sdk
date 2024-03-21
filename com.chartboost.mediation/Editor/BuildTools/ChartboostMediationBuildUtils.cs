using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chartboost.Editor.EditorWindows.Adapters;
using Chartboost.Editor.EditorWindows.Adapters.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor.BuildTools
{
    public class ChartboostMediationBuildUtils
    {
        private const string PathToAdapterUpdates = "adapter_updates.txt";
        private const string ArgBuildTarget = "buildTarget";
        private const string ArgAddNewNetworks = "addNewNetworks";

        private static readonly string EOL = System.Environment.NewLine;
        
        private const string Unselected = "Unselected";
        private static bool DefaultAddCondition(string id, Dictionary<string, AdapterSelection> selections)
        {
            if (id == "aps")
                return false;
            
            return !selections.ContainsKey(id) || selections[id].android == Unselected ||
                   selections[id].ios == Unselected;
        }

        [MenuItem("Chartboost Mediation/Update Adapters")]
        public static void UpdateAdapters()
        {
            ParseCommandLineArgumentsToUnity(out var args);
            args[ArgAddNewNetworks] = "true";
            args[ArgBuildTarget] = "android";
            
            var platform = GetPlatform(args);
            var adapterUpdates = string.Empty;
            if(File.Exists(adapterUpdates))    
                File.Delete(adapterUpdates);
            
            AdapterDataSource.Update();
            AdaptersWindow.LoadSelections();
            
            if(args.TryGetValue(ArgAddNewNetworks, out var addNetworks))
            {
                if (addNetworks == "true")
                {
                    var newNetworks = AdaptersWindow.AddNewNetworks(platform, DefaultAddCondition);

                    if (newNetworks.Count > 0)
                    {
                        adapterUpdates = $"New Networks: \n {JsonConvert.SerializeObject(newNetworks, Formatting.Indented)}";
                        File.AppendAllText(PathToAdapterUpdates, $"\n{adapterUpdates}");
                    }
                    Log(newNetworks.Count > 0 ? $"[Adapters] {adapterUpdates}" : "[Adapters] No New Networks");
                }
            }
            
            var upgrades = AdaptersWindow.UpgradePlatformToLatest(platform);
            if (upgrades.Count > 0)
            {
                adapterUpdates = $"Upgraded: \n {JsonConvert.SerializeObject(upgrades, Formatting.Indented)}";
                File.WriteAllText(PathToAdapterUpdates, adapterUpdates);
            }
            Log(upgrades.Count > 0 ? $"[Adapters] {adapterUpdates}" : "[Adapters] No Upgrades.");

            AdaptersWindow.SaveSelections();
            
            var changed = AdaptersWindow.CheckChartboostMediationVersion();
            Log(changed ? $"[Adapters] Chartboost Mediation Version Has Been Updated" :  "[Adapters] Chartboost Mediation Version is Up to Date");
        }
        
        private static void ParseCommandLineArgumentsToUnity(out Dictionary<string, string> providedArguments)
        {
            providedArguments = new Dictionary<string, string>();
            var args = System.Environment.GetCommandLineArgs();
            Debug.Log(
                $"{EOL}" +
                $"#####################{EOL}" +
                $"# Parsing Arguments #{EOL}" +
                $"#####################{EOL}" +
                $"{EOL}"
            );

            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse any flags
                var isFlag = args[current].StartsWith("-");
                if (!isFlag)
                    continue;
                var flag = args[current].TrimStart('-');

                // Parse optional value
                var flagHasValue = next < args.Length && !args[next].StartsWith("-");
                var flagValue = flagHasValue ? args[next].TrimStart('-') : string.Empty;
                var displayValue = $"{flagValue}";

                // Assign Value
                Debug.Log($"Found Flag \"{flag}\" with value \"{displayValue}\"");
                providedArguments.Add(flag, displayValue);
            }
        }
        
        private static void Log(string message)
        {
            if (Application.isBatchMode)
                Console.WriteLine(message);
            else
                Debug.Log(message);
        }

        private static Platform GetPlatform([NotNull] Dictionary<string, string> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (args.TryGetValue(ArgBuildTarget, out var buildPlatform))
                return buildPlatform switch
                {
                    "android" => Platform.Android,
                    "ios" => Platform.IOS,
                    _ => Platform.None
                };

            Log("ERROR => Platform not provided");
            return Platform.None;
        }
    }
}
