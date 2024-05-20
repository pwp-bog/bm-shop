using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace bm_shop
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение других настроек...

            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedTheme = selectedItem.Tag.ToString();
                ApplicationData.Current.LocalSettings.Values["AppTheme"] = selectedTheme;
            }

            var messageDialog = new MessageDialog("Изменения сохранены.", "Успешно");
            messageDialog.ShowAsync();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedTheme = selectedItem.Tag.ToString();
                ElementTheme theme = ElementTheme.Default;

                switch (selectedTheme)
                {
                    case "Light":
                        theme = ElementTheme.Light;
                        break;
                    case "Dark":
                        theme = ElementTheme.Dark;
                        break;
                    default:
                        theme = ElementTheme.Default;
                        break;
                }

                ApplyThemeToFrame(theme);
            }
        }

        private void ApplyThemeToFrame(ElementTheme theme)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = theme;
            }
        }

    }
}
