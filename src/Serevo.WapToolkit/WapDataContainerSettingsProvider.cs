using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.Storage;

namespace Serevo.WapToolkit
{
    /// <summary>
    /// Provide the <see cref="SettingsProvider"/> to read and write from/to <see cref="ApplicationData"/> (<see cref="ApplicationDataContainer"/>).
    /// <para>Note: <see cref="IApplicationSettingsProvider"/> is not inherited. So the some fanctions (<see cref=" ApplicationSettingsBase.Reset"/>, <see cref="ApplicationSettingsBase.Upgrade"/>, and more) are not work.</para> 
    /// <para>Note: <see cref="ApplicationScopedSettingAttribute"/> is not suppoered. Please Divide the value to other Settings class (No specify <see cref="SettingsProviderAttribute"/> to use <see cref="LocalFileSettingsProvider"/>).</para> 
    /// </summary>
    /// <code>
    /// [SettingsProvider(typeof(<see cref="SettingsProvider"/>))]
    /// partial class Settings { }
    /// </code>
    public sealed class WapDataContainerSettingsProvider : SettingsProvider
    {
        // https://docs.microsoft.com/en-us//dotnet/framework/winforms/advanced/application-settings-architecture ではなく、
        // https://dobon.net/vb/dotnet/programing/registrysettingsprovider.html を参考にした。
        // これで実行ファイルの AssemblyProductAttribute の値が渡されるが用途不明。
        // また、ライブラリ (.dll) 内の設定でも実行ファイルの値と同じになる。

        /// <summary>
        /// Please see <see cref="SettingsProvider.ApplicationName"/>.
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Please see <see cref="ProviderBase.Initialize"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
            => base.Initialize(nameof(WapDataContainerSettingsProvider), config);

        string CreateKey(SettingsContext context)
        {
            DumpContext(context);

            return  string.Join("."
                // 設定クラスのフルネーム
                , context["GroupName"]
                // ??
                , context["SettingsKey"]
                );
        }

        [Conditional("DEBUG")]
        void DumpContext(SettingsContext context)
        {
            foreach(var key in context.Keys)
            {
                Debug.WriteLine($"{key}: {context[key]}");
            }
        }

        bool IsUserSetting(SettingsProperty setting)
        {
            var user = setting.Attributes.Contains(typeof(UserScopedSettingAttribute));

            var app = setting.Attributes.Contains(typeof(ApplicationScopedSettingAttribute));

            if (user && app)
            {
                throw new ConfigurationErrorsException($"Setting '{setting.Name}' has both ApplicationScopedSettingAttribute and UserScopedSettingAttribute.");
            }
            if (!user && !app)
            {
                throw new ConfigurationErrorsException($"Setting '{setting.Name}' does not spedified either ApplicationScopedSettingAttribute or UserScopedSettingAttribute.");
            }
            return user;
        }

        /// <summary>
        /// Please see <see cref="SettingsProvider.GetPropertyValues"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            var results = new SettingsPropertyValueCollection();

            var key = CreateKey(context);

            var roamingContainer = ApplicationData.Current.RoamingSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);

            var localContainer = ApplicationData.Current.LocalSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
            
            foreach (SettingsProperty prop in collection)
            {
                var propValue = new SettingsPropertyValue(prop);

                if (IsUserSetting(prop))
                {
                    var container = propValue.Property.Attributes.Values
                        .Cast<Attribute>()
                        .OfType<SettingsManageabilityAttribute>()
                        .SingleOrDefault()?
                        .Manageability == SettingsManageability.Roaming ? roamingContainer : localContainer;

                    var value = container.Values[prop.Name];

                    if (value is null)
                    {
                        var subContainer = container.CreateContainer(propValue.Name, ApplicationDataCreateDisposition.Always);

                        propValue.SerializedValue = subContainer.Values.Count == 0 ? null : string.Concat(
                            from o in subContainer.Values
                            let page = int.Parse(o.Key)
                            orderby page
                            select o.Value
                            );
                    }
                    else
                    {
                        propValue.SerializedValue = value;
                    }
                }
                else
                {
                    throw new NotSupportedException($"{nameof(ApplicationScopedSettingAttribute)} is not supported by {nameof(WapDataContainerSettingsProvider)}. Divide the value to other Settings class.");
                }


                results.Add(propValue);
            }

            return results;
        }

        /// <summary>
        /// Please see  <see cref="SettingsProvider.SetPropertyValues"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection"></param>
        /// <exception cref="NotSupportedException"></exception>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            var key = CreateKey(context);

            var roamingContainer = ApplicationData.Current.RoamingSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);

            var localContainer = ApplicationData.Current.LocalSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);

            foreach (SettingsPropertyValue propValue in collection)
            {
                if (IsUserSetting(propValue.Property))
                {
                    var container = propValue.Property.Attributes.Values
                        .Cast<Attribute>()
                        .OfType<SettingsManageabilityAttribute>()
                        .SingleOrDefault()?
                        .Manageability == SettingsManageability.Roaming ? roamingContainer : localContainer;

                    if (propValue.SerializedValue is string s)
                    {
                        var bytes = Encoding.UTF8.GetBytes(s);

                        var maxLength = 4000;

                        if (s.Length <= maxLength)
                        {
                            container.DeleteContainer(propValue.Name);

                            container.Values[propValue.Name] = s;
                        }
                        else
                        {
                            container.Values[propValue.Name] = null;

                            var subContainers = container.CreateContainer(propValue.Name, ApplicationDataCreateDisposition.Always);

                            var docPropNameValuePairs =
                                from page in Enumerable.Range(0, (s.Length - 1) / maxLength + 1)
                                let start = page * maxLength
                                let value = new string(s.Skip(start).Take(maxLength).ToArray())
                                select new { name = page.ToString(), value }
                                ;
                            foreach (var pair in docPropNameValuePairs)
                            {
                                subContainers.Values[pair.name] = pair.value;
                            }
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException($"{nameof(ApplicationScopedSettingAttribute)} is not supported by {nameof(WapDataContainerSettingsProvider)}. Divide the value to other Settings class.");
                }
            }
        }
    }
}
