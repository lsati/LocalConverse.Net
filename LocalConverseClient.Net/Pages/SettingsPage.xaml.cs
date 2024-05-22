using LocalConverseClient.Net.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace LocalConverseClient.Net
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private LocalConverseSettings settings;

        public SettingsPage()
        {
            InitializeComponent();

            this.settings = LocalConverseSettingsManager.GetInstance();

            LoadUIControlsWithSettings();
        }

        private void LoadUIControlsWithSettings()
        {
            // models directory
            ModelsDirectoryTb.Text = settings.ModelDirectory;

            AvailableModels.Items.Clear();
            if (this.settings.LoadedModels.Count > 0)
            {
                foreach (var model in this.settings.LoadedModels)
                {
                    AvailableModels.Items.Add(model);
                }
                AvailableModels.Text = this.settings.LastSelectedModelName;
            }
            else
            {
                EmptyModelLabel.Content = "No Models found in label directory";
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string selectedModel = AvailableModels.Text;

            if (String.IsNullOrWhiteSpace(ModelsDirectoryTb.Text) ||
                String.IsNullOrWhiteSpace(AvailableModels.Text) ||
                    !Directory.Exists(ModelsDirectoryTb.Text) ||
                    !File.Exists(selectedModel))
            {
                MessageBox.Show("Invalid settings. Please fix", "Settings error");
                return;
            }

            this.settings.ModelDirectory = ModelsDirectoryTb.Text;
            this.settings.LastSelectedModelName = AvailableModels.Text;
            LocalConverseSettingsManager.SaveInstance();
        }

        private void ModelDirectoryLink_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ModelsDirectoryTb.Text))
            {
                MessageBox.Show("Directory does not exist. Please fix");
                return;
            }

            Process.Start("Explorer.exe", ModelsDirectoryTb.Text);
        }
    }
}
