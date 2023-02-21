using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Serevo.WapToolkit;
using AppSettings1 = TestConsoleApp.Properties.AppSettings;
using LibSettings1 = TestSettingsLib.Properties.LibSettings1;

namespace TestConsoleApp
{
    static class App
    {
        static void Main()
        {
            while (true)
            {
                Console.Write("Input command (h: Help) > ");

                switch (Console.ReadLine().ToLower())
                {
                    case "h":

                        Console.WriteLine("s: Display Local Configuration Url Root Folder exists status");
                        Console.WriteLine("s-o: Display Other Config Path");

                        Console.WriteLine("o: Open AppCenter Config File location");

                        Console.WriteLine("u: Move AppCenter Config File previous version to new version's location");

                        Console.WriteLine();

                        continue;

                    case "m":

                        WapConfigurationManagerIntegration.MigrateExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

                        continue;

                    case "ml":

                        TestSettingsLib.LibUtil.CallWapConfigManMigrate(ConfigurationUserLevel.PerUserRoamingAndLocal);

                        continue;

                    case "values":

                        Console.WriteLine($"AppSettings1.Value: {AppSettings1.Default.Value}");
                        Console.WriteLine($"LibSettings1.Value: {LibSettings1.Default.Value}");
                        Console.WriteLine();
                        continue;

                    case "exists":

                        string existsString(bool exists) => exists ? "Exists" : "Not Exists";
                        var local = WapConfigurationManagerIntegration.GetExeConfigurationUrlRoot(ConfigurationUserLevel.PerUserRoamingAndLocal);
                        var roaming = WapConfigurationManagerIntegration.GetExeConfigurationUrlRoot(ConfigurationUserLevel.PerUserRoaming);
                        Console.WriteLine($"Local: {local}: {existsString(Directory.Exists(local))}");
                        Console.WriteLine($"Roaming: {roaming}: {existsString(Directory.Exists(roaming))}");
                        Console.WriteLine();
                        continue;

                    case "others":

                        Console.WriteLine("Application.CommonAppDataPath: " + System.Windows.Forms.Application.CommonAppDataPath);
                        Console.WriteLine("Application.LocalUserAppDataPath: " + System.Windows.Forms.Application.LocalUserAppDataPath);
                        Console.WriteLine("Environment.SpecialFolder.ApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                        Console.WriteLine("Environment.SpecialFolder.LocalApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                        Console.WriteLine("Environment.SpecialFolder.CommonApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                        continue;

                    case "open":

                        var original = WapConfigurationManagerIntegration.GetExeConfigurationUrlRoot(ConfigurationUserLevel.PerUserRoamingAndLocal);

                        Console.WriteLine($"original: {original}");

                        Process.Start("EXPLORER.EXE", $@"/select,""{original}""");

                        var redirected = WapConfigurationManagerIntegration.GetRedirectedExeConfigurationUrlRoot(ConfigurationUserLevel.PerUserRoamingAndLocal);

                        if (redirected != null)
                        {
                            Console.WriteLine($"redirected: {redirected}");

                            Process.Start("EXPLORER.EXE", $@"/select,""{redirected}""");
                        }

                        continue;

                    case "i1":

                        AppSettings1.Default.Value++;
                        AppSettings1.Default.Save();

                        continue;

                    case "il1":

                        LibSettings1.Default.Value++;
                        LibSettings1.Default.Save();

                        continue;

                    case "u1":

                        AppSettings1.Default.Upgrade();
                        AppSettings1.Default.Save();

                        continue;

                    case "ul1":

                        LibSettings1.Default.Upgrade();
                        LibSettings1.Default.Save();

                        continue;

                    case "r1":

                        AppSettings1.Default.Reload();

                        continue;

                    case "rl1":

                        LibSettings1.Default.Reload();

                        continue;

                    default:

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid command.");
                        Console.ResetColor();

                        continue;
                }
            }
        }
    }
}
