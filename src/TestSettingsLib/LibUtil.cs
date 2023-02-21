using System.Configuration;
using Serevo.WapToolkit;

namespace TestSettingsLib
{
    public static class LibUtil
    {
        public static void CallWapConfigManMigrate(ConfigurationUserLevel userLevel)
        {
            WapConfigurationManagerIntegration.MigrateExeConfiguration(userLevel);
        }
    }
}
