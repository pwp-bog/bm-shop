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
using Windows.UI.Xaml.Media.Imaging;
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
            FillData();
        }

        public Dictionary<string, string> AdditionalInfoButtonsText = new Dictionary<string, string>();
        public void FillData()
        {
            NameMaterial.Text = CatalogPage.CurrentMateriall.name;
            CostMaterial.Text = CatalogPage.CurrentMateriall.cost.ToString() + " руб.";
            DescriptionText.Text = CatalogPage.CurrentMateriall.description;
            
            BitmapImage img = new BitmapImage();
            img.UriSource = new Uri(CatalogPage.CurrentMateriall.photo);
            ImageMaterial.Source = img;
            FooterText.Text = CatalogPage.CurrentMateriall.advantages;

            AdditionalInfoButtonsText.Add("Преимущества", CatalogPage.CurrentMateriall.advantages);
            AdditionalInfoButtonsText.Add("Характеристики", CatalogPage.CurrentMateriall.characteristics);
            AdditionalInfoButtonsText.Add("Способ применения", CatalogPage.CurrentMateriall.modeOfApplication);
            AdditionalInfoButtonsText.Add("Хранение", CatalogPage.CurrentMateriall.storage);
            AdditionalInfoButtonsText.Add("Отзывы", "Тут пока ни чё нет");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as Button;

            foreach (var child in ButtonStackPanel.Children)
            {
                if (child is Button button)
                {
                    // Измените цвет границы для нажатой кнопки и всех остальных
                    if (button == clickedButton)
                    {
                        button.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0)); // Цвет для нажатой кнопки
                        FooterText.Text = AdditionalInfoButtonsText[button.Content.ToString()];
                    }
                    else
                    {
                        button.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 233, 239, 253)); // Цвет для остальных кнопок
                    }
                }
            }
        }
    }
}
