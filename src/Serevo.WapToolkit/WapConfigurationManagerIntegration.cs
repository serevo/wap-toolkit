using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    static class WapConfigurationManagerIntegration
    {
        public static void UpgradeUserConfiguration(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return;

            var originalRoot = new DirectoryInfo(GetUserConfigurationUrlRoot(userLevel));

            if (originalRoot.Exists) return;

            if (!originalRoot.Parent.Exists) return;

            var oldRoot = originalRoot.Parent
                .EnumerateDirectories()
                .OrderByDescending(o=> o
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Select(oo => oo.LastWriteTime)
                    .OrderBy(v => v)
                    .LastOrDefault()
                    )
                .FirstOrDefault();

            if (oldRoot is null) return;

            var oldFiles = oldRoot.GetFiles("*", SearchOption.AllDirectories);

            foreach(var oldFile in oldFiles)
            {
                var oldFileRelativePath = GetRelativePath(oldRoot.FullName, oldFile.FullName);

                var newFile = new FileInfo(Path.Combine(originalRoot.FullName, oldFileRelativePath));

                newFile.Directory.Create();

                oldFile.CopyTo(newFile.FullName);
            }
        }

        public static string GetUserConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(userLevel).FilePath;

            var versionNumberFolder = Path.GetDirectoryName(userConfig);

            var urlPrefixedRootFolder = Path.GetDirectoryName(versionNumberFolder);

            return urlPrefixedRootFolder;
        }

        public static string GetRedirectedUserConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return null;

            var original = GetUserConfigurationUrlRoot(userLevel);

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
