using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using Object = UnityEngine.Object;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Chartboost.Editor
{
    public static class Utilities
    {
        public static PackageInfo FindPackage(string packageName)
        {
            var packages = Client.List(false, false);
            while (!packages.IsCompleted) { }
            var packageInfos = packages.Result.ToList();
            var desiredPackage = packageInfos.Find(x => x.name == packageName);
            return desiredPackage;
        }

        private static readonly Regex SWhitespace = new Regex(@"\s+");

        public static string RemoveWhitespace(this string source) 
            => string.IsNullOrEmpty(source) ? source : SWhitespace.Replace(source, string.Empty);

        public static T LoadAsset<T>(this string path) where T : Object => path.FileExist() ? AssetDatabase.LoadAssetAtPath<T>(path) : null;
        public static string ReadAllText(this string path) => !path.FileExist() ? null : File.ReadAllText(path);

        public static string[] ReadAllLines(this string path) => !path.FileExist() ? null : File.ReadAllLines(path);

        public static bool FileExist(this string path) => !string.IsNullOrEmpty(path) && File.Exists(path);

        public static void FileCreate(this string path, IEnumerable<string> allLines)
        {
            if (allLines == null)
                return;
            File.WriteAllLines(path, allLines);
        }

        public static void FileCreate(this string path, string allText)
        {
            if (string.IsNullOrEmpty(allText))
                return;
            File.WriteAllText(path, allText);
        }

        public static bool DirectoryExists(this string path) => !string.IsNullOrEmpty(path) && Directory.Exists(path);

        public static void DirectoryCreate(this string path)
        {
            if (!path.DirectoryExists())
                Directory.CreateDirectory(path);
        }

        public static void DeleteFileWithMeta(this string path) => DeleteWithFunc(File.Exists, File.Delete, path);

        public static void DeleteDirectoryWithMeta(this string path) => DeleteWithFunc(Directory.Exists, location => Directory.Delete(location, true), path);

        private static void DeleteWithFunc(Func<string, bool> exist, Action<string> delete, string path)
        {
            if (exist(path))
                delete(path);
            var metaPath = $"{path}.meta";
            if (File.Exists(metaPath))
                File.Delete(metaPath);
            AssetDatabase.Refresh();
        }
    }
}
