using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
    public sealed partial class Basket : Page
    {
        public Basket()
        {
            this.InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            FillCatalog($"SELECT * FROM `materials` m JOIN `basket` p where m.id = p.materialId and p.userId = {SignInPage.CurrentUser.id};");
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
            List<int> Quantity = new List<int>();
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

                Quantity.Add(int.Parse(row[16].ToString()));

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

                // Проверка наличия значения в списке
                if (!AdeddList.Contains(TitleNote[i]))
                {
                    addTextToGrid(k, j, MaterialsList[i], i, Quantity[i]);
                    AdeddList.Add(TitleNote[i]);

                    j++;
                    if (j >= 1)
                    {
                        j = 0;
                        k++;
                    }
                }

                if (i >= TitleNote.Count-1)
                {
                    AllCostText.Text = AllCost.ToString() + " руб.";

                    QuantityMaterial.Text = GetCorrectWordFormForProduct(QuantityMaterialInBasket);
                }
                    
            }
        }

        public double AllCost = 0;
        public int QuantityMaterialInBasket = 0;
        
        public string GetCorrectWordFormForProduct(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Число должно быть неотрицательным.");
            }

            int lastDigit = count % 10; // последняя цифра числа
            int lastTwoDigits = count % 100; // последние две цифры числа

            // Обработка исключений для чисел, оканчивающихся на 11-14
            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
            {
                return $"{count} товаров";
            }

            // Определение правильной формы слова "товар" в зависимости от последней цифры числа
            switch (lastDigit)
            {
                case 1:
                    return $"{count} товар";
                case 2:
                case 3:
                case 4:
                    return $"{count} товара";
                default:
                    return $"{count} товаров";
            }
        }

        public void addTextToGrid(int row, int column, Materials currentMaterial, int i, int quantity)
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
            image.Width = 135;
            image.Height = 135;

            // Задание цвета для обводки
            Windows.UI.Color color = Windows.UI.Color.FromArgb(255, 129, 129, 129);
            Windows.UI.Color color1 = Windows.UI.Color.FromArgb(255, 255, 255, 255);
            Windows.UI.Color BlackColor = Windows.UI.Color.FromArgb(255, 0, 0, 0);
            Windows.UI.Color fb = Windows.UI.Color.FromArgb(255, 251, 251, 251);
            Windows.UI.Color f1 = Windows.UI.Color.FromArgb(255, 241, 241, 241);

            // Создание нового объекта Border
            Border border = new Border();
            border.Name = currentMaterial.name;
            border.Background = new SolidColorBrush(color1);
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.Margin = new Thickness(10, 5, 10, 15);

            // Создание заголовка товара
            TextBlock MaterialName = new TextBlock();
            MaterialName.Text = currentMaterial.name;
            MaterialName.FontSize = 28;
            MaterialName.Foreground = new SolidColorBrush(BlackColor);
            MaterialName.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialName.VerticalAlignment = VerticalAlignment.Top;
            MaterialName.Margin = new Thickness(10, 0, 0, 0);

            // Создание цвета товара
            TextBlock MaterialColor = new TextBlock();
            MaterialColor.Text = currentMaterial.color;
            MaterialColor.FontSize = 18;
            MaterialColor.Foreground = new SolidColorBrush(BlackColor);
            MaterialColor.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialColor.VerticalAlignment = VerticalAlignment.Top;
            MaterialColor.Margin = new Thickness(10, 0, 0, 0);

            // Создание цены товара
            TextBlock MaterialCost = new TextBlock();
            //MaterialCost.Text = currentMaterial.cost.ToString() + " руб.";
            MaterialCost.Text = currentMaterial.cost.ToString();
            MaterialCost.FontSize = 18;
            MaterialCost.Name = "MaterialCost";
            MaterialCost.Foreground = new SolidColorBrush(BlackColor);
            MaterialCost.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialCost.VerticalAlignment = VerticalAlignment.Top;
            MaterialCost.Margin = new Thickness(10, 0, 0, 0);

            // Создание объёма товара
            TextBlock MaterialWeight = new TextBlock();
            MaterialWeight.Text = currentMaterial.weigth;
            MaterialWeight.FontSize = 18;
            MaterialWeight.Foreground = new SolidColorBrush(BlackColor);
            MaterialWeight.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialWeight.VerticalAlignment = VerticalAlignment.Top;
            MaterialWeight.Margin = new Thickness(10, 0, 0, 0);

            // Создание заголовка товара
            TextBlock MaterialBigCost = new TextBlock();
            //MaterialBigCost.Text = currentMaterial.cost.ToString() + " руб.";
            MaterialBigCost.Text = (currentMaterial.cost * quantity).ToString();
            MaterialBigCost.FontSize = 28;
            MaterialBigCost.Name = "MaterialBigCost";
            MaterialBigCost.Foreground = new SolidColorBrush(BlackColor);
            MaterialBigCost.HorizontalAlignment = HorizontalAlignment.Right;
            MaterialBigCost.VerticalAlignment = VerticalAlignment.Top;


            // Создание кнопки выбора количества товаров
            Grid BtnGrid = new Grid();
            BtnGrid.Width = 180;
            BtnGrid.Height = 35;
            BtnGrid.HorizontalAlignment = HorizontalAlignment.Right;
            BtnGrid.VerticalAlignment = VerticalAlignment.Top;
            BtnGrid.Margin = new Thickness(10, 0, 0, 0);

            BtnGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            BtnGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            BtnGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            BtnGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // Добавлена колонка для CheckBox

            Button MinusBtn = new Button();
            MinusBtn.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            MinusBtn.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            MinusBtn.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            MinusBtn.BorderThickness = new Thickness(0);
            MinusBtn.Width = 35;
            MinusBtn.Height = 35;
            MinusBtn.FontSize = 20;
            MinusBtn.Content = "-";
            MinusBtn.FontSize = 20;
            

            TextBox QuantityMaterial = new TextBox();
            QuantityMaterial.FontSize = 20;
            QuantityMaterial.Height = 35;
            QuantityMaterial.Width = 85;
            QuantityMaterial.Text = quantity.ToString(); //Количество товара в корзине
            QuantityMaterial.TextAlignment = TextAlignment.Center;
            QuantityMaterial.HorizontalAlignment = HorizontalAlignment.Center;
            QuantityMaterial.VerticalAlignment = VerticalAlignment.Center;
            QuantityMaterial.Margin = new Thickness(0);
            QuantityMaterial.TextChanged += (sender, e) => TextBoxChanged(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost);

            MinusBtn.Tapped += (sender, e) => MinusBtnClick(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost);

            Button DeleteBtn = new Button();
            DeleteBtn.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            DeleteBtn.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            DeleteBtn.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            DeleteBtn.BorderThickness = new Thickness(0);
            DeleteBtn.Width = 35;
            DeleteBtn.Height = 35;
            FontIcon icon = new FontIcon();
            icon.Glyph = "\uE711";
            DeleteBtn.Content = icon;
            DeleteBtn.FontSize = 20;
            DeleteBtn.HorizontalAlignment = HorizontalAlignment.Right;
            DeleteBtn.VerticalAlignment = VerticalAlignment.Top;

            Button PlusBtn = new Button();
            PlusBtn.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            PlusBtn.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            PlusBtn.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            PlusBtn.BorderThickness = new Thickness(0);
            PlusBtn.Width = 35;
            PlusBtn.Height = 35;
            PlusBtn.Content = "+";
            PlusBtn.FontSize = 20;
            PlusBtn.Tapped += (sender, e) => PlusBtnClick(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost);

            // Создание CheckBox выбора товара
            CheckBox IsSelected = new CheckBox();
            IsSelected.HorizontalAlignment = HorizontalAlignment.Right;
            IsSelected.VerticalAlignment = VerticalAlignment.Top;
            IsSelected.Margin = new Thickness(5,0,0,0);
            IsSelected.IsChecked = true;
            AllCost += double.Parse(MaterialBigCost.Text);
            QuantityMaterialInBasket++;

            BtnGrid.Children.Add(MinusBtn);
            BtnGrid.Children.Add(QuantityMaterial);
            BtnGrid.Children.Add(PlusBtn);
            BtnGrid.Children.Add(IsSelected); // Добавление CheckBox в BtnGrid

            Grid.SetColumn(MinusBtn, 0);
            Grid.SetColumn(QuantityMaterial, 1);
            Grid.SetColumn(PlusBtn, 2);
            Grid.SetColumn(IsSelected, 3); // Установка CheckBox в новую колонку

            // Создание контейнера Grid
            Grid containerGrid = new Grid();
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Добавление Image и TextBlock в Grid
            containerGrid.Children.Add(image);
            containerGrid.Children.Add(MaterialName);
            containerGrid.Children.Add(MaterialColor);
            containerGrid.Children.Add(MaterialCost);
            containerGrid.Children.Add(MaterialWeight);
            containerGrid.Children.Add(MaterialBigCost);
            containerGrid.Children.Add(BtnGrid);
            containerGrid.Children.Add(DeleteBtn);

            // Установка строк и колонок для Image и TextBlock
            Grid.SetColumn(image, 0);
            Grid.SetRow(image, 0);
            Grid.SetRowSpan(image, 4);
            Grid.SetColumn(MaterialName, 1);
            Grid.SetRow(MaterialName, 0);
            Grid.SetColumn(MaterialColor, 1);
            Grid.SetRow(MaterialColor, 1);
            Grid.SetColumn(MaterialCost, 1);
            Grid.SetRow(MaterialCost, 2);
            Grid.SetColumn(MaterialWeight, 1);
            Grid.SetRow(MaterialWeight, 3);
            Grid.SetColumn(MaterialBigCost, 2);
            Grid.SetRow(MaterialBigCost, 1);
            Grid.SetRowSpan(MaterialBigCost, 1);
            Grid.SetColumn(DeleteBtn, 2);
            Grid.SetRow(DeleteBtn, 0);
            Grid.SetColumn(BtnGrid, 2);
            Grid.SetRow(BtnGrid, 2);

            // Задание расположения контейнера Grid внутри Border
            border.Child = containerGrid;

            // Задание расположения Border в Grid
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);

            // Добавление объекта Border в Grid
            CatalogGrid.Children.Add(border);
            DeleteBtn.Tapped += (sender, e) => DeleteBtnClick(sender, e, border);
        }

        private void DeleteBtnClick(object sender, TappedRoutedEventArgs e, Border border)
        {
            border.Visibility = Visibility.Collapsed;
            //Удалить товар из корзины (запрос в бд)
        }

        private void MinusBtnClick(object sender, TappedRoutedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox)
        {
            if (sender is Button button)
            {
                var parentGrid = button.Parent as Grid;
                if (parentGrid != null)
                {

                    if (quantityTextBox != null && materialCostTextBox != null && materialBigCostTextBox != null &&
                        int.TryParse(quantityTextBox.Text, out int quantity) &&
                        double.TryParse(materialCostTextBox.Text, out double materialCost))
                    {
                        if (quantity >= 2)
                        {
                            quantity--;
                            quantityTextBox.Text = quantity.ToString();
                        }
                        else if (quantity == 0)
                        {
                            //parentGrid.Visibility = Visibility.Collapsed;
                            //Удалить из корзины??TODO
                        }

                        //double newBigCost = quantity * materialCost;
                        //materialBigCostTextBox.Text = newBigCost.ToString("F2");
                    }
                }
            }
        }

        public void TextBoxChanged(object sender, TextChangedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox)
        {
            if(quantityTextBox.Text == string.Empty || quantityTextBox.Text == "0")
            {
                double newBigCost = 1 * double.Parse(materialCostTextBox.Text);
                materialBigCostTextBox.Text = newBigCost.ToString("F2");
                quantityTextBox.Text = "1";
                quantityTextBox.SelectionStart = quantityTextBox.Text.Length;    
            }
            else
            {
                double buf = double.Parse(quantityTextBox.Text);
                if (buf <= 0)
                {
                    double newBigCost = 1 * double.Parse(materialCostTextBox.Text);
                    materialBigCostTextBox.Text = newBigCost.ToString("F2");
                }
                else
                {
                    double newBigCost = buf * double.Parse(materialCostTextBox.Text);
                    materialBigCostTextBox.Text = newBigCost.ToString("F2");
                }
            }
        }

        private void PlusBtnClick(object sender, TappedRoutedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox)
        {
            if (sender is Button button)
            {
                var parentGrid = button.Parent as Grid;
                if (parentGrid != null)
                {

                    if (quantityTextBox != null && materialCostTextBox != null && materialBigCostTextBox != null &&
                        int.TryParse(quantityTextBox.Text, out int quantity) &&
                        double.TryParse(materialCostTextBox.Text, out double materialCost))
                    {
                        quantity++;
                        quantityTextBox.Text = quantity.ToString();

                        //double newBigCost = quantity * materialCost;
                        //materialBigCostTextBox.Text = newBigCost.ToString("F2");
                    }
                }
            }
        }


        public void getNamePicture(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Windows.UI.Xaml.Controls.Image tappedImage = sender as Windows.UI.Xaml.Controls.Image;

            if (tappedImage.Tag != null)
            {
                if (MainPage.CategoryName == "purchases")
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
    }
}
