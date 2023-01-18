using System.Configuration;

namespace DemoWinFormsApp.Properties
{

    [SettingsProvider(typeof(Serevo.WapToolkit.WapDataContainerSettingsProvider))]
    internal sealed partial class Settings
    {
        public Settings()
        {
        }
    }
}
