using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    /// <summary>
    /// 
    /// </summary>
    public static class WapConfigurationManagerIntegration
    {
        const string _myLocalSettingsContainerKey = "e3953958-4b7c-4686-96e8-daacc0e358b1";

        static ApplicationDataContainer MyLocalSettingsContainer => ApplicationData.Current
            .LocalSettings.CreateContainer(_myLocalSettingsContainerKey, ApplicationDataCreateDisposition.Always);

        static string GetLatestUrlRoot(ConfigurationUserLevel userLevel) => MyLocalSettingsContainer
            .Values[userLevel.ToString()] as string;

        static void SetLatestUrlRoot(ConfigurationUserLevel userLevel, string path) => MyLocalSettingsContainer
            .Values[userLevel.ToString()] = path;

        static string GetRelativePath(string relativeTo, string path)
        {
            // Only .NET Core 2.0 +
            // return Path.GetRelativePath(appDataPath, originalPath);

            return new Uri(relativeTo + "\\").MakeRelativeUri(new Uri(path)).ToString().Replace("/", "\\");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLevel"></param>
        public static void MigrateExeConfiguration(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return;

            var urlRoot = new DirectoryInfo(GetExeConfigurationUrlRoot(userLevel));

            if (!urlRoot.Exists)
            {
                DirectoryInfo latestUrlRoot;

                var latestUrlRootPath = GetLatestUrlRoot(userLevel);

                if (latestUrlRootPath != null)
                {
                    latestUrlRoot = new DirectoryInfo(latestUrlRootPath);
                }
                else if (urlRoot.Parent.Exists)
                {
                    var prefix = urlRoot.Name.Substring(0, urlRoot.Name.LastIndexOf("_Url_"));

                    latestUrlRoot = urlRoot.Parent
                        .EnumerateDirectories($"{prefix}_url_*", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(o => o
                            .EnumerateFiles("*", SearchOption.AllDirectories)
                            .Select(oo => oo.LastWriteTime)
                            .OrderBy(v => v)
                            .LastOrDefault()
                            )
                        .FirstOrDefault();
                }
                else
                {
                    latestUrlRoot = null;
                }

                if (latestUrlRoot?.Exists == true)
                {
                    var files = latestUrlRoot.GetFiles("*", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        var fileRelativePath = GetRelativePath(latestUrlRoot.FullName, file.FullName);

                        var newFile = new FileInfo(Path.Combine(urlRoot.FullName, fileRelativePath));

                        newFile.Directory.Create();

                        file.CopyTo(newFile.FullName);
                    }
                }
            }

            SetLatestUrlRoot(userLevel, urlRoot.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLevel"></param>
        /// <returns></returns>
        public static string GetExeConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            var userConfig = ConfigurationManager.OpenExeConfiguration(userLevel).FilePath;

            var versionNumberFolder = Path.GetDirectoryName(userConfig);

            var urlPrefixedRootFolder = Path.GetDirectoryName(versionNumberFolder);

            return urlPrefixedRootFolder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLevel"></param>
        /// <returns></returns>
        public static string GetRedirectedExeConfigurationUrlRoot(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return null;

            var original = GetExeConfigurationUrlRoot(userLevel);

            var appDataLocalFolder = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

            var relative = GetRelativePath(appDataLocalFolder, original);

            var redirected = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, relative);

            return redirected;
        }
    }
}
