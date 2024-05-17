using MySqlConnector;
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
    public sealed partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            this.InitializeComponent();
        }

        //Обработчик кнопки назад
        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }

        //Обработчик кнопки SignUp
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Text;
            string surname = SurnameTextBox.Text;
            string name = NameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(surname) ||
            string.IsNullOrEmpty(name) || string.IsNullOrEmpty(patronymic))
            {
                var msg = new MessageDialog("Для создания учётной записи необходимо заполнить все поля", "bm shop");
                var result = msg.ShowAsync();
            }
            else
            {
                DB db = new DB();
                db.openConnection();

                using (MySqlConnection connection = db.getConnection())
                {
                    // Проверка наличия логина в базе данных
                    MySqlCommand checkCommand = new MySqlCommand(
                        $"SELECT COUNT(*) FROM `user` WHERE `login` = @login",
                        db.getConnection());
                    checkCommand.Parameters.AddWithValue("@login", login);
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        var messageDialog = new MessageDialog("Учётная запись с таким логином уже существует", "bm shop");
                        var result = messageDialog.ShowAsync();
                    }
                    else
                    {
                        // Продолжение кода для вставки новой записи
                        MySqlCommand insertCommand = new MySqlCommand(
                            $"INSERT INTO `user` (`login`, `password`, `surname`, `name`, `patronymic`, `admin`)" +
                            $"VALUES (@login, @password, @surname, @name, @patronymic, @admin);",
                            db.getConnection());
                        insertCommand.Parameters.AddWithValue("@login", login);
                        insertCommand.Parameters.AddWithValue("@password", password);
                        insertCommand.Parameters.AddWithValue("@surname", surname);
                        insertCommand.Parameters.AddWithValue("@name", name);
                        insertCommand.Parameters.AddWithValue("@patronymic", patronymic);
                        insertCommand.Parameters.AddWithValue("@admin", "false");

                        if (insertCommand.ExecuteNonQuery() == 1)
                        {
                            var messageDialog = new MessageDialog("Учётная запись успешно создана", "bm shop");
                            var result = messageDialog.ShowAsync();

                            Frame.Navigate(typeof(SignInPage));
                        }
                        else
                        {
                            var messageDialog = new MessageDialog("Возникла ошибка при создании учётной записи, попробуйте снова", "bm shop");
                            var result = messageDialog.ShowAsync();
                        }
                    }
                    db.closeConnection();
                }
            }
        }
    }
}
