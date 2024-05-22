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
using Windows.UI;
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

        public static bool isBasket;
        public Dictionary<string, string> AdditionalInfoButtonsText = new Dictionary<string, string>();
        public void FillData()
        {
            NameMaterial.Text = CatalogPage.CurrentMateriall.name;

            BitmapImage img = new BitmapImage();
            img.UriSource = new Uri(CatalogPage.CurrentMateriall.photo);
            ImageMaterial.Source = img;
            FooterText.Text = CatalogPage.CurrentMateriall.description;

            AdditionalInfoButtonsText.Add("Описание", CatalogPage.CurrentMateriall.description);
            AdditionalInfoButtonsText.Add("Преимущества", CatalogPage.CurrentMateriall.advantages);
            AdditionalInfoButtonsText.Add("Характеристики", CatalogPage.CurrentMateriall.characteristics);
            AdditionalInfoButtonsText.Add("Способ применения", CatalogPage.CurrentMateriall.modeOfApplication);
            AdditionalInfoButtonsText.Add("Хранение", CatalogPage.CurrentMateriall.storage);
            AdditionalInfoButtonsText.Add("Отзывы", "Тут пока ни чё нет");


            DB db = new DB();

            db.openConnection();

            DataTable table = new DataTable();
            DataTable table1 = new DataTable();

            using (MySqlConnection connection = new DB().getConnection())
            {
                string command1 = "";

                if (isBasket)
                {
                    command1 = $"SELECT DISTINCT weigth, cost, quantity FROM `materials` m JOIN purchases p where m.name = \"{CatalogPage.CurrentMateriall.name}\" and m.id = p.materialId;";
                }
                else
                {
                    command1 = $"SELECT weigth, cost, quantity FROM `materials` where name = \"{CatalogPage.CurrentMateriall.name}\" and quantity > 0;";
                }
                MySqlCommand findAllWeight = new MySqlCommand(command1, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(findAllWeight))
                {
                    adapter.Fill(table);
                }

                MySqlCommand QyantityMaterial = new MySqlCommand($"SELECT weigth, cost, quantity FROM `materials` m JOIN purchases p where m.name = \"{CatalogPage.CurrentMateriall.name}\" and m.id = p.materialId and weigth = \"{CatalogPage.CurrentMateriall.weigth}\";", connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(QyantityMaterial))
                {
                    adapter.Fill(table1);
                }
            }

            db.closeConnection();

            foreach (DataRow row in table.Rows)
            {
                WeigthMaterial.Items.Add(row[0]);
            }

            int countBueydMaterial = 0;
            foreach (DataRow row in table1.Rows)
            {
                countBueydMaterial++;
            }

            WeigthMaterial.SelectedItem = CatalogPage.CurrentMateriall.weigth;
            //WeigthMaterial.PlaceholderText = CatalogPage.CurrentMateriall.weigth;

            if (isBasket)
            {
                Quantity.Text = countBueydMaterial.ToString() + " шт.";
                CostMaterial.Text = (CatalogPage.CurrentMateriall.cost * countBueydMaterial).ToString() + " руб.";
                Quantity.Visibility = Visibility.Visible;
                BuyButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                Quantity.Visibility = Visibility.Collapsed;
                BuyButton.Visibility = Visibility.Visible;
            }
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
                        Color accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
                        button.BorderBrush = new SolidColorBrush(accentColor); // Цвет для нажатой кнопки
                        FooterText.Text = AdditionalInfoButtonsText[button.Content.ToString()];
                    }
                    else
                    {
                        // Получение ресурса
                        var resource = Application.Current.Resources["AppBarItemDisabledForegroundThemeBrush"] as SolidColorBrush;

                        if (resource != null)
                        {
                            // Установка BorderBrush
                            button.BorderBrush = resource;
                        }
                        else
                        {
                            // Ресурс не найден, можете задать цвет по умолчанию
                            button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
                        }
                    }
                }
            }
        }

        private void WeigthMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DB db = new DB();

            db.openConnection();

            DataTable table = new DataTable();
            DataTable table1 = new DataTable();

            using (MySqlConnection connection = new DB().getConnection())
            {
                MySqlCommand findAllWeight = new MySqlCommand($"SELECT cost FROM `materials` where name = \"{CatalogPage.CurrentMateriall.name}\" and quantity > 0 and weigth = \"{WeigthMaterial.SelectedItem}\";", connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(findAllWeight))
                {
                    adapter.Fill(table);
                }

                MySqlCommand QyantityMaterial = new MySqlCommand($"SELECT weigth, cost, quantity FROM `materials` m JOIN purchases p where m.name = \"{CatalogPage.CurrentMateriall.name}\" and m.id = p.materialId and weigth = \"{WeigthMaterial.SelectedItem}\";", connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(QyantityMaterial))
                {
                    adapter.Fill(table1);
                }
            }

            db.closeConnection();

            double ThisCost = 0;
            foreach (DataRow row in table.Rows)
            {
                CostMaterial.Text = row[0].ToString() + " руб.";
                ThisCost = double.Parse(row[0].ToString());
            }

            int countBueydMaterial = 0;
            foreach (DataRow row in table1.Rows)
            {
                countBueydMaterial++;
            }
            Quantity.Text = countBueydMaterial.ToString() + " шт.";

            if (isBasket)
            {
                Quantity.Text = countBueydMaterial.ToString() + " шт.";
                CostMaterial.Text = (ThisCost * countBueydMaterial).ToString() + " руб.";
                Quantity.Visibility = Visibility.Visible;
                BuyButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                //Quantity.Text = countBueydMaterial.ToString() + " шт.";
                //CostMaterial.Text = ThisCost + " руб.";
                Quantity.Visibility = Visibility.Collapsed;
                BuyButton.Visibility = Visibility.Visible;
            }
        }

        //Кнопка купить
        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (WeigthMaterial.SelectedItem != null)
            {
                // Открываем соединение с базой данных
                DB db = new DB();
                db.openConnection();
                using (MySqlConnection connection = new DB().getConnection())
                {
                    // Проверяем, что соединение открыто
                    if (db.getConnection().State == ConnectionState.Open)
                    {
                        DataTable table = new DataTable();
                        DataTable table1 = new DataTable();

                        //// Команда изменения количества
                        //string editQuantityQuery = "UPDATE `materials` SET `quantity` = quantity - 1 WHERE `name` = @name AND `weigth` = @weigth";
                        //MySqlCommand editQuantityCommand = new MySqlCommand(editQuantityQuery, db.getConnection());
                        //editQuantityCommand.Parameters.AddWithValue("@name", CatalogPage.CurrentMateriall.name);
                        //editQuantityCommand.Parameters.AddWithValue("@weigth", WeigthMaterial.SelectedItem.ToString());

                        // Команда получения id для изменённого товара
                        string getIdCurrentMaterialQuery = "SELECT id FROM `materials` WHERE `name` = @name AND `weigth` = @weigth AND quantity > 0";
                        MySqlCommand getIdCurrentMaterial = new MySqlCommand(getIdCurrentMaterialQuery, db.getConnection());
                        getIdCurrentMaterial.Parameters.AddWithValue("@name", CatalogPage.CurrentMateriall.name);
                        getIdCurrentMaterial.Parameters.AddWithValue("@weigth", WeigthMaterial.SelectedItem.ToString());

                        // id изменённого товара
                        int buf = 0;
                        // Получение id товара
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(getIdCurrentMaterial))
                        {
                            adapter.Fill(table);
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            buf = int.Parse(row[0].ToString());
                        }

                        //Команда для увеличения количества товара
                        string CheckMaterialInBasket = $"SELECT b.id, b.materialId, b.userId, b.quantity FROM `materials` m JOIN basket b WHERE {buf} = b.materialId;";
                        MySqlCommand CheckMaterialInBasketCommand = new MySqlCommand(CheckMaterialInBasket, db.getConnection());
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(CheckMaterialInBasketCommand))
                        {
                            adapter.Fill(table1);
                        }

                        int BasketId = 0;
                        int materialId = 0;
                        int userId = 0;
                        int quantity = 0;
                        
                        foreach(DataRow row in table1.Rows)
                        {
                            BasketId = int.Parse(row[0].ToString());
                            materialId = int.Parse(row[1].ToString());
                            userId = int.Parse(row[2].ToString());
                            quantity = int.Parse(row[3].ToString());
                        }

                        if (BasketId == 0 && materialId == 0 && userId == 0 && quantity == 0)
                        {
                            // Команда для записи в таблицу покупок
                            string insertIntoPurchasesQuery = "INSERT INTO `basket` (`id`, `materialId`, `userId`, `quantity`) VALUES (NULL, @materialId, @userId, @quantity)";
                            MySqlCommand insertIntoPurchases = new MySqlCommand(insertIntoPurchasesQuery, db.getConnection());
                            insertIntoPurchases.Parameters.AddWithValue("@materialId", buf);
                            insertIntoPurchases.Parameters.AddWithValue("@userId", SignInPage.CurrentUser.id);
                            insertIntoPurchases.Parameters.AddWithValue("@quantity", 1);

                            // Выполнение команд изменения количества и записи товара
                            try
                            {
                                int insertResult = insertIntoPurchases.ExecuteNonQuery();
                                if (insertResult == 1)
                                {
                                    //int editResult = editQuantityCommand.ExecuteNonQuery();
                                    //if (editResult == 1)
                                    //{
                                    //    var messageDialog = new MessageDialog("Товар добавлен в корзину", "bm shop");
                                    //    await messageDialog.ShowAsync();
                                    //}
                                    //else
                                    //{
                                    //    var messageDialog = new MessageDialog("Возникла ошибка во время добавления товара в корзину, попробуйте снова", "bm shop");
                                    //    await messageDialog.ShowAsync();
                                    //}
                                }
                                else
                                {
                                    var messageDialog = new MessageDialog("Возникла ошибка во время добавления товара в корзину, попробуйте снова", "bm shop");
                                    await messageDialog.ShowAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                var messageDialog = new MessageDialog($"Возникла ошибка: {ex.Message}", "bm shop");
                                // Использование IndexOf с StringComparison.OrdinalIgnoreCase для сравнения без учета регистра
                                if (ex.Message.IndexOf("Cannot add or update a child row: a foreign key constraint fails", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    messageDialog = new MessageDialog("Товар закончился", "bm shop");
                                }
                                await messageDialog.ShowAsync();
                            }
                            finally
                            {
                                db.closeConnection();
                            }
                        }
                        else
                        {
                            string updateValueInBasket = $"UPDATE `basket` SET `quantity` = {quantity+1} WHERE `basket`.`id` = {BasketId};";
                            MySqlCommand insertIntoBasket = new MySqlCommand(updateValueInBasket, db.getConnection());
                            // Выполнение команд изменения количества и записи товара
                            try
                            {
                                int insertResult = insertIntoBasket.ExecuteNonQuery();
                                if (insertResult == 1)
                                {
                                    //int editResult = editQuantityCommand.ExecuteNonQuery();
                                    //if (editResult == 1)
                                    //{
                                    var messageDialog = new MessageDialog("Товар успешно добавлен в корзину", "bm shop");
                                    await messageDialog.ShowAsync();
                                    //}
                                    //else
                                    //{
                                    //    var messageDialog = new MessageDialog("Возникла ошибка во время добавления товара в корзину, попробуйте снова", "bm shop");
                                    //    await messageDialog.ShowAsync();
                                    //}
                                }
                                else
                                {
                                    var messageDialog = new MessageDialog("Возникла ошибка во время добавления товара в корзину, попробуйте снова", "bm shop");
                                    await messageDialog.ShowAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                var messageDialog = new MessageDialog($"Возникла ошибка: {ex.Message}", "bm shop");
                                // Использование IndexOf с StringComparison.OrdinalIgnoreCase для сравнения без учета регистра
                                if (ex.Message.IndexOf("Cannot add or update a child row: a foreign key constraint fails", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    messageDialog = new MessageDialog("Товар закончился", "bm shop");
                                }
                                await messageDialog.ShowAsync();
                            }
                            finally
                            {
                                db.closeConnection();
                            }
                        }
                        

                    }
                    else
                    {
                        var messageDialog = new MessageDialog("Не удалось подключиться к базе данных.", "bm shop");
                        await messageDialog.ShowAsync();
                    }
                }
            }
            else
            {
                var msg = new MessageDialog("Выберите объём перед покупкой", "bm shop");
                await msg.ShowAsync();
            }
        }
    }
}
