using System;
using System.Deployment.Application;
using System.Reflection;
using Windows.ApplicationModel;

namespace DemoWinFormsApp
{
    public static class AppInfoHelper
    {
        public static bool HasPackage { get; }

        public static Package Package { get; }

        static AppInfoHelper()
        {
            try
            {
                Package = Package.Current;

                HasPackage = true;
            }
            catch (InvalidOperationException)
            {
                HasPackage = false;
            }
        }

        public static string GetDisplayName()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
            }
            else if (HasPackage)
            {
                return Package.DisplayName;
            }
            else
            {
                return Assembly.GetCallingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            }
        }

        public static Version GetVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            else if (HasPackage)
            {
                var version = Package.Id.Version;

                return new Version(version.Major, version.Minor, version.Build, version.Revision);
            }
            else
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }
}
