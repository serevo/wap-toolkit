using System.Configuration;

namespace DemoSettingsClassLib.Properties
{

    [SettingsProvider(typeof(Serevo.WapDataContainerSettingsProvider))]
    internal sealed partial class Settings
    {
        public Settings()
        {
        }
    }
}
