using System.Configuration;

namespace DemoSettingsClassLib.Properties
{

    [SettingsProvider(typeof(Serevo.WapToolkit.WapDataContainerSettingsProvider))]
    internal sealed partial class Settings
    {
        public Settings()
        {
        }
    }
}
