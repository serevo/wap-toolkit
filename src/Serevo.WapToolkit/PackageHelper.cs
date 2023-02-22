using System;
using Windows.ApplicationModel;

namespace Serevo.WapToolkit
{
    static class PackageHelper
    {
        public static bool HasPackage { get; }

        public static Package Package { get; }

        static PackageHelper()
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
    }
}
