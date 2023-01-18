using DemoSettingsClassLib.Properties;

namespace DemoSettingsClassLib
{
    public static class SettingsService
    {
        public static int Int32Value
        {
            get => Settings.Default.Int32Value;
            set
            {
                Settings.Default.Int32Value = value;
                Settings.Default.Save();
            }
        }

        public static int Int32Value2
        {
            get => Settings2.Default.Int32Value2;
            set
            {
                Settings2.Default.Int32Value2 = value;
                Settings2.Default.Save();
            }
        }
    }
}
