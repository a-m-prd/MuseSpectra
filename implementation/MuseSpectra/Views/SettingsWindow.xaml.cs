using Microsoft.Win32; // Import the Microsoft.Win32 namespace for registry operations
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;


namespace MuseSpectra.Views {
    public partial class SettingsWindow : Window {
        // Constants for the startup registry key and application name
        private const string StartupRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "MuseSpectra";

        public SettingsWindow() {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings() {
            // Open the startup registry key in read-only mode
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, false)) {
                if (key != null) {
                    // Set the checkbox state based on whether the app is set to start with Windows
                    StartWithWindowsCheckBox.IsChecked = key.GetValue(AppName) != null;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            // Open the startup registry key in write mode
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true)) {
                if (StartWithWindowsCheckBox.IsChecked == true) {
                    // Set the app to start with Windows by adding it to the registry
                    key.SetValue(AppName, $"\"{System.Reflection.Assembly.GetExecutingAssembly().Location}\"");
                }
                else {
                    // Remove the app from the startup registry if the checkbox is unchecked
                    key.DeleteValue(AppName, false);
                }
            }

            MessageBox.Show("Settings saved. Please restart the application for changes to take effect.");
            this.Close();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e) {
            // Construct the file path for the user guide
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user-guide.txt");

            if (File.Exists(filePath)) {
                // Open the user guide using the default application for .txt files
                Process.Start(new ProcessStartInfo {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            else {
                MessageBox.Show("User guide not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}