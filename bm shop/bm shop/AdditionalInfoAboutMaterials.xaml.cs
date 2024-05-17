using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AdditionalInfoAboutMaterials : Page
    {
        public AdditionalInfoAboutMaterials()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var color = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
            var color1 = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 233, 239, 253));
            testBtn.BorderBrush = color;
            testBtn1.BorderBrush = color1;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            var color = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
            var color1 = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 233, 239, 253));
            testBtn1.BorderBrush = color;
            testBtn.BorderBrush = color1;
        }
    }
}
