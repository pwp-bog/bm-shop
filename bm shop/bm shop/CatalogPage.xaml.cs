using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Xml.Linq;
using Windows.UI.Xaml.Media.Imaging;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace bm_shop
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            this.InitializeComponent();
            FillData();
        }


        public void FillData()
        {
            //CategoryNameTextBlock.Text = MainPage.CategoryName;

            if(MainPage.CategoryName == "purchases")
            {
                AdditionalInfoAboutMaterials.isBasket = true;
                FillCatalog($"SELECT * FROM `materials` m JOIN `purchases` p where m.id = p.materialId and p.userId = {SignInPage.CurrentUser.id};");
            }
            else if(MainPage.CategoryName == "find")
            {
                AdditionalInfoAboutMaterials.isBasket = false;
                FillCatalog($"SELECT * FROM `materials` where `category` = '{MainPage.CategoryName}' and quantity > 0;");
            }
            else
            {
                AdditionalInfoAboutMaterials.isBasket = false;
                FillCatalog($"SELECT * FROM `materials` where name LIKE '%{MainPage.FindInfo}%' and quantity > 0;");
            }
        }

        //Лист товаров
        public List<Materials> MaterialsList = new List<Materials>();
        //Текущий товар
        public static Materials CurrentMateriall;

        public void FillCatalog(string SqlCommand)
        {
            DB db = new DB();
            db.openConnection();

            CatalogGrid.Children.Clear();
            MaterialsList.Clear();
            CurrentMateriall = null;

            List<string> Photo = new List<string>();
            List<int> Id = new List<int>();
            List<string> TitleNote = new List<string>();

            DataTable table = new DataTable();

            using (MySqlConnection connection = db.getConnection())
            {
                MySqlCommand command = new MySqlCommand(SqlCommand, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            db.closeConnection();


            foreach (DataRow row in table.Rows)
            {
                Materials buf = new Materials(int.Parse(row[0].ToString()), row[1].ToString(), row[2].ToString(), row[3].ToString(),
                    row[4].ToString(), double.Parse(row[5].ToString()), row[6].ToString(), row[7].ToString(), row[8].ToString(),
                    row[9].ToString(), int.Parse(row[10].ToString()), row[11].ToString());
                MaterialsList.Add(buf);

                //Photo.Add(buf.);
                Id.Add(buf.id);
                TitleNote.Add(buf.name);
            }

            // Итерация по массиву URL изображений
            int j = 0;
            int k = 0;
            List<string> AdeddList = new List<string>();
            for (int i = 0; i < TitleNote.Count; i++)
            {
                // Определение столбцов и строк
                CatalogGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                //if(AdditionalInfoAboutMaterials.isBasket)
                //{
                //    addTextToGrid(k, j, MaterialsList[i], i);
                //}
                //else
                //{
                //    // Проверка наличия значения в списке
                //    if (!AdeddList.Contains(TitleNote[i]))
                //    {
                //        addTextToGrid(k, j, MaterialsList[i], i);
                //        AdeddList.Add(TitleNote[i]);
                //    }
                //}
                // Проверка наличия значения в списке
                if (!AdeddList.Contains(TitleNote[i]))
                {
                    addTextToGrid(k, j, MaterialsList[i], i);
                    AdeddList.Add(TitleNote[i]);

                    j++;
                    if (j >= 4)
                    {
                        j = 0;
                        k++;
                    }
                }
            }
        }

        public void addTextToGrid(int row, int column, Materials currentMaterial, int i)
        {
            // Создание нового объекта BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(currentMaterial.photo);

            // Создание нового объекта Image
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            image.Source = bitmapImage;
            image.Name = currentMaterial.name;
            image.Tag = i;
            image.Tapped += new Windows.UI.Xaml.Input.TappedEventHandler(getNamePicture);
            image.Stretch = Stretch.Uniform;

            // Задание визуальных параметров Image
            image.Width = 450;
            image.Height = 450;

            // Задание цвета для обводки
            //Windows.UI.Color color = Windows.UI.Color.FromArgb(0xFF, 0x60, 0x77, 0x99);
            Windows.UI.Color color = Windows.UI.Color.FromArgb(255, 129, 129, 129);
            Windows.UI.Color color1 = Windows.UI.Color.FromArgb(255, 255, 255, 255);

            // Создание нового объекта Border
            Border border = new Border();
            border.Name = currentMaterial.name;
            border.BorderBrush = new SolidColorBrush(color);
            border.BorderThickness = new Thickness(5);
            border.CornerRadius = new CornerRadius(15); // Устанавливаем радиус закругления
            border.Background = new SolidColorBrush(color1); // Устанавливаем радиус закругления

            // Задание размеров Border, соответствующих размерам изображения
            border.Width = image.Width;
            border.Height = image.Height;

            // Установка Padding для Border
            border.Margin = new Thickness(12, 0, 0, 12);

            // Задание расположения Image внутри Border
            border.Child = image;

            // Задание расположения Border в Grid
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);

            // Добавление объекта Border в Grid
            CatalogGrid.Children.Add(border); // Замените "CatalogGrid" на имя вашего Grid элемента
        }


        public void getNamePicture(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Image tappedImage = sender as Windows.UI.Xaml.Controls.Image;

            if (tappedImage.Tag != null)
            {
                if(MainPage.CategoryName == "purchases")
                {
                    int index = (int)tappedImage.Tag;
                    CatalogPage.CurrentMateriall = MaterialsList[index];
                    AdditionalInfoAboutMaterials.isBasket = true;
                    Frame.Navigate(typeof(AdditionalInfoAboutMaterials));
                }
                else
                {
                    int index = (int)tappedImage.Tag;
                    CatalogPage.CurrentMateriall = MaterialsList[index];
                    AdditionalInfoAboutMaterials.isBasket = false;
                    Frame.Navigate(typeof(AdditionalInfoAboutMaterials));
                }
            }
        }

        //Обработчик кнопки назад
        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Frame.Navigate(typeof(CategoryPage));
        }

        //Обработчик кнопки корзина
        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {

        }

        //Обработчик кнопки выйти из уч записи
        private void Image_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            var msg = new MessageDialog($"Вы действительно хотите выйти?", "bm shop");

            // Создание команды для кнопки "Да"
            UICommand yesCommand = new UICommand("Да");
            yesCommand.Invoked = (command) =>
            {
                Frame.Navigate(typeof(SignInPage));
            };

            // Создание команды для кнопки "Нет"
            UICommand noCommand = new UICommand("Нет");
            noCommand.Invoked = (command) =>
            {
            };

            // Добавление команд к диалоговому окну
            msg.Commands.Add(yesCommand);
            msg.Commands.Add(noCommand);

            // Установка команды по умолчанию (в данном случае "Нет")
            msg.DefaultCommandIndex = 1;

            // Отображение диалогового окна
            var result = msg.ShowAsync();
        }
    }
}
