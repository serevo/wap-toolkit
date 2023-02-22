using System.Configuration;
using System.IO;
using System.Linq;

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
        /// <param name="userLevel"></param>
        public static void MigrateExeConfiguration(ConfigurationUserLevel userLevel)
        {
            if (!PackageHelper.HasPackage) return;

            var urlRoot = new DirectoryInfo(WapConfigurationManagerHelper.GetExeConfigurationUrlRoot(userLevel));

            if (urlRoot.Exists) return;

            if (!urlRoot.Parent.Exists) return;

            var prefix = urlRoot.Name.Substring(0, urlRoot.Name.LastIndexOf("_Url_"));

            var latestUrlRoot = urlRoot.Parent
                .EnumerateDirectories($"{prefix}_url_*", SearchOption.TopDirectoryOnly)
                .OrderByDescending(o => o
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Select(oo => oo.LastWriteTime)
                    .OrderBy(v => v)
                    .LastOrDefault()
                    )
                .FirstOrDefault();

            var files = latestUrlRoot.GetFiles("*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileRelativePath = WapConfigurationManagerHelper.GetRelativePath(latestUrlRoot.FullName, file.FullName);

                var newFile = new FileInfo(Path.Combine(urlRoot.FullName, fileRelativePath));

                newFile.Directory.Create();

                file.CopyTo(newFile.FullName);
            }
        }
    }
}
