using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();

            // Восстановление других настроек...

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("AppTheme", out object theme))
            {
                switch (theme.ToString())
                {
                    case "Light":
                        // Установить тему для каждой отдельной страницы
                        this.RequestedTheme = ElementTheme.Light;
                        break;
                    case "Dark":
                        // Установить тему для каждой отдельной страницы
                        this.RequestedTheme = ElementTheme.Dark;
                        break;
                    default:
                        // По умолчанию установить светлую тему
                        this.RequestedTheme = ElementTheme.Light;
                        break;
                }
            }


            // Ваш код запуска приложения...
        }


        //Обработка кнопки SignIn
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignInPage));
        }

        //Обработчик кнопки SignUp
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUpPage));
        }
    }
}
