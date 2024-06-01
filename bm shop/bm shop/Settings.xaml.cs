using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
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
    public sealed partial class Settings : ContentDialog
    {
        public Settings()
        {
            this.InitializeComponent();
            FillData();
        }

        // Обработчик нажатия кнопки "Сохранить"
        public void SaveButton_Click()
        {
            // Сохранение изменений
            DB db = new DB();
            db.openConnection();

            DataTable table = new DataTable();
            using (MySqlConnection connection = db.getConnection())
            {
                string CheckSqlCommandText = $"SELECT * FROM user WHERE login = \"{Login.Text}\";";
                MySqlCommand CheckCommand = new MySqlCommand(CheckSqlCommandText, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(CheckCommand))
                {
                    adapter.Fill(table);
                }
                
            }

            db.closeConnection();

            if (table.Rows.Count == 0 || SignInPage.CurrentUser.login == Login.Text)
            {

                DB db1 = new DB();
                db1.openConnection();

                using (MySqlConnection connection1 = db1.getConnection())
                {
                    string login = Login.Text;
                    string password = Password.Text;
                    string surname = Surname.Text;
                    string name = Name.Text;
                    string patronymic = Patronymic.Text;

                    if(login == string.Empty || password == string.Empty || surname == string.Empty || name == string.Empty || patronymic == string.Empty)
                    {
                        var msgDialog = new MessageDialog("Необходимо заполнить все поля", "bm shop");
                        var res = msgDialog.ShowAsync();
                    }
                    else
                    {
                        string SqlCommandText = $"UPDATE `user` SET `login` ='{login}', `password` ='{password}', `surname` = '{surname}', `name` = '{name}', `patronymic` = '{patronymic}' WHERE `user`.`id` = {SignInPage.CurrentUser.id};";
                        MySqlCommand command = new MySqlCommand(SqlCommandText, connection1);

                        if (command.ExecuteNonQuery() == 1)
                        {
                            var msgDialog = new MessageDialog("Данные успешно изменены", "bm shop");
                            var res = msgDialog.ShowAsync();

                            SignInPage.CurrentUser.login = Login.Text;
                            SignInPage.CurrentUser.password = Password.Text;
                            SignInPage.CurrentUser.surname = Surname.Text;
                            SignInPage.CurrentUser.name = Name.Text;
                            SignInPage.CurrentUser.patronymic = Patronymic.Text;
                            // Закрываем диалог
                            this.Hide();
                        }
                        else
                        {
                            var msgDialog = new MessageDialog("Возникла ошибка при изменении данных", "bm shop");
                            var res = msgDialog.ShowAsync();
                        }
                    }
                }
                db1.closeConnection();
            }
            else
            {
                var msgDialog = new MessageDialog("Такой логин уже занят, введите другой", "bm shop");
                var res = msgDialog.ShowAsync();
            }
        }

        // Обработчик нажатия кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Отмена изменений

            // Закрываем диалог
            this.Hide();
        }


        public void FillData()
        {
            Login.PlaceholderText = "Логин: " + SignInPage.CurrentUser.login;
            Password.PlaceholderText = "Пароль: " +  SignInPage.CurrentUser.password;
            Surname.PlaceholderText = "Фамилия: " + SignInPage.CurrentUser.surname;
            Name.PlaceholderText = "Имя: " + SignInPage.CurrentUser.name;
            Patronymic.PlaceholderText = "Отчество: " + SignInPage.CurrentUser.patronymic;
        }

        // Выйти из уч записи
        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Закрываем диалог
            ContentDialog.Hide();

            // Навигация на SignInPage
            (Window.Current.Content as Frame).Navigate(typeof(SignInPage));
        }
    }

}
