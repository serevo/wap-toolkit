using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    /// <summary>
    /// If the application is packaged, act as WapDataContainerSettingsProvider, otherwise act as LocalFileSettingsProvider.
    /// <para>Note: <see cref="ApplicationScopedSettingAttribute"/> is not suppoered in either case.. Please Divide the value to other Settings class (No specify <see cref="SettingsProviderAttribute"/>).</para> 
    /// </summary>
    /// <code>
    /// [SettingsProvider(typeof(<see cref="WapDataContainerOrLocalFileSettingsProvider"/>))]
    /// partial class Settings { }
    /// </code>
    public sealed class WapDataContainerOrLocalFileSettingsProvider : LocalFileSettingsProvider, IApplicationSettingsProvider
    {
        /// <summary>
        /// Please see <see cref="ProviderBase.Initialize"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
            => base.Initialize(nameof(WapDataContainerOrLocalFileSettingsProvider), config);

        /// <summary>
        /// Please see <see cref="SettingsProvider.GetPropertyValues"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            if (PackageHelper.HasPackage)
            {
                return WapDataContainerSettingsProvider.GetPropertyValuesInner(context, properties);
            }
            else
            {
                return base.GetPropertyValues(context, properties);
            }
        }

        /// <summary>
        /// Please see  <see cref="SettingsProvider.SetPropertyValues"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="values"></param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            if (PackageHelper.HasPackage)
            {
                WapDataContainerSettingsProvider.SetPropertyValuesInner(context, values);
            }
            else
            {
                base.SetPropertyValues(context, values);
            }
        }
    }
}
