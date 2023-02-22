using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Serevo.WapToolkit;
using AppSettings1 = TestConsoleApp.Properties.AppSettings1;
using AppSettings2 = TestConsoleApp.Properties.AppSettings2;
using LibSettings1 = TestSettingsLib.Properties.LibSettings1;
using LibSettings2 = TestSettingsLib.Properties.LibSettings2;

namespace TestConsoleApp
{
    static class App
    {
        static void Main()
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().FullName);
            Console.WriteLine(typeof(LibSettings1).Assembly.GetName().FullName);

            while (true)
            {
                Console.WriteLine();
                Console.Write("Input command > ");

                switch (Console.ReadLine().ToLower())
                {
                    case "exists":
                    case "e":
                        {
                            var userLevel = ChooseUserLevel();

                            if (userLevel != ConfigurationUserLevel.None)
                            {
                                var path = WapConfigurationManagerHelper.GetExeConfigurationUrlRoot(userLevel);

                                Console.WriteLine($"({(Directory.Exists(path) ? "Exists" : "Not Exists")}) {path}");
                            }

                            continue;
                        }

                    case "open":
                    case "o":
                        {
                            var userLevel = ChooseUserLevel();

                            var original = WapConfigurationManagerHelper.GetExeConfigurationUrlRoot(userLevel);

                            Console.WriteLine($"original: {original}");

                            Process.Start("EXPLORER.EXE", $@"/select,""{original}""");

                            var redirected = WapConfigurationManagerHelper.GetExeConfigurationUrlRootRedirected(userLevel);

                            if (redirected != null)
                            {
                                Console.WriteLine($"redirected: {redirected}");

                                Process.Start("EXPLORER.EXE", $@"/select,""{redirected}""");
                            }

                            continue;
                        }

                    case "migrate":
                        {
                            var userLevel = ChooseUserLevel();

                            if (userLevel != ConfigurationUserLevel.None)
                            {
                                WapConfigurationManagerIntegration.MigrateExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                            }

                            continue;
                        }

                    case "value":
                    case "val":
                    case "v":

                        ChooseSettings().Cast<dynamic>().ToList().ForEach(o => Console.WriteLine($"{o.GetType().Name}: {o.Value}"));
                        continue;

                    case "increment":
                    case "inc":
                    case "i":

                        ChooseSettings().Cast<dynamic>().ToList().ForEach(o => o.Value++);
                        continue;

                    case "save":
                    case "s":

                        ChooseSettings().ToList().ForEach(o => o.Save());
                        continue;

                    case "upgrade":

                        ChooseSettings().ToList().ForEach(o => o.Upgrade());
                        continue;

                    case "reload":

                        ChooseSettings().ToList().ForEach(o => o.Reload());
                        continue;

                    case "others":

                        Console.WriteLine("Application.CommonAppDataPath: " + System.Windows.Forms.Application.CommonAppDataPath);
                        Console.WriteLine("Application.LocalUserAppDataPath: " + System.Windows.Forms.Application.LocalUserAppDataPath);
                        Console.WriteLine("Environment.SpecialFolder.ApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                        Console.WriteLine("Environment.SpecialFolder.LocalApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                        Console.WriteLine("Environment.SpecialFolder.CommonApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
                        continue;

                    default:

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid command.");
                        Console.ResetColor();
                        continue;
                }
            }
        }

        static ApplicationSettingsBase[] ChooseSettings()
        {
            Console.Write("Choose Container (A1 / L1). If empty then all. If other keys then to cancel.");

            switch (Console.ReadLine().ToUpper())
            {
                case "": return new ApplicationSettingsBase[] { AppSettings1.Default, AppSettings2.Default, LibSettings1.Default, LibSettings2.Default };
                case "A1": return new[] { AppSettings1.Default };
                case "A2": return new[] { AppSettings2.Default };
                case "L1": return new[] { LibSettings1.Default };
                case "L2": return new[] { LibSettings2.Default };
                default: return Array.Empty<ApplicationSettingsBase>();
            }
        }

        static ConfigurationUserLevel ChooseUserLevel()
        {
            Console.Write("Choose UserLevel (L:Local / R:Roaming). If empty then L. If other keys then to cancel.");

            switch (Console.ReadLine().ToUpper())
            {
                case "": return ConfigurationUserLevel.PerUserRoamingAndLocal;
                case "L": return ConfigurationUserLevel.PerUserRoamingAndLocal;
                case "R": return ConfigurationUserLevel.PerUserRoaming;
                default: return  ConfigurationUserLevel.None;
            }
        }
    }
}
