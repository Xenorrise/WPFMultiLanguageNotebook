using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;
using System.Threading;
using System.ComponentModel;

namespace WPFMultilanguageNotebook
{
    public partial class MainWindow : Window
    {
        private string currentFilePath = null;
        private string lastSavedText = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Clear();
            currentFilePath = null;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    MainTextBox.Text = File.ReadAllText(openDialog.FileName);
                    currentFilePath = openDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    currentFilePath = saveDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                File.WriteAllText(currentFilePath, MainTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LanguageMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string lang)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                Properties.Resources.Culture = new CultureInfo(lang);

                string content = MainTextBox.Text;

                var newWindow = new MainWindow();
                newWindow.currentFilePath = this.currentFilePath;
                newWindow.lastSavedText = this.lastSavedText;
                newWindow.MainTextBox.Text = content;

                Application.Current.MainWindow = newWindow;
                this.Close();
                newWindow.Show();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        NewFile_Click(sender, e);
                        break;
                    case Key.O:
                        OpenFile_Click(sender, e);
                        break;
                    case Key.S:
                        SaveFile_Click(sender, e);
                        break;
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MainTextBox.Text != lastSavedText)
            {
                var result = MessageBox.Show(Properties.Resources.ConfirmExit,
                    Properties.Resources.UnsavedChanges,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
