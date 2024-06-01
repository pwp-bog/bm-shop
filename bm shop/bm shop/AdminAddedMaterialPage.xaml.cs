using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
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
    public sealed partial class AdminAddedMaterialPage : Page
    {
        public AdminAddedMaterialPage()
        {
            this.InitializeComponent();
        }

        //Обработчик кнопки добавить товар
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            if(AddMaterialName.Text == String.Empty || AddMaterialDescription.Text == String.Empty || AddMaterialColor.Text == String.Empty || AddMaterialWeight.Text == String.Empty || AddMaterialCost.Text == String.Empty || AddMaterialAdvantages.Text == String.Empty || AddMaterialCharacteristics.Text == String.Empty || AddMaterialModeOfApplication.Text == String.Empty || AddMaterialStorage.Text == String.Empty || AddMaterialQuantity.Text == String.Empty || AddMaterialPhoto.Text == String.Empty || AddMaterialCategory.Text == String.Empty)
            {
                var msg = new MessageDialog("Для добавления данных необходимо заполнить все поля", "bm shop");
                var res = msg.ShowAsync();
            }
            else
            {
                DB db = new DB();

                db.openConnection();

                using (MySqlConnection connection = db.getConnection())
                {
                    string SqlCommandText = $"INSERT INTO `materials` (`id`, `name`, `description`, `color`, `weigth`, `cost`, `advantages`, `characteristics`, `modeOfApplication`, `storage`, `quantity`, `photo`, `category`) VALUES (NULL, '{AddMaterialName.Text}', '{AddMaterialDescription.Text}', '{AddMaterialColor.Text}', '{AddMaterialWeight.Text}', '{AddMaterialCost.Text}', '{AddMaterialAdvantages.Text}', '{AddMaterialCharacteristics.Text}', '{AddMaterialModeOfApplication.Text}', '{AddMaterialStorage.Text}', '{AddMaterialQuantity.Text}', '{AddMaterialPhoto.Text}', '{AddMaterialCategory.Text}');";
                    MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                    int res = command.ExecuteNonQuery();
                    var msg1 = new MessageDialog("Товар добавлен", "bm shop");
                    var res1 = msg1.ShowAsync();
                }
            }
        }

        //Обработчик кнопки удалить товар
        private void Button_Tapped11(object sender, TappedRoutedEventArgs e)
        {
            if(DeleteSearchBox.Text == String.Empty || DeleteSearchBoxWeight.Text == String.Empty)
            {
                var msg = new MessageDialog($"Заполните все поля для удаления товара", "bm shop");
                var result = msg.ShowAsync();
            }
            else
            {
                DB db = new DB();
                db.openConnection();

                using (MySqlConnection connection = db.getConnection())
                {
                    string SqlCommandText = $"DELETE FROM materials WHERE name = {DeleteSearchBox.Text} and weigth = {DeleteSearchBoxWeight.Text}";
                    MySqlCommand command = new MySqlCommand(SqlCommandText , connection);

                    if(command.ExecuteNonQuery() == 0)
                    {
                        var msg = new MessageDialog("Возникла ошибка при удалении", "bm shop");
                        var res = msg.ShowAsync();
                    }
                    else
                    {
                        var msg = new MessageDialog("Товар удалён", "bm shop");
                        var res = msg.ShowAsync();
                    }
                }

                db.closeConnection();
            }
        }

        //Обработчик кнопки поиск товара для его редактирования
        private void Button_Tapped1123(object sender, TappedRoutedEventArgs e)
        {
            if (EditNameMaterialTextBox.Text == String.Empty || EditWeigthMaterialTextBox.Text == String.Empty || EditValueOnDataBase.Text == String.Empty || ComboBoxEdit.SelectedItem == null)
            {
                var msg = new MessageDialog("Заполните все поля для редактирования товара", "bm shop");
                var result = msg.ShowAsync();
                return;
            }

            DB db = new DB();
            db.openConnection();

            string SqlCommandText = "";

            // Преобразование SelectedItem в ComboBoxItem и получение строки
            ComboBoxItem selectedItem = ComboBoxEdit.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                var msg = new MessageDialog("Некорректное значение в ComboBox", "bm shop");
                var result = msg.ShowAsync();
                return;
            }

            string selectedItemContent = selectedItem.Content.ToString();

            switch (selectedItemContent)
            {
                case "Наименование товара":
                    SqlCommandText = $"UPDATE `materials` SET `name` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Цвет товара":
                    SqlCommandText = $"UPDATE `materials` SET `color` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Объём товара":
                    SqlCommandText = $"UPDATE `materials` SET `weigth` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Стоимость товара":
                    SqlCommandText = $"UPDATE `materials` SET `cost` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Количество товара":
                    SqlCommandText = $"UPDATE `materials` SET `quantity` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Фотография товара":
                    SqlCommandText = $"UPDATE `materials` SET `photo` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Категория товара":
                    SqlCommandText = $"UPDATE `materials` SET `category` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Способ применения товара":
                    SqlCommandText = $"UPDATE `materials` SET `modeOfApplication` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Описание товара":
                    SqlCommandText = $"UPDATE `materials` SET `description` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Преимущества товара":
                    SqlCommandText = $"UPDATE `materials` SET `advantages` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Характеристики товара":
                    SqlCommandText = $"UPDATE `materials` SET `characteristics` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                case "Способ хранения товара":
                    SqlCommandText = $"UPDATE `materials` SET `storage` = @value WHERE `name` = @name AND `weigth` = @weigth;";
                    break;
                default:
                    var msg = new MessageDialog("Некорректное значение в ComboBox", "bm shop");
                    var result = msg.ShowAsync();
                    return;
            }

            using (MySqlConnection connection = db.getConnection())
            {
                MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                // Параметры для предотвращения SQL-инъекций
                command.Parameters.AddWithValue("@value", EditValueOnDataBase.Text);
                command.Parameters.AddWithValue("@name", EditNameMaterialTextBox.Text);
                command.Parameters.AddWithValue("@weigth", EditWeigthMaterialTextBox.Text);

                try
                {
                    if (command.ExecuteNonQuery() == 0)
                    {
                        var msg = new MessageDialog("Возникла ошибка при обновлении", "bm shop");
                        var res = msg.ShowAsync();
                    }
                    else
                    {
                        var msg = new MessageDialog("Информация о товаре обновлёна", "bm shop");
                        var res = msg.ShowAsync();
                    }
                }
                catch (Exception ex)
                {
                    var msg = new MessageDialog($"Ошибка: {ex.Message}", "bm shop");
                    var res = msg.ShowAsync();
                }
            }

            db.closeConnection();
        }

        //Добавить новых админов
        private void Button_Tapped11shuma(object sender, TappedRoutedEventArgs e)
        {
            if(AddNewAdminiUserTextBoxLogin.Text == String.Empty || AddNewAdminiUserTextBoxPassword.Text == String.Empty || AddNewAdminiUserTextBoxSurname.Text == String.Empty || AddNewAdminiUserTextBoxName.Text == String.Empty || AddNewAdminiUserTextBoxPatronymic.Text == String.Empty)
            {
                var msg = new MessageDialog("Для добавление администратора необходимо заполнить все поля", "bm shop");
                var res = msg.ShowAsync();
            }
            else
            {
                DB db = new DB();
                DB db1 = new DB();
                db.openConnection();
                db1.openConnection();

                DataTable table = new DataTable();

                using (MySqlConnection connection1 = db1.getConnection())
                {
                    string SqlCommandCheckLoginText = $"SELECT * FROM `user` WHERE id = '{AddNewAdminiUserTextBoxLogin.Text}';";
                    MySqlCommand command = new MySqlCommand(SqlCommandCheckLoginText, connection1);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
                db1.closeConnection();

                bool BufValue = false;
                if(table.Rows.Count == 0)
                {
                    BufValue = true;
                }   
                

                if(BufValue)
                {
                    using (MySqlConnection connection = db.getConnection())
                    {
                        string SqlCommandText = $"INSERT INTO `user` (`id`, `login`, `password`, `surname`, `name`, `patronymic`, `admin`) VALUES (NULL, '{AddNewAdminiUserTextBoxLogin.Text}', '{AddNewAdminiUserTextBoxPassword.Text}', '{AddNewAdminiUserTextBoxSurname.Text}', '{AddNewAdminiUserTextBoxName.Text}', '{AddNewAdminiUserTextBoxPatronymic.Text}', 'true');";
                        MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                        if (command.ExecuteNonQuery() == 1)
                        {
                            var msg1 = new MessageDialog("Учётная запись успешно добавлена", "bm shop");
                            var res1 = msg1.ShowAsync();
                        }
                        else
                        {
                            var msg1 = new MessageDialog("Возникла ошибка при добавлении учётной записи", "bm shop");
                            var res1 = msg1.ShowAsync();
                        }
                    }

                    db.closeConnection();
                }
                else
                {
                    var msg = new MessageDialog("Данный логин уже занят", "bm shop");
                    var res = msg.ShowAsync();
                }
            }
        }
    }
}
