using System;
using System.Configuration;
using System.IO;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    static class WapConfigurationManagerHelper
    {
        public static string GetExeConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(userLevel).FilePath;

            var versionNumberFolder = Path.GetDirectoryName(userConfig);

            var urlPrefixedRootFolder = Path.GetDirectoryName(versionNumberFolder);

            return urlPrefixedRootFolder;
        }

        public static string GetExeConfigurationUrlRootRedirected(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return null;

            var original = GetExeConfigurationUrlRoot(userLevel);

            var appDataLocalFolder = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            var relative = GetRelativePath(appDataLocalFolder, original);

            var redirected = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, relative);

            return redirected;
        }

        public static string GetRelativePath(string relativeTo, string path)
        {
            // Only .NET Core 2.0 +
            // return Path.GetRelativePath(appDataPath, originalPath);

            return new Uri(relativeTo + "\\").MakeRelativeUri(new Uri(path)).ToString().Replace("/", "\\");
        }
    }
}
