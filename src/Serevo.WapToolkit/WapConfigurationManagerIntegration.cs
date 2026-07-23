using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    /// <summary>
    /// Support <see cref="ConfigurationManager"/> Integration.
    /// </summary>
    public static class WapConfigurationManagerIntegration
    {
        /// <summary>
        /// Move configuration files of executable (.exe) managed by ConfigurationManager to new location. Call this method before calling any configuration members.
        /// </summary>
        /// <param name="userLevel">Specify PerUserRoaming or PerUserRoamingAndLocal.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void MigrateUnsignedExeConfiguration(ConfigurationUserLevel userLevel)
        {
            if (userLevel != ConfigurationUserLevel.PerUserRoaming &
                userLevel != ConfigurationUserLevel.PerUserRoamingAndLocal)
                throw new ArgumentOutOfRangeException(nameof(userLevel));

            if (!PackageHelper.HasPackage) return;

            var urlRoot = new DirectoryInfo(GetExeConfigurationUrlRoot(userLevel));

            if (urlRoot is null || urlRoot.Exists) return;

            if (urlRoot.Parent?.Exists != true) return;

            var index = urlRoot.Name.LastIndexOf("_Url_", StringComparison.OrdinalIgnoreCase);

            if (index < 0) return;

            var prefix = urlRoot.Name.Substring(0, index);

            var latestUrlRoot = urlRoot.Parent
                .EnumerateDirectories($"{prefix}_url_*", SearchOption.TopDirectoryOnly)
                .OrderByDescending(o => o
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Select(oo => oo.LastWriteTime)
                    .OrderBy(v => v)
                    .LastOrDefault()
                    )
                .FirstOrDefault();

            if (latestUrlRoot is null) return;

            var files = latestUrlRoot.GetFiles("*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileRelativePath = GetRelativePath(latestUrlRoot.FullName, file.FullName);

                var newFile = new FileInfo(Path.Combine(urlRoot.FullName, fileRelativePath));

                newFile.Directory.Create();

                file.CopyTo(newFile.FullName);
            }
        }

        internal static string GetExeConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(userLevel).FilePath;

            var versionNumberFolder = Path.GetDirectoryName(userConfig);

            var urlPrefixedRootFolder = Path.GetDirectoryName(versionNumberFolder);

            return urlPrefixedRootFolder;
        }

        internal static string GetExeConfigurationUrlRootRedirected(ConfigurationUserLevel userLevel)
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
