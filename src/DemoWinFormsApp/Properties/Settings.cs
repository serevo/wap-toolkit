using System.Configuration;

namespace DemoWinFormsApp.Properties
{

    [SettingsProvider(typeof(Serevo.WapDataContainerSettingsProvider))]
    internal sealed partial class Settings
    {
        public Settings()
        {
        }
    }
}
