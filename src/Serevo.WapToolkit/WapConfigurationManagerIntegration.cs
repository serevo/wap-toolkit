using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    static class WapConfigurationManagerIntegration
    {
        public static void UpgradeExeConfiguration(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return;

            var originalUrlRoot = new DirectoryInfo(GetExeConfigurationUrlRoot(userLevel));

            if (originalUrlRoot.Exists) return;

            if (!originalUrlRoot.Parent.Exists) return;

            var prefix = originalUrlRoot.Name.Substring(0, originalUrlRoot.Name.LastIndexOf("_Url_"));

            var previousUrlRoot = originalUrlRoot.Parent
                .EnumerateDirectories($"{prefix}_url_*", SearchOption.TopDirectoryOnly)
                .OrderByDescending(o => o
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Select(oo => oo.LastWriteTime)
                    .OrderBy(v => v)
                    .LastOrDefault()
                    )
                .FirstOrDefault();

            if (previousUrlRoot is null) return;

            var files = previousUrlRoot.GetFiles("*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileRelativePath = GetRelativePath(previousUrlRoot.FullName, file.FullName);

                var newFile = new FileInfo(Path.Combine(originalUrlRoot.FullName, fileRelativePath));

                newFile.Directory.Create();

                file.CopyTo(newFile.FullName);
            }
        }

        public static string GetExeConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(userLevel).FilePath;

            var versionNumberFolder = Path.GetDirectoryName(userConfig);

            var urlPrefixedRootFolder = Path.GetDirectoryName(versionNumberFolder);

            return urlPrefixedRootFolder;
        }

        public static string GetRedirectedExeConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return null;

            var original = GetExeConfigurationUrlRoot(userLevel);

            var appDataLocalFolder = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            var relative = GetRelativePath(appDataLocalFolder, original);

            var redirected = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, relative);

            return redirected;
        }

        static string GetRelativePath(string relativeTo, string path)
        {
            // Only .NET Core 2.0 +
            // return Path.GetRelativePath(appDataPath, originalPath);

            return new Uri(relativeTo + "\\").MakeRelativeUri(new Uri(path)).ToString().Replace("/", "\\");
        }
    }
}
