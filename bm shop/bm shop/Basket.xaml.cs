using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using static System.Net.WebRequestMethods;
using Windows.UI.Xaml.Shapes;
using System.Reflection;
using Windows.Security.Authentication.Identity.Core;
using Windows.ApplicationModel.Activation;
using System.Xml.Linq;
using Windows.UI;
using System.Data.SqlClient;
using OfficeOpenXml;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;
using System.Threading.Tasks;
using OfficeOpenXml.Style;



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

        //Функция заполнения данных
        public void FillData()
        {
            AllCost = 0;
            QuantityMaterialInBasket = 0;
            FillCatalog($"SELECT * FROM `materials` m JOIN `basket` p where m.id = p.materialId and p.userId = {SignInPage.CurrentUser.id};");
        }

        //Лист товаров
        public List<Materials> MaterialsList = new List<Materials>();
        //Текущий товар
        public static Materials CurrentMateriall;

        //Функция заполнения карточек с товарами
        public void FillCatalog(string SqlCommand)
        {
            DB db = new DB();
            db.openConnection();

            CatalogGrid.Children.Clear();
            MaterialsList.Clear();
            CurrentMateriall = null;

            // Создание CheckBox программно
            CheckBox selectAllCheckBox = new CheckBox
            {
                Name = "SelectAll",
                Content = "Выбрать все товары",
                FontSize = 18,
                Margin = new Thickness(10, 10, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                IsChecked = true
            };

            selectAllCheckBox.Click += SelectAllCheckBox_Click;

            // Создание Rectangle программно
            Windows.UI.Xaml.Shapes.Rectangle redLine = new Windows.UI.Xaml.Shapes.Rectangle
            {
                VerticalAlignment = VerticalAlignment.Top,
                Height = 1, // высота линии
                Fill = new SolidColorBrush(Windows.UI.Colors.Gray)
            };

            // Определение строк в Grid
            CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); // строка для CheckBox
            CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); // строка для линии

            // Добавление элементов в Grid
            CatalogGrid.Children.Add(selectAllCheckBox);
            Grid.SetRow(selectAllCheckBox, 0);
            Grid.SetColumn(selectAllCheckBox, 0);

            CatalogGrid.Children.Add(redLine);
            Grid.SetRow(redLine, 1);
            Grid.SetColumn(redLine, 0);

            List<string> Photo = new List<string>();
            List<int> Id = new List<int>();
            List<int> Quantity = new List<int>();
            List<int> BasketId = new List<int>();
            List<string> SelectedStatusForMaterial = new List<string>();
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


                BasketId.Add(int.Parse(row[13].ToString()));
                Quantity.Add(int.Parse(row[16].ToString()));
                SelectedStatusForMaterial.Add(row[17].ToString());

                //Photo.Add(buf.);
                Id.Add(buf.id);
                TitleNote.Add(buf.name);
            }

            // Итерация по массиву URL изображений
            int j = 0;
            int k = 1;
            List<string> AdeddList = new List<string>();
            for (int i = 0; i < TitleNote.Count; i++)
            {
                // Определение столбцов и строк
                CatalogGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                // Проверка наличия значения в списке
                if (!AdeddList.Contains(TitleNote[i]))
                {
                    addTextToGrid(k, j, MaterialsList[i], i, Quantity[i], SelectedStatusForMaterial[i], BasketId[i]);
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

        private bool selectAllPreviousState = true; // Предыдущее состояние SelectAll

        //Проставление галочек выбора для всех товаров, для кнопки выбрать все товары
        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;

            // Если текущее состояние SelectAll отличается от предыдущего,
            // то обновляем галочки у всех остальных CheckBox
            if (isChecked != selectAllPreviousState)
            {
                foreach (var child in CatalogGrid.Children)
                {
                    if (child is Windows.UI.Xaml.Controls.Border border)
                    {
                        foreach (var gridChild in (border.Child as Grid).Children)
                        {
                            if (gridChild is Grid containerGrid)
                            {
                                foreach (var btnGridChild in containerGrid.Children)
                                {
                                    if (btnGridChild is CheckBox checkBox && checkBox.Name != "SelectAll")
                                    {
                                        checkBox.IsChecked = isChecked;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Обновляем предыдущее состояние SelectAll
            selectAllPreviousState = isChecked;
        }

        public static SolidColorBrush brush1 = (SolidColorBrush)Application.Current.Resources["AppBarItemPointerOverForegroundThemeBrush"];

        //Отображение плашки с товаром в список корзины
        public void addTextToGrid(int row, int column, Materials currentMaterial, int i, int quantity, string SelectedStatusForMaterial, int BasketId)
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

            // Получение ресурса ThemeResource
            var brush = (SolidColorBrush)Application.Current.Resources["SystemControlPageBackgroundListLowBrush"];
            

            var currentTheme = Application.Current.RequestedTheme;
            if (currentTheme == ApplicationTheme.Dark)
            {
                brush1 = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                brush1 = new SolidColorBrush(Windows.UI.Colors.Black);
            }


            // Создание нового объекта Border
            Windows.UI.Xaml.Controls.Border border = new Windows.UI.Xaml.Controls.Border();
            border.Name = currentMaterial.name;
            //border.Background = brush;
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.Margin = new Thickness(10, 5, 10, 15);

            // Создание заголовка товара
            TextBlock MaterialName = new TextBlock();
            MaterialName.Text = currentMaterial.name;
            MaterialName.FontSize = 28;
            MaterialName.Name = "MaterialName";
            MaterialName.Foreground = brush1;
            MaterialName.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialName.VerticalAlignment = VerticalAlignment.Top;
            MaterialName.Margin = new Thickness(10, 0, 0, 0);

            // Создание цвета товара
            TextBlock MaterialColor = new TextBlock();
            MaterialColor.Text = currentMaterial.color;
            MaterialColor.FontSize = 18;
            MaterialColor.Name = "MaterialColor";
            MaterialColor.Foreground = brush1;
            MaterialColor.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialColor.VerticalAlignment = VerticalAlignment.Top;
            MaterialColor.Margin = new Thickness(10, 0, 0, 0);

            // Создание цены товара
            TextBlock MaterialCost = new TextBlock();
            //MaterialCost.Text = currentMaterial.cost.ToString() + " руб.";
            MaterialCost.Text = currentMaterial.cost.ToString();
            MaterialCost.FontSize = 18;
            MaterialCost.Name = "MaterialCost";
            MaterialCost.Foreground = brush1;
            MaterialCost.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialCost.VerticalAlignment = VerticalAlignment.Top;
            MaterialCost.Margin = new Thickness(10, 0, 0, 0);

            // Создание объёма товара
            TextBlock MaterialWeight = new TextBlock();
            MaterialWeight.Text = currentMaterial.weigth;
            MaterialWeight.FontSize = 18;
            MaterialWeight.Name = "MaterialWeight";
            MaterialWeight.Foreground = brush1;
            MaterialWeight.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialWeight.VerticalAlignment = VerticalAlignment.Top;
            MaterialWeight.Margin = new Thickness(10, 0, 0, 0);

            // Создание заголовка товара
            TextBlock MaterialBigCost = new TextBlock();
            //MaterialBigCost.Text = currentMaterial.cost.ToString() + " руб.";
            MaterialBigCost.Text = (currentMaterial.cost * quantity).ToString();
            MaterialBigCost.FontSize = 28;
            MaterialBigCost.Name = "MaterialBigCost";
            MaterialBigCost.Foreground = brush1;
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
            MinusBtn.Foreground = brush1;
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
            QuantityMaterial.Foreground = brush1;


            Button DeleteBtn = new Button();
            DeleteBtn.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            DeleteBtn.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            DeleteBtn.Foreground = brush1;
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
            PlusBtn.Foreground = brush1;
            PlusBtn.BorderThickness = new Thickness(0);
            PlusBtn.Width = 35;
            PlusBtn.Height = 35;
            PlusBtn.Content = "+";
            PlusBtn.FontSize = 20;

            if(bool.Parse(SelectedStatusForMaterial))
            {
                AllCost += double.Parse(MaterialBigCost.Text);
                QuantityMaterialInBasket++;
            }

            // Создание CheckBox выбора товара
            CheckBox IsSelected = new CheckBox();
            IsSelected.BorderBrush = new SolidColorBrush(BlackColor);
            IsSelected.Name = "IsSelected";
            IsSelected.HorizontalAlignment = HorizontalAlignment.Right;
            IsSelected.VerticalAlignment = VerticalAlignment.Top;
            IsSelected.Margin = new Thickness(5,0,0,0);
            IsSelected.IsChecked = bool.Parse(SelectedStatusForMaterial);
            IsSelected.Checked += (sender, e) => IsSelected_Checked(sender, e, MaterialBigCost, QuantityMaterial, PlusBtn, MinusBtn, BasketId);
            IsSelected.Unchecked += (sender, e) => IsSelected_Checked(sender, e, MaterialBigCost, QuantityMaterial, PlusBtn, MinusBtn, BasketId);


            MinusBtn.Tapped += (sender, e) => MinusBtnClick(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost, IsSelected);
            PlusBtn.Tapped += (sender, e) => PlusBtnClick(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost, IsSelected, currentMaterial);

            // Привязка события TextChanged
            QuantityMaterial.TextChanged += (sender, e) => TextBoxTextChanged(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost, IsSelected, currentMaterial);

            // Привязка события LostFocus
            QuantityMaterial.LostFocus += (sender, e) => TextBoxLostFocus(sender, e, QuantityMaterial, MaterialCost, MaterialBigCost, IsSelected, currentMaterial);


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
            DeleteBtn.Tapped += (sender, e) => DeleteBtnClick(sender, e, border, currentMaterial);
        }

        //Добавление элемента в список покупкок (за счёт поставленной галочки)
        public void IsSelected_Checked(object sender, RoutedEventArgs e, TextBlock MaterialBigCost, TextBox MaterialTextBox, Button PlusBtn, Button MinusBtn, int BasketId)
        {
            // Получаем CheckBox, который вызвал событие
            CheckBox checkBox = sender as CheckBox;

            // Ваш код обработки события для конкретного CheckBox
            if (checkBox.IsChecked == true)
            {
                MaterialTextBox.IsEnabled = true;
                MinusBtn.IsEnabled = true;
                PlusBtn.IsEnabled = true;
                // Код, который нужно выполнить, если CheckBox отмечен
                //var msg = new MessageDialog($"MatBigCst = {MaterialBigCost.Text}\nAllCost = {AllCost}", "bm shop");
                //var result = msg.ShowAsync();
                AllCost += double.Parse(MaterialBigCost.Text);
                AllCostText.Text = AllCost.ToString("F2");
                QuantityMaterialInBasket++;
                QuantityMaterial.Text = GetCorrectWordFormForProduct(QuantityMaterialInBasket);
            }
            else
            {
                MaterialTextBox.IsEnabled = false;
                PlusBtn.IsEnabled = false;
                MinusBtn.IsEnabled = false;
                // Код, который нужно выполнить, если CheckBox не отмечен
                //var msg = new MessageDialog($"MatBigCst = {MaterialBigCost.Text}\nAllCost = {AllCost}", "bm shop");
                //var result = msg.ShowAsync();
                AllCost -= double.Parse(MaterialBigCost.Text);
                //msg = new MessageDialog($"AllCost = {AllCost}", "bm shop");
                //result = msg.ShowAsync();
                AllCostText.Text = AllCost.ToString("F2");
                QuantityMaterialInBasket--;
                QuantityMaterial.Text = GetCorrectWordFormForProduct(QuantityMaterialInBasket);
            }

            DB db = new DB();
            db.openConnection();

            using (MySqlConnection connection = db.getConnection())
            {
                string SqlCommandText = $"UPDATE `basket` SET `selected` = '{checkBox.IsChecked}' WHERE `basket`.`id` = {BasketId};";
                MySqlCommand command = new MySqlCommand(SqlCommandText, connection);
                command.ExecuteNonQuery();
            }

            db.closeConnection();
        }


        public double AllCost = 0;
        public int QuantityMaterialInBasket = 0;
        
        //Получение корректной формы слова "товар"
        public static string GetCorrectWordFormForProduct(int count)
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

        //Крестик удаления товара из корзины
        private void DeleteBtnClick(object sender, TappedRoutedEventArgs e, Windows.UI.Xaml.Controls.Border border, Materials CurrentMaterial)
        {
            border.Visibility = Visibility.Collapsed;
            //Удалить товар из корзины (запрос в бд)

            //DELETE FROM basket WHERE `basket`.`id` = 3
            DB db = new DB();
            db.openConnection();

            using (MySqlConnection connection = db.getConnection())
            {
                string SqlText = $"DELETE FROM basket WHERE `basket`.`materialId` = {CurrentMaterial.id} and `userId` = {SignInPage.CurrentUser.id}";
                MySqlCommand command = new MySqlCommand(SqlText, connection);

                if (command.ExecuteNonQuery() == 1)
                {
                    var msg = new MessageDialog($"Товар удалён из корзины", "bm shop");
                    var result = msg.ShowAsync();
                }
            }

            db.closeConnection();
            FillData();
        }

        //Кнопка уменьшения количества товара
        private void MinusBtnClick(object sender, TappedRoutedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox, CheckBox IsSelect)
        {
            if (sender is Button button)
            {
                var parentGrid = button.Parent as Grid;
                if (parentGrid != null)
                {
                    if(IsSelect.IsChecked == true)
                    {
                        if (quantityTextBox != null && materialCostTextBox != null && materialBigCostTextBox != null &&
                        int.TryParse(quantityTextBox.Text, out int quantity) &&
                        double.TryParse(materialCostTextBox.Text, out double materialCost))
                        {
                            //var minusBtn = sender as Button;
                            if (quantity >= 2)
                            {
                                quantity--;
                                quantityTextBox.Text = quantity.ToString();
                                //minusBtn.IsEnabled = true;
                            }
                            else if (quantity <= 1)
                            {
                                //minusBtn.IsEnabled = false;
                                //parentGrid.Visibility = Visibility.Collapsed;
                                //Удалить из корзины??TODO
                            }
                            //double newBigCost = quantity * materialCost;
                            //materialBigCostTextBox.Text = newBigCost.ToString("F2");
                        }
                    }
                }
            }
        }

        //Функция обработки текстового поля количества товара
        public void TextBoxTextChanged(object sender, TextChangedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox, CheckBox checkBox, Materials CurrentMaterial)
        {
            DB db1 = new DB();
            db1.openConnection();

            DataTable table = new DataTable();

            using (MySqlConnection connection = db1.getConnection())
            {
                string TextSqlCommand = $"SELECT m.quantity FROM `basket`b JOIN materials m WHERE {CurrentMaterial.id} = m.id and b.materialId = {CurrentMaterial.id};";
                MySqlCommand command = new MySqlCommand(TextSqlCommand, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            db1.closeConnection();

            foreach (DataRow row in table.Rows)
            {
                if (quantityTextBox.Text == string.Empty)
                {

                }
                else
                {
                    try
                    {
                        if (int.Parse(quantityTextBox.Text) >= int.Parse(row[0].ToString()))
                        {
                            quantityTextBox.Text = row[0].ToString();
                            quantityTextBox.SelectionStart = quantityTextBox.Text.Length;
                            //var plusBtn = sender as Button;
                            //plusBtn.IsEnabled = false;
                        }

                        if (checkBox.IsChecked == true)
                        {
                            quantityTextBox.IsReadOnly = false;
                            if (quantityTextBox.Text == string.Empty || quantityTextBox.Text == "0")
                            {
                                AllCost -= double.Parse(materialBigCostTextBox.Text);
                                double newBigCost = 1 * double.Parse(materialCostTextBox.Text);
                                materialBigCostTextBox.Text = newBigCost.ToString("F2");
                                quantityTextBox.Text = "1";
                                quantityTextBox.SelectionStart = quantityTextBox.Text.Length;
                                AllCost += double.Parse(materialBigCostTextBox.Text);
                                if (checkBox.IsChecked == true)
                                {
                                    AllCostText.Text = AllCost.ToString("F2");

                                    DB db = new DB();
                                    db.openConnection();

                                    using (MySqlConnection connection = db.getConnection())
                                    {
                                        string SqlCommandText = $"UPDATE `basket` SET `quantity` = '{quantityTextBox.Text}' WHERE `basket`.`materialId` = {CurrentMaterial.id} and `basket`.`userId` = {SignInPage.CurrentUser.id};";
                                        MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                                        command.ExecuteNonQuery();
                                    }

                                    db.closeConnection();
                                }
                            }
                            else
                            {
                                double buf = double.Parse(quantityTextBox.Text);
                                if (buf <= 0)
                                {
                                    AllCost -= double.Parse(materialBigCostTextBox.Text);
                                    double newBigCost = 1 * double.Parse(materialCostTextBox.Text);
                                    materialBigCostTextBox.Text = newBigCost.ToString("F2");
                                    AllCost += double.Parse(materialBigCostTextBox.Text);
                                    if (checkBox.IsChecked == true)
                                    {
                                        AllCostText.Text = AllCost.ToString("F2");

                                        DB db = new DB();
                                        db.openConnection();

                                        using (MySqlConnection connection = db.getConnection())
                                        {
                                            quantityTextBox.Text = "1";
                                            quantityTextBox.SelectionStart = quantityTextBox.Text.Length;
                                            string SqlCommandText = $"UPDATE `basket` SET `quantity` = '{quantityTextBox.Text}' WHERE `basket`.`materialId` = {CurrentMaterial.id} and `basket`.`userId` = {SignInPage.CurrentUser.id};";
                                            MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                                            command.ExecuteNonQuery();
                                        }

                                        db.closeConnection();
                                    }
                                }
                                else
                                {
                                    AllCost -= double.Parse(materialBigCostTextBox.Text);
                                    double newBigCost = buf * double.Parse(materialCostTextBox.Text);
                                    materialBigCostTextBox.Text = newBigCost.ToString("F2");
                                    AllCost += double.Parse(materialBigCostTextBox.Text);
                                    if (checkBox.IsChecked == true)
                                    {
                                        AllCostText.Text = AllCost.ToString("F2");

                                        DB db = new DB();
                                        db.openConnection();

                                        using (MySqlConnection connection = db.getConnection())
                                        {
                                            string SqlCommandText = $"UPDATE `basket` SET `quantity` = '{quantityTextBox.Text}' WHERE `basket`.`materialId` = {CurrentMaterial.id} and `basket`.`userId` = {SignInPage.CurrentUser.id};";
                                            MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                                            command.ExecuteNonQuery();
                                        }

                                        db.closeConnection();
                                    }
                                }
                            }
                        }
                        else
                        {
                            quantityTextBox.IsReadOnly = true;
                            // Перенести в проставление галочки
                        }
                    }
                    catch (Exception)
                    {
                        //var msg = new MessageDialog($"Введите корректное число", "bm shop");
                        //var result = msg.ShowAsync();
                    }
                }
            }
        }


        public void TextBoxLostFocus(object sender, RoutedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox, CheckBox checkBox, Materials currentMaterial)
        {
            quantityTextBox.IsReadOnly = false;
            // Вызов метода TextBoxTextChanged с текущими параметрами
            TextBoxTextChanged(sender, null, quantityTextBox, materialCostTextBox, materialBigCostTextBox, checkBox, currentMaterial);

            if (checkBox.IsChecked == true)
            {
                quantityTextBox.IsReadOnly = false;
                if (quantityTextBox.Text == string.Empty || quantityTextBox.Text == "0")
                {
                    AllCost -= double.Parse(materialBigCostTextBox.Text);
                    double newBigCost = 1 * double.Parse(materialCostTextBox.Text);
                    materialBigCostTextBox.Text = newBigCost.ToString("F2");
                    quantityTextBox.Text = "1";
                    quantityTextBox.SelectionStart = quantityTextBox.Text.Length;
                    AllCost += double.Parse(materialBigCostTextBox.Text);
                    if (checkBox.IsChecked == true)
                        AllCostText.Text = AllCost.ToString("F2");
                }
                else
                {
                    double buf = double.Parse(quantityTextBox.Text);
                    if (buf <= 0)
                    {
                        AllCost -= double.Parse(materialBigCostTextBox.Text);
                        double newBigCost = 1 * double.Parse(materialCostTextBox.Text);
                        materialBigCostTextBox.Text = newBigCost.ToString("F2");
                        AllCost += double.Parse(materialBigCostTextBox.Text);
                        if (checkBox.IsChecked == true)
                            AllCostText.Text = AllCost.ToString("F2");
                    }
                    else
                    {
                        AllCost -= double.Parse(materialBigCostTextBox.Text);
                        double newBigCost = buf * double.Parse(materialCostTextBox.Text);
                        materialBigCostTextBox.Text = newBigCost.ToString("F2");
                        AllCost += double.Parse(materialBigCostTextBox.Text);
                        if (checkBox.IsChecked == true)
                            AllCostText.Text = AllCost.ToString("F2");
                    }
                }
            }
            else
            {
                quantityTextBox.IsReadOnly = true;
                // Перенести в проставление галочки
            }
        }


        //Кнопка увеличения количества товара
        private void PlusBtnClick(object sender, TappedRoutedEventArgs e, TextBox quantityTextBox, TextBlock materialCostTextBox, TextBlock materialBigCostTextBox, CheckBox IsSelected, Materials currentMaterial)
        {
            if (sender is Button button)
            {
                var parentGrid = button.Parent as Grid;
                if (parentGrid != null)
                {
                    if(IsSelected.IsChecked == true)
                    {
                        if (quantityTextBox != null && materialCostTextBox != null && materialBigCostTextBox != null &&
                        int.TryParse(quantityTextBox.Text, out int quantity) &&
                        double.TryParse(materialCostTextBox.Text, out double materialCost))
                        {
                            DB db = new DB();
                            db.openConnection();

                            DataTable table = new DataTable();

                            using (MySqlConnection connection = db.getConnection())
                            {
                                string TextSqlCommand = $"SELECT m.quantity FROM `basket`b JOIN materials m WHERE {currentMaterial.id} = m.id and b.materialId = {currentMaterial.id};";
                                MySqlCommand command = new MySqlCommand(TextSqlCommand, connection);

                                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                                {
                                    adapter.Fill(table);
                                }
                            }

                            db.closeConnection();

                            foreach (DataRow row in table.Rows)
                            {
                                if(quantity >= int.Parse(row[0].ToString()))
                                {
                                    quantity = int.Parse(row[0].ToString());
                                    //var plusBtn = sender as Button;
                                    //plusBtn.IsEnabled = false;
                                }
                                else
                                {
                                    //var plusBtn = sender as Button;
                                    //plusBtn.IsEnabled = true;
                                    quantity++;
                                }
                            }

                            quantityTextBox.Text = quantity.ToString();

                            //double newBigCost = quantity * materialCost;
                            //materialBigCostTextBox.Text = newBigCost.ToString("F2");
                        }
                    }
                }
            }
        }

        //Получение названия товара и переход на страницу просмотра содержимого
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

        //Обработчик кнопки оплатить корзину
        private void BuyButtonClick(object sender, RoutedEventArgs e)
        {
            DB db = new DB();

            db.openConnection();

            DataTable table = new DataTable();


            List<Item> ExcelList = new List<Item>();

            using (MySqlConnection connection = db.getConnection())
            {
                string GetAllCorrectMaterialId = $"SELECT * FROM `basket` WHERE userId = \"{SignInPage.CurrentUser.id}\" and selected = \"true\";";
                MySqlCommand AllCorrectMaterialId = new MySqlCommand(GetAllCorrectMaterialId, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(AllCorrectMaterialId))
                {
                    adapter.Fill(table);
                }
            }

            db.closeConnection();

            List<BasketNote> BasketList = new List<BasketNote>();
            foreach (DataRow row in table.Rows)
            {
                int id = int.Parse(row[0].ToString());
                int materialId = int.Parse(row[1].ToString());
                int userId = int.Parse(row[2].ToString());
                int quantity = int.Parse(row[3].ToString());
                bool selected = bool.Parse(row[4].ToString());

                BasketNote buf = new BasketNote(id,materialId,userId, quantity, selected);
                BasketList.Add(buf);
            }

            //Количество для товара из таблицы товары
            List<int> QuantityInMaterial = new List<int>();
            foreach(var item in BasketList)
            {
                DB db1 = new DB();
                db1.openConnection();

                DataTable table1 = new DataTable();

                using (MySqlConnection connection1 = db1.getConnection())
                {
                    string CheckQuantityInMaterialTable = $"SELECT * FROM `materials` WHERE id = \"{item.materialId}\";";
                    MySqlCommand QuantityMaterialCommand = new MySqlCommand(CheckQuantityInMaterialTable, connection1);

                    using (MySqlDataAdapter adapter1 = new MySqlDataAdapter(QuantityMaterialCommand))
                    {
                        adapter1.Fill(table1);
                    }
                }

                db1.closeConnection();

                foreach (DataRow row in table1.Rows)
                {
                    QuantityInMaterial.Add(int.Parse(row[10].ToString()));

                    //if количество товара больше в корзине, то сообщение сосал яйца
                    if (item.quantity > int.Parse(row[10].ToString()))
                    {
                        var msgDialog = new MessageDialog($"Количество товара в корзине больше, чем количество товара в магазине", "bm shop");
                        var result = msgDialog.ShowAsync();
                    }
                    //else количество товара меньше в корзине, то:
                    else
                    {
                        DB db2 = new DB();
                        db2.openConnection();

                        DataTable table2 = new DataTable();

                        using (MySqlConnection connection2 = db2.getConnection())
                        {
                            string ChangeQuantityValueInMaterialTable = $"UPDATE `materials` SET `quantity` = '{int.Parse(row[10].ToString()) - item.quantity}' WHERE `materials`.`id` = {item.materialId};";
                            //значение в бд = бд - текущее
                            MySqlCommand ChangeQuantity = new MySqlCommand(ChangeQuantityValueInMaterialTable, connection2);
                            int ResChangeQuantity = ChangeQuantity.ExecuteNonQuery();

                            int ResWriteInPurchases = 0;
                            for (int i = 0; i < item.quantity; i++)
                            {
                                string WriteInPurchasesTable = $"INSERT INTO `purchases` (`id`, `materialId`, `userId`) VALUES (NULL, '{item.materialId}', '{SignInPage.CurrentUser.id}');";
                                MySqlCommand WriteInPurchases = new MySqlCommand(WriteInPurchasesTable, connection2);
                                ResWriteInPurchases = WriteInPurchases.ExecuteNonQuery();

                                Item buf = new Item(row[1].ToString(), decimal.Parse(row[5].ToString()));
                                ExcelList.Add(buf);
                            }

                            string DeleteFromBasketTable = $"DELETE FROM basket WHERE `basket`.`id` = {item.id}";
                            MySqlCommand DeleteBasketNote = new MySqlCommand(DeleteFromBasketTable, connection2);
                            int ResDeleteBasketNote = DeleteBasketNote.ExecuteNonQuery();
                        }

                        db2.closeConnection();
                    }
                }
            }

            ShowTransactionDialog(ExcelList);
        }

        //Класс экземляра товара для записи в Excel файл
        public class Item
        {
            public string Name { get; set; }
            public decimal Price { get; set; }

            public Item() { }
            public Item(string name, decimal price)
            {
                Name = name;
                Price = price;
            }
        }

        //Функция записи в Excel файл
        private async void ShowTransactionDialog(List<Item> ExcelList)
        {
            // Создание диалога
            var msgDialog1 = new ContentDialog
            {
                Title = "bm shop",
                Content = "Операция успешно завершена. Вы хотите получить чек?",
                PrimaryButtonText = "Да",
                SecondaryButtonText = "Нет"
            };

            // Показ диалога и обработка результата
            var result1 = await msgDialog1.ShowAsync();
            if (result1 == ContentDialogResult.Primary)
            {
                await GenerateReceiptAsync(ExcelList); // Дождаться завершения
            }
            else
            {
                FillData();
            }
        }


        public async Task GenerateReceiptAsync(List<Item> items)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    // Добавляем новый лист в документ
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Чек");

                    // Заполняем заголовок
                    worksheet.Cells[1, 1].Value = "ООО \"bm shop\"";
                    worksheet.Cells[2, 1].Value = "г.Витебск, пр Черняховского 31/5";
                    worksheet.Cells[4, 1].Value = "ПЛАТЕЖНЫЙ ДОКУМЕНТ";
                    worksheet.Cells[5, 1].Value = "ЧЕК ПРОДАЖИ";

                    // Заполняем заголовки колонок
                    worksheet.Cells[7, 1].Value = "Наименование";
                    worksheet.Cells[7, 2].Value = "Цена";

                    // Заполняем данные
                    int row = 8;
                    decimal total = 0;
                    foreach (var item in items)
                    {
                        worksheet.Cells[row, 1].Value = item.Name;
                        worksheet.Cells[row, 2].Value = item.Price;
                        total += item.Price;
                        row++;
                    }

                    // Итог
                    worksheet.Cells[row+1, 1].Value = "Итого к оплате";
                    worksheet.Cells[row+1, 2].Value = total;

                    // Стилизация
                    worksheet.Cells[1, 1, 1, 3].Merge = true;
                    worksheet.Cells[2, 1, 2, 3].Merge = true;
                    worksheet.Cells[4, 1, 4, 3].Merge = true;
                    worksheet.Cells[5, 1, 5, 3].Merge = true;
                    worksheet.Cells[7, 1, 7, 3].Style.Font.Bold = true;

                    // Авторазмер колонок
                    worksheet.Cells.AutoFitColumns();

                    // Стилизация заголовков
                    using (ExcelRange range = worksheet.Cells[1, 1, 5, 1])
                    {
                        range.Style.Font.Bold = true; // Установка жирного шрифта
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Выравнивание текста по центру
                    }

                    // Стилизация строки с итогом
                    using (ExcelRange range = worksheet.Cells[row + 1, 1, row + 1, 2])
                    {
                        range.Style.Font.Bold = true; // Установка жирного шрифта
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Выравнивание текста по правому краю
                    }


                    // Сохранение файла
                    var savePicker = new FileSavePicker
                    {
                        SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                        FileTypeChoices = { { "Excel files", new List<string> { ".xlsx" } } },
                        SuggestedFileName = "Receipt"
                    };

                    var file = await savePicker.PickSaveFileAsync();

                    if (file != null)
                    {
                        using (var stream = await file.OpenStreamForWriteAsync())
                        {
                            package.SaveAs(stream);
                        }

                        FillData();
                        var dialog = new ContentDialog
                        {
                            Title = "Файл сохранен",
                            Content = "Ваш чек был успешно сохранен.",
                            CloseButtonText = "ОК"
                        };
                        await dialog.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при сохранении файла
                var dialog = new ContentDialog
                {
                    Title = "Ошибка",
                    Content = $"Возникла ошибка при сохранении файла: {ex.Message}",
                    CloseButtonText = "ОК"
                };
                await dialog.ShowAsync();
                FillData();
            }
            FillData();
        }

    }
}
