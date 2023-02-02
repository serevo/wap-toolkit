using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using DemoWinFormsApp.Properties;

namespace DemoWinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = $@"
{"ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)"}: {ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath}
{"ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming)"}: {ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming).FilePath}
{"Application.CommonAppDataPath"}: {Application.CommonAppDataPath}
{"Application.UserAppDataPath"}: {Application.UserAppDataPath}
{"Application.LocalUserAppDataPath"}: {Application.LocalUserAppDataPath}
{"Environment.SpecialFolder.CommonApplicationData"}: {Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}
{"Environment.SpecialFolder.ApplicationData"}: {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}
{"Environment.SpecialFolder.LocalApplicationData)"}: {Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}
";

            textBox3.Text = AppInfoHelper.HasPackage ? Windows.Storage.ApplicationData.Current.LocalFolder.Path : null;
            textBox4.Text = AppInfoHelper.HasPackage ? Windows.Storage.ApplicationData.Current.RoamingFolder.Path : null;
            textBox5.Text = AppInfoHelper.HasPackage ? Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path : null;

            button3.Enabled = AppInfoHelper.HasPackage;
            button4.Enabled = AppInfoHelper.HasPackage;
            button5.Enabled = AppInfoHelper.HasPackage;

            checkBox1.Checked = Settings.Default.BooleanValue;
            checkBox2.Checked = Settings2.Default.BooleanValue2;

            numericUpDown1.Value = DemoSettingsClassLib.Properties.Settings.Default.Int32Value;
            numericUpDown2.Value = DemoSettingsClassLib.Properties.Settings2.Default.Int32Value2;
        }

        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BooleanValue = checkBox1.Checked;
            Settings.Default.Save();
        }

        void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings2.Default.BooleanValue2 = checkBox2.Checked;
            Settings2.Default.Save();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Settings.Default.Upgrade();
            checkBox1.Checked = Settings.Default.BooleanValue;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Settings2.Default.Upgrade();
            checkBox2.Checked = Settings2.Default.BooleanValue2;
        }

        void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(textBox3.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(textBox4.Text);
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }

        void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(textBox5.Text);
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DemoSettingsClassLib.Properties.Settings.Default.Int32Value = (int)numericUpDown1.Value;
            DemoSettingsClassLib.Properties.Settings.Default.Save();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            DemoSettingsClassLib.Properties.Settings2.Default.Int32Value2 = (int)numericUpDown2.Value;
            DemoSettingsClassLib.Properties.Settings2.Default.Save();
        }
    }
}
