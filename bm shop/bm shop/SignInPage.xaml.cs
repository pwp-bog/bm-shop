using MySqlConnector;
using System;
using System.Collections.Generic;
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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace bm_shop
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        public SignInPage()
        {
            this.InitializeComponent();
        }

        public static User CurrentUser;

        //Обработка кнопки назад
        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }


        //Обработка кнопки SignIn
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Проверка на корректность введённых данных
            if(!string.IsNullOrEmpty(LoginTextBox.Text) && !string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                CurrentUser = null;
                string login = LoginTextBox.Text;
                string password = PasswordTextBox.Text;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    var msg = new MessageDialog("Заполните все значения для входа в учётную запись", "bm shop");
                    var result = msg.ShowAsync();
                }
                else
                {
                    DB db = new DB();
                    db.openConnection();

                    DataTable table = new DataTable();

                    using (MySqlConnection connection = db.getConnection())
                    {
                        MySqlCommand command = new MySqlCommand($"SELECT * FROM `user` WHERE login = @login AND password = @password;", connection);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(table);
                        }
                    }

                    db.closeConnection();

                    if (table.Rows.Count > 0)
                    {
                        CurrentUser = new User(
                            Convert.ToInt32(table.Rows[0][0].ToString()),
                            table.Rows[0][1].ToString(),
                            table.Rows[0][2].ToString(),
                            table.Rows[0][3].ToString(),
                            table.Rows[0][4].ToString(),
                            table.Rows[0][5].ToString(),
                            bool.Parse(table.Rows[0][6].ToString())
                        );

                        Frame.Navigate(typeof(MainPage));
                    }
                    else
                    {
                        var msg = new MessageDialog("Ошибка входа, проверьте введённые данные и попробуйте снова", "bm shop");
                        var result = msg.ShowAsync();
                    }
                }
            }
            else
            {
                var msg = new MessageDialog($"Для входа в приложение, необходимо заполнить все поля", "bm shop");
                var result = msg.ShowAsync();
            }
        }
    }
}
