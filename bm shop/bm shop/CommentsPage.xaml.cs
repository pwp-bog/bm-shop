using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation.Geofencing;
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
    public sealed partial class CommentsPage : Page
    {
        public CommentsPage()
        {
            this.InitializeComponent();
            FillData();
        }

        //Список комментариев
        List<Comments> comments = new List<Comments>();
        List<string> weight = new List<string>();
        List<string> name = new List<string>();

        //Функция заполнения комментариями
        public void FillData()
        {
            //Очистка таблицы отзывов
            CatalogGrid.Children.Clear();

            DB db = new DB();
            db.openConnection();

            DataTable table = new DataTable();

            using (MySqlConnection connection = db.getConnection())
            {
                string SqlCommandText = $"SELECT c.id, c.materialId, c.userId, c.materialMark, c.dateOfWriting, c.commentText, c.quantityLikeDislike, c.commentId, m.weigth, u.name FROM `comments` c JOIN `materials` m ON c.materialId = m.id JOIN `user` u ON c.userId = u.id WHERE c.materialMark IS NOT NULL AND materialId = {CatalogPage.CurrentMateriall.id} ORDER BY `quantityLikeDislike` DESC;";
                MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            db.closeConnection();
            
            foreach (DataRow row in table.Rows)
            {
                Comments buf = new Comments(int.Parse(row[0].ToString()), int.Parse(row[1].ToString()), int.Parse(row[2].ToString()),
                    row[3].ToString(), row[4].ToString(), row[5].ToString(), int.Parse(row[6].ToString()), row[7].ToString());
                comments.Add(buf);
                weight.Add(row[8].ToString());
                name.Add(row[9].ToString());
            }

            //Создание плашек
            int j = 0;
            int k = 1;
            CatalogGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            CreateAnswerAboutMaterial();
            for (int i = 0; i < comments.Count; i++)
            {
                // Определение столбцов и строк
                CatalogGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                CatalogGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                CreateComment(k, j, comments[i], weight[i], name[i]);

                j++;
                if (j >= 1)
                {
                    j = 0;
                    k++;
                }
            }

            if(comments.Count == 0)
            {
                TextBlock MaterialMark = new TextBlock();
                MaterialMark.Text = "У данного товара ещё нет отзывов";
                MaterialMark.FontSize = 20;
                MaterialMark.Name = "MaterialMark";
                MaterialMark.HorizontalAlignment = HorizontalAlignment.Left;
                MaterialMark.VerticalAlignment = VerticalAlignment.Top;
                MaterialMark.Margin = new Thickness(0, 0, 0, 0);

                Grid.SetRow(MaterialMark, 1);
                Grid.SetColumn(MaterialMark, 1);
                CatalogGrid.Children.Add(MaterialMark);
            }
        }

        //Функция создания плашки комментария
        public void CreateComment(int row, int column, Comments CurrentComment, string Weight, string Name)
        {
            // Создание нового объекта Border
            Windows.UI.Xaml.Controls.Border border = new Windows.UI.Xaml.Controls.Border();
            border.Name = CurrentComment.id.ToString();
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.Margin = new Thickness(0, 5, 10, 15);

            // Создание оценки товара
            TextBlock MaterialMark = new TextBlock();
            MaterialMark.Text = GetQuantityStar(CurrentComment.materialMark);
            MaterialMark.FontSize = 20;
            MaterialMark.Name = "MaterialMark";
            MaterialMark.HorizontalAlignment = HorizontalAlignment.Left;
            MaterialMark.VerticalAlignment = VerticalAlignment.Top;
            MaterialMark.Margin = new Thickness(0, 0, 0, 0);

            // Создание Имя комментатора
            TextBlock  UserName = new TextBlock();
            UserName.Text = Name;
            UserName.HorizontalAlignment = HorizontalAlignment.Left;
            UserName.VerticalAlignment = VerticalAlignment.Top;
            UserName.FontSize = 20;
            UserName.Name = "UserName";
            UserName.Margin = new Thickness(10, 0, 0, 0);

            // Создание даты публикации отзыва
            TextBlock DateOfWriting = new TextBlock();
            DateOfWriting.Text = ConvertToDate(CurrentComment.dateOfWriting);
            DateOfWriting.HorizontalAlignment = HorizontalAlignment.Left;
            DateOfWriting.VerticalAlignment = VerticalAlignment.Top;
            DateOfWriting.FontSize = 20;
            DateOfWriting.Name = "DateOfWriting";
            DateOfWriting.Margin = new Thickness(10, 0, 0, 0);

            // Создание объёма товара
            TextBlock WeightMaterial = new TextBlock();
            WeightMaterial.Text = Weight;
            WeightMaterial.HorizontalAlignment = HorizontalAlignment.Left;
            WeightMaterial.VerticalAlignment = VerticalAlignment.Top;
            WeightMaterial.FontSize = 20;
            WeightMaterial.Name = "Weight";
            WeightMaterial.Margin = new Thickness(0, 0, 0, 0);


            // Создание текста отзыва
            TextBlock CommentText = new TextBlock();
            CommentText.Text = CurrentComment.commentText;
            CommentText.HorizontalAlignment = HorizontalAlignment.Left;
            CommentText.VerticalAlignment = VerticalAlignment.Top;
            CommentText.FontSize = 20;
            CommentText.Name = "CommentText";
            CommentText.Margin = new Thickness(0, 10, 0, 0);

            // Создание кнопки лайк
            Button LikeButton = new Button();
            LikeButton.Content = "🖒";
            LikeButton.HorizontalAlignment = HorizontalAlignment.Left;
            LikeButton.VerticalAlignment = VerticalAlignment.Top;
            LikeButton.FontSize = 20;
            LikeButton.Name = "LikeButton";
            LikeButton.Margin = new Thickness(0, 0, 0, 0);

            // Создание кнопки дизлайк
            Button DislikeButton = new Button();
            DislikeButton.Content = "🖓";
            DislikeButton.HorizontalAlignment = HorizontalAlignment.Left;
            DislikeButton.VerticalAlignment = VerticalAlignment.Top;
            DislikeButton.FontSize = 20;
            DislikeButton.Name = "DislikeButton";
            DislikeButton.Margin = new Thickness(5, 0, 0, 0);

            // Создание кнопки Ответить
            Button AnswerButton = new Button();
            AnswerButton.Content = "Ответить";
            AnswerButton.HorizontalAlignment = HorizontalAlignment.Left;
            AnswerButton.VerticalAlignment = VerticalAlignment.Top;
            AnswerButton.FontSize = 20;
            AnswerButton.Name = "AnswerButton";
            AnswerButton.Margin = new Thickness(5, 0, 0, 0);

            // Создание поля для ввода ответа
            TextBox WriteAnswerTextBox = new TextBox();
            WriteAnswerTextBox.PlaceholderText = "Комментарий:";
            WriteAnswerTextBox.FontSize = 20;
            WriteAnswerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            WriteAnswerTextBox.VerticalAlignment = VerticalAlignment.Top;
            WriteAnswerTextBox.Name = "WriteAnswerTextBox";
            WriteAnswerTextBox.Visibility = Visibility.Collapsed;

            // Создание кнопки отмены написания ответа
            Button  CancelWriteButton = new Button();
            CancelWriteButton.Content = "Отмена";
            CancelWriteButton.FontSize = 20;
            CancelWriteButton.HorizontalAlignment = HorizontalAlignment.Left;
            CancelWriteButton.VerticalAlignment = VerticalAlignment.Top;
            CancelWriteButton.Name = "CancelWriteButton";
            CancelWriteButton.Visibility = Visibility.Collapsed;

            // Создание кнопки ответить
            Button SendAnswerButton = new Button();
            SendAnswerButton.Content = "Отправить";
            SendAnswerButton.FontSize = 20;
            SendAnswerButton.HorizontalAlignment = HorizontalAlignment.Left;
            SendAnswerButton.VerticalAlignment = VerticalAlignment.Top;
            SendAnswerButton.Name = "SendAnswerButton";
            SendAnswerButton.Visibility = Visibility.Collapsed;

            // Создание кнопки посмотреть ответы
            TextBlock GetAnswers = new TextBlock();
            GetAnswers.Text = "v Ответы";
            GetAnswers.HorizontalAlignment = HorizontalAlignment.Left;
            GetAnswers.VerticalAlignment = VerticalAlignment.Top;
            GetAnswers.FontSize = 20;
            GetAnswers.Name = "GetAnswers";
            GetAnswers.Margin = new Thickness(0, 0, 0, 0);

            // Создание таблицы для отображение ответов на комментарии
            Grid AnswersGrid = new Grid();
            AnswersGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            FillAnswerGrid(CurrentComment.id, GetAnswers, AnswersGrid);


            Grid containerGrid = new Grid();
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });


            containerGrid.Children.Add(UserName);
            containerGrid.Children.Add(DateOfWriting);
            containerGrid.Children.Add(WeightMaterial);
            containerGrid.Children.Add(CommentText);
            containerGrid.Children.Add(LikeButton);
            containerGrid.Children.Add(DislikeButton);
            containerGrid.Children.Add(AnswerButton);
            containerGrid.Children.Add(WriteAnswerTextBox);
            containerGrid.Children.Add(CancelWriteButton);
            containerGrid.Children.Add(SendAnswerButton);
            containerGrid.Children.Add(GetAnswers);
            containerGrid.Children.Add(AnswersGrid);

            // Задание расположения Border в Grid
            Grid.SetRow(WeightMaterial, 1);
            Grid.SetColumn(WeightMaterial, 0);
            Grid.SetRow(CommentText, 2);
            Grid.SetColumn(CommentText, 0);
            Grid.SetColumnSpan(CommentText, 3);
            Grid.SetRow(LikeButton, 3);
            Grid.SetColumn(LikeButton, 0);
            Grid.SetRow(DislikeButton, 3);
            Grid.SetColumn(DislikeButton, 1);
            Grid.SetRow(AnswerButton, 3);
            Grid.SetColumn(AnswerButton, 2);
            Grid.SetRow(WriteAnswerTextBox, 4);
            Grid.SetColumn(WriteAnswerTextBox, 0);
            //Grid.SetColumnSpan(WriteAnswerTextBox, 3);
            Grid.SetRow(CancelWriteButton, 4);
            Grid.SetColumn(CancelWriteButton, 1);
            Grid.SetRow(SendAnswerButton, 4);
            Grid.SetColumn(SendAnswerButton, 2);

            Grid.SetRow(GetAnswers, 5);
            Grid.SetColumn(GetAnswers, 0);
            Grid.SetRow(AnswersGrid, 6);
            Grid.SetColumn(AnswersGrid, 0);
            Grid.SetColumnSpan(AnswersGrid, 3);

            containerGrid.Children.Add(MaterialMark);

            Grid.SetRow(MaterialMark, 0);
            Grid.SetColumn(MaterialMark, 0);
            Grid.SetRow(UserName, 0);
            Grid.SetColumn(UserName, 1);
            Grid.SetRow(DateOfWriting, 0);
            Grid.SetColumn(DateOfWriting, 2);
            

            // Задание функции обработчика нажатия на кнопку ответы
            GetAnswers.Tapped += (sender, e) =>  HideOrOpenAnswerComment(sender, e, AnswersGrid, GetAnswers);
            AnswersGrid.Visibility = Visibility.Collapsed;
            AnswerButton.Click += (sender, e) => HideOrOpenAnswerTextBoxAndButtons(sender, e, WriteAnswerTextBox, CancelWriteButton, SendAnswerButton);
            SendAnswerButton.Click += (sender, e) => CreateAnswer(sender, e, WriteAnswerTextBox, AnswersGrid, GetAnswers,   CurrentComment);


            // Задание расположения Border в Grid
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            border.Child = containerGrid;
            // Добавление объекта Border в Grid
            CatalogGrid.Children.Add(border);
        }

        // Плашка для создания отзыва о товаре
        public void CreateAnswerAboutMaterial()
        {
            var brush1 = new SolidColorBrush(Windows.UI.Colors.Transparent);

            // Создание нового объекта Border
            Windows.UI.Xaml.Controls.Border border = new Windows.UI.Xaml.Controls.Border();
            border.Background = brush1;
            border.HorizontalAlignment = HorizontalAlignment.Left;
            border.Margin = new Thickness(0, 5, 10, 15);

            // Создание поля для ввода ответа
            TextBox WriteAnswerTextBox = new TextBox();
            WriteAnswerTextBox.PlaceholderText = "Комментарий:";
            WriteAnswerTextBox.FontSize = 20;
            WriteAnswerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            WriteAnswerTextBox.VerticalAlignment = VerticalAlignment.Top;
            WriteAnswerTextBox.Name = "WriteAnswerTextBox";
            WriteAnswerTextBox.Width = 850;
            WriteAnswerTextBox.TextWrapping = TextWrapping.Wrap;

            //          Создание звёзд

            // Первая звезда
            TextBlock FirstStar = new TextBlock();
            FirstStar.Text = "☆";
            FirstStar.FontSize = 30;
            FirstStar.HorizontalAlignment = HorizontalAlignment.Left;
            FirstStar.VerticalAlignment = VerticalAlignment.Top;
            FirstStar.Name = "1";

            // Вторая звезда
            TextBlock SecondaryStar = new TextBlock();
            SecondaryStar.Text = "☆";
            SecondaryStar.FontSize = 30;
            SecondaryStar.HorizontalAlignment = HorizontalAlignment.Left;
            SecondaryStar.VerticalAlignment = VerticalAlignment.Top;
            SecondaryStar.Name = "2";

            // Третья звезда
            TextBlock ThirdStar = new TextBlock();
            ThirdStar.Text = "☆";
            ThirdStar.FontSize = 30;
            ThirdStar.HorizontalAlignment = HorizontalAlignment.Left;
            ThirdStar.VerticalAlignment = VerticalAlignment.Top;
            ThirdStar.Name = "3";

            // Четвёртая звезда
            TextBlock FourthStar = new TextBlock();
            FourthStar.Text = "☆";
            FourthStar.FontSize = 30;
            FourthStar.HorizontalAlignment = HorizontalAlignment.Left;
            FourthStar.VerticalAlignment = VerticalAlignment.Top;
            FourthStar.Name = "4";

            // Пятая звезда
            TextBlock FivesStar = new TextBlock();
            FivesStar.Text = "☆";
            FivesStar.FontSize = 30;
            FivesStar.HorizontalAlignment = HorizontalAlignment.Left;
            FivesStar.VerticalAlignment = VerticalAlignment.Top;
            FivesStar.Name = "5";

            FirstStar.Tapped += (sender, e) => ChangeMaterialMark(sender, e, 1, FirstStar, SecondaryStar, ThirdStar, FourthStar, FivesStar);
            SecondaryStar.Tapped += (sender, e) => ChangeMaterialMark(sender, e, 2, FirstStar, SecondaryStar, ThirdStar, FourthStar, FivesStar);
            ThirdStar.Tapped += (sender, e) => ChangeMaterialMark(sender, e, 3, FirstStar, SecondaryStar, ThirdStar, FourthStar, FivesStar);
            FourthStar.Tapped += (sender, e) => ChangeMaterialMark(sender, e, 4, FirstStar, SecondaryStar, ThirdStar, FourthStar, FivesStar);
            FivesStar.Tapped += (sender, e) => ChangeMaterialMark(sender, e, 5, FirstStar, SecondaryStar, ThirdStar, FourthStar, FivesStar);


            // Создание кнопки ответить
            Button SendAnswerButton = new Button();
            SendAnswerButton.Content = "Отправить";
            SendAnswerButton.FontSize = 20;
            SendAnswerButton.HorizontalAlignment = HorizontalAlignment.Left;
            SendAnswerButton.VerticalAlignment = VerticalAlignment.Top;
            SendAnswerButton.Name = "SendAnswerButton";

            SendAnswerButton.Click += (sender, e) => SendAnswerAboutMaterial(sender, e, GetMaterialMark(FirstStar, SecondaryStar, ThirdStar, FourthStar, FivesStar));

            Grid containerGrid = new Grid();
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            containerGrid.Children.Add(WriteAnswerTextBox);
            containerGrid.Children.Add(SendAnswerButton);
            containerGrid.Children.Add(FirstStar);
            containerGrid.Children.Add(SecondaryStar);
            containerGrid.Children.Add(ThirdStar);
            containerGrid.Children.Add(FourthStar);
            containerGrid.Children.Add(FivesStar);

            Grid.SetRow(FirstStar, 0);
            Grid.SetColumn(FirstStar, 0);
            Grid.SetRow(SecondaryStar, 0);
            Grid.SetColumn(SecondaryStar, 1);
            Grid.SetRow(ThirdStar, 0);
            Grid.SetColumn(ThirdStar, 2);
            Grid.SetRow(FourthStar, 0);
            Grid.SetColumn(FourthStar, 3);
            Grid.SetRow(FivesStar, 0);
            Grid.SetColumn(FivesStar, 4);


            Grid.SetRow(WriteAnswerTextBox, 0);
            Grid.SetColumn(WriteAnswerTextBox, 5);
            Grid.SetRow(SendAnswerButton, 0);
            Grid.SetColumn(SendAnswerButton, 6);



            // Задание расположения Border в Grid
            Grid.SetRow(border, 0);
            Grid.SetColumn(border, 0);
            border.Child = containerGrid;
            // Добавление объекта Border в Grid
            CatalogGrid.Children.Add(border);
        }

        // Получение значения оценки в зависимости от поставленных звёзд
        public void ChangeMaterialMark(object sender, TappedRoutedEventArgs e, int star, TextBlock o, TextBlock t, TextBlock th, TextBlock f,TextBlock fi)
        {
            int Count = star;

            string b = "★";
            string b1 = "☆";
            
            o.Text = b1;
            t.Text = b1;
            th.Text = b1;
            f.Text = b1;
            fi.Text = b1;

            switch (star)
            {
                case 1:
                    o.Text = b;
                    break;
                case 2:
                    o.Text = b;
                    t.Text = b;
                    break;
                case 3:
                    o.Text = b;
                    t.Text = b;
                    th.Text = b;
                    break;
                case 4:
                    o.Text = b;
                    t.Text = b;
                    th.Text = b;
                    f.Text = b;
                    break;
                case 5:
                    o.Text = b;
                    t.Text = b;
                    th.Text = b;
                    f.Text = b;
                    fi.Text = b;
                    break;
            }
        }

        public int GetMaterialMark(TextBlock one, TextBlock two, TextBlock three, TextBlock fourth, TextBlock fives)
        {
            int result = 0;

            if(one.Text == "★")
                result++;
            if (two.Text == "★")
                result++;
            if (three.Text == "★")
                result++;
            if (fourth.Text == "★")
                result++;
            if (fives.Text == "★")
                result++;

            return result;
        }

        // Запись отзыва на товар к бд
        public void SendAnswerAboutMaterial(object sender, RoutedEventArgs e, int mark)
        {
            TextBox textBox = this.FindName("WriteAnswerTextBox") as TextBox;

            DB db = new DB();
            db.openConnection();

            var buf = DateTime.Now;
            string currentDate = buf.ToString("yyyy-MM-dd");

            using (MySqlConnection connection = db.getConnection())
            {
                string SqlCommandText = $"INSERT INTO `comments` (`id`, `materialId`, `userId`, `materialMark`, `dateOfWriting`, `commentText`, `QuantityLikeDislike`, `commentId`) VALUES (NULL, '{CatalogPage.CurrentMateriall.id}', '{SignInPage.CurrentUser.id}', '{mark}', '{currentDate}', '{textBox.Text}', '0', NULL);";
                MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                if(command.ExecuteNonQuery() == 1)
                {
                    FillData();
                }
                else
                {
                    var msg = new MessageDialog("Возникла ошибка при отправке комментария", "bm shop");
                    var res = msg.ShowAsync();
                }
            }
        }

        // Функция добавляющая ответы на отзыв в таблицу ответ
        public void CreateAnswerComment(int row, int column, Comments CurrentComment, string Weight, string Name, Grid AnswerGrid, int commentId)
        {
            var brush1 = new SolidColorBrush(Windows.UI.Colors.Black);

            // Создание нового объекта Border
            Windows.UI.Xaml.Controls.Border border = new Windows.UI.Xaml.Controls.Border();
            border.Name = CurrentComment.id.ToString();
            border.Background = brush1;
            border.HorizontalAlignment = HorizontalAlignment.Stretch;
            border.Margin = new Thickness(10, 5, 10, 15);

            // Создание Имя комментатора
            TextBlock UserName = new TextBlock();
            UserName.Text = Name;
            UserName.HorizontalAlignment = HorizontalAlignment.Left;
            UserName.VerticalAlignment = VerticalAlignment.Top;
            UserName.FontSize = 20;
            UserName.Name = "UserName";
            UserName.Margin = new Thickness(0, 0, 0, 0);

            // Создание даты публикации отзыва
            TextBlock DateOfWriting = new TextBlock();
            DateOfWriting.Text = ConvertToDate(CurrentComment.dateOfWriting);
            DateOfWriting.HorizontalAlignment = HorizontalAlignment.Left;
            DateOfWriting.VerticalAlignment = VerticalAlignment.Top;
            DateOfWriting.FontSize = 20;
            DateOfWriting.Name = "DateOfWriting";
            DateOfWriting.Margin = new Thickness(10, 0, 0, 0);

            // Создание объёма товара
            TextBlock WeightMaterial = new TextBlock();
            WeightMaterial.Text = Weight;
            WeightMaterial.HorizontalAlignment = HorizontalAlignment.Left;
            WeightMaterial.VerticalAlignment = VerticalAlignment.Top;
            WeightMaterial.FontSize = 20;
            WeightMaterial.Name = "Weight";
            WeightMaterial.Margin = new Thickness(0, 0, 0, 0);


            // Создание текста отзыва
            TextBlock CommentText = new TextBlock();
            CommentText.Text = CurrentComment.commentText;
            CommentText.HorizontalAlignment = HorizontalAlignment.Left;
            CommentText.VerticalAlignment = VerticalAlignment.Top;
            CommentText.FontSize = 20;
            CommentText.Name = "CommentText";
            CommentText.Margin = new Thickness(0, 10, 0, 0);

            // Создание кнопки лайк
            Button LikeButton = new Button();
            LikeButton.Content = "🖒";
            LikeButton.HorizontalAlignment = HorizontalAlignment.Left;
            LikeButton.VerticalAlignment = VerticalAlignment.Top;
            LikeButton.FontSize = 20;
            LikeButton.Name = "LikeButton";
            LikeButton.Margin = new Thickness(0, 0, 0, 0);

            // Создание кнопки дизлайк
            Button DislikeButton = new Button();
            DislikeButton.Content = "🖓";
            DislikeButton.HorizontalAlignment = HorizontalAlignment.Left;
            DislikeButton.VerticalAlignment = VerticalAlignment.Top;
            DislikeButton.FontSize = 20;
            DislikeButton.Name = "DislikeButton";
            DislikeButton.Margin = new Thickness(5, 0, 0, 0);

            // Создание кнопки Ответить
            Button AnswerButton = new Button();
            AnswerButton.Content = "Ответить";
            AnswerButton.HorizontalAlignment = HorizontalAlignment.Left;
            AnswerButton.VerticalAlignment = VerticalAlignment.Top;
            AnswerButton.FontSize = 20;
            AnswerButton.Name = "AnswerButton";
            AnswerButton.Margin = new Thickness(5, 0, 0, 0);

            // Создание поля для ввода ответа
            TextBox WriteAnswerTextBox = new TextBox();
            WriteAnswerTextBox.PlaceholderText = "Комментарий:";
            WriteAnswerTextBox.FontSize = 20;
            WriteAnswerTextBox.HorizontalAlignment = HorizontalAlignment.Left;
            WriteAnswerTextBox.VerticalAlignment = VerticalAlignment.Top;
            WriteAnswerTextBox.Name = "WriteAnswerTextBox";
            WriteAnswerTextBox.Visibility = Visibility.Collapsed;

            // Создание кнопки отмены написания ответа
            Button CancelWriteButton = new Button();
            CancelWriteButton.Content = "Отмена";
            CancelWriteButton.FontSize = 20;
            CancelWriteButton.HorizontalAlignment = HorizontalAlignment.Left;
            CancelWriteButton.VerticalAlignment = VerticalAlignment.Top;
            CancelWriteButton.Name = "CancelWriteButton";
            CancelWriteButton.Visibility = Visibility.Collapsed;

            // Создание кнопки ответить
            Button SendAnswerButton = new Button();
            SendAnswerButton.Content = "Отправить";
            SendAnswerButton.FontSize = 20;
            SendAnswerButton.HorizontalAlignment = HorizontalAlignment.Left;
            SendAnswerButton.VerticalAlignment = VerticalAlignment.Top;
            SendAnswerButton.Name = "SendAnswerButton";
            SendAnswerButton.Visibility = Visibility.Collapsed;

            Grid containerGrid = new Grid();
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            containerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });


            containerGrid.Children.Add(UserName);
            containerGrid.Children.Add(DateOfWriting);
            containerGrid.Children.Add(WeightMaterial);
            containerGrid.Children.Add(CommentText);
            containerGrid.Children.Add(LikeButton);
            containerGrid.Children.Add(DislikeButton);
            containerGrid.Children.Add(AnswerButton);
            containerGrid.Children.Add(WriteAnswerTextBox);
            containerGrid.Children.Add(CancelWriteButton);
            containerGrid.Children.Add(SendAnswerButton);

            // Задание расположения Border в Grid
            Grid.SetRow(WeightMaterial, 1);
            Grid.SetColumn(WeightMaterial, 0);
            Grid.SetRow(CommentText, 2);
            Grid.SetColumn(CommentText, 0);
            Grid.SetColumnSpan(CommentText, 3);
            Grid.SetRow(LikeButton, 3);
            Grid.SetColumn(LikeButton, 0);
            Grid.SetRow(DislikeButton, 3);
            Grid.SetColumn(DislikeButton, 1);
            Grid.SetRow(AnswerButton, 3);
            Grid.SetColumn(AnswerButton, 2);
            Grid.SetRow(WriteAnswerTextBox, 4);
            Grid.SetColumn(WriteAnswerTextBox, 0);
            Grid.SetRow(CancelWriteButton, 4);
            Grid.SetColumn(CancelWriteButton, 1);
            Grid.SetRow(SendAnswerButton, 4);
            Grid.SetColumn(SendAnswerButton, 2);

            Grid.SetRow(UserName, 0);
            Grid.SetColumn(UserName, 1);
            Grid.SetRow(DateOfWriting, 0);
            Grid.SetColumn(DateOfWriting, 2);


            AnswerButton.Click += (sender, e) => HideOrOpenAnswerTextBoxAndButtons(sender, e, WriteAnswerTextBox, CancelWriteButton, SendAnswerButton);

            SendAnswerButton.Click += (sender, e) => CreateAnswer(sender, e, WriteAnswerTextBox, CurrentComment, commentId);


            // Задание расположения Border в Grid
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            border.Child = containerGrid;
            // Добавление объекта Border в Grid
            AnswerGrid.Children.Add(border);
        }

        //Список ответов на комментарии
        List<Comments> AnswerComments = new List<Comments>();
        List<string> AnswerWeight = new List<string>();
        List<string> AnswerName = new List<string>();

        // Заполняющая ответы
        public void FillAnswerGrid(int commentId, TextBlock GetAnswer, Grid AnswerGrid)
        {
            AnswerComments.Clear();
            AnswerWeight.Clear();
            AnswerName.Clear();

            DB db = new DB();
            db.openConnection();

            DataTable table = new DataTable();

            using (MySqlConnection connection = db.getConnection())
            {
                string SqlCommandText = $"SELECT c.id, c.materialId, c.userId, c.materialMark, c.dateOfWriting, c.commentText, c.quantityLikeDislike, c.commentId, m.weigth, u.name FROM `comments` c JOIN `materials` m ON c.materialId = m.id JOIN `user` u ON c.userId = u.id WHERE c.commentId IS NOT NULL AND materialId = {CatalogPage.CurrentMateriall.id} AND c.commentId = {commentId} ORDER BY `quantityLikeDislike` DESC;";
                MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            db.closeConnection();

            if(table.Rows.Count == 0)
            {
                GetAnswer.Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    Comments buf = new Comments(int.Parse(row[0].ToString()), int.Parse(row[1].ToString()), int.Parse(row[2].ToString()),
                        row[3].ToString(), row[4].ToString(), row[5].ToString(), int.Parse(row[6].ToString()), row[7].ToString());
                    AnswerComments.Add(buf);
                    AnswerWeight.Add(row[8].ToString());
                    AnswerName.Add(row[9].ToString());
                }

                //Создание плашек
                int j = 0;
                int k = 0;
                for(int i = 0; i < AnswerComments.Count; i++)
                {
                    // Определение столбцов и строк
                    AnswerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    AnswerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    // Определение столбцов и строк
                    CreateAnswerComment(k, j, AnswerComments[i], AnswerWeight[i], AnswerName[i], AnswerGrid, commentId);

                    j++;
                    if (j >= 1)
                    {
                        j = 0;
                        k++;
                    }
                }   
            }
        }

        // Функция конвертации оценки в звёзды
        public string GetQuantityStar(string Mark)
        {
            string result = "";

            if(Mark.ToLower() != "" && Mark != null)
            {
                int MaterialMark = int.Parse(Mark);

                for (int i = 0; i < MaterialMark; i++)
                {
                    result += "★";
                }

                for (int i = 0; i < (5 - MaterialMark); i++)
                {
                    result += "☆";
                }

                return result;
            }
            else
            {
                return "";
            }
        }

        // Функция обрезки времени из даты
        public string ConvertToDate(string Date)
        {
            return Date.Substring(0, 10);
        }

        // Показать/Скрыть ответы
        public void HideOrOpenAnswerComment(object sender, RoutedEventArgs e, Grid AnswerGrid, TextBlock  AnswerText)
        {
            if(AnswerText.Text == "v Ответы")
            {
                AnswerGrid.Visibility = Visibility.Visible;
                AnswerText.Text = "⌃ Ответы";
            }
            else if(AnswerText.Text == "⌃ Ответы")
            {
                AnswerGrid.Visibility = Visibility.Collapsed;
                AnswerText.Text = "v Ответы";
            }
        }

        // Показать/Скрыть поле для ввода ответа и кнопки отменить и отправить
        public void HideOrOpenAnswerTextBoxAndButtons(object sender, RoutedEventArgs e, TextBox textBox, Button btn1, Button btn2)
        {
            var buf = Visibility.Collapsed;
            if (textBox.Visibility == Visibility.Collapsed)
                buf = Visibility.Visible;
            else
                buf = Visibility.Collapsed;

            textBox.Visibility = buf;
            btn1.Visibility = buf;
            btn2.Visibility = buf;
        }

        // Отправка ответа на ответ
        public void CreateAnswer(object sender, RoutedEventArgs e, TextBox textBox, Comments CurrentComment,int commentId)
        {
            if (textBox.Text != String.Empty)
            {
                DB db = new DB();
                db.openConnection();

                using (MySqlConnection connection = db.getConnection())
                {
                    var buf = DateTime.Now;
                    string currentDate = buf.ToString("yyyy-MM-dd");
                    var name = this.FindName("UserName") as TextBlock;
                    textBox.Text = $"{name.Text}, {textBox.Text}";

                    string SqlCommandText = $"INSERT INTO `comments` (`id`, `materialId`, `userId`, `materialMark`, `dateOfWriting`, `commentText`, `QuantityLikeDislike`, `commentId`) VALUES (NULL, '{CatalogPage.CurrentMateriall.id}', '{SignInPage.CurrentUser.id}', NULL, '{currentDate}', '{textBox.Text}', '0', '{commentId}');";
                    MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                    if (command.ExecuteNonQuery() >= 1)
                    {
                        textBox.Text = String.Empty;
                        var btn1 = this.FindName("SendAnswerButton") as Button;
                        var btn2 = this.FindName("CancelWriteButton") as Button;

                        HideOrOpenAnswerTextBoxAndButtons(sender, e, textBox, btn1, btn2);
                        //FillAnswerGrid(CurrentComment.id, AnswerText, AnswerGrid);
                    }
                    else
                    {
                        var msg = new MessageDialog("Произошла ошибка, не удалось отправить комментарий");
                        var res = msg.ShowAsync();
                    }
                }


                db.closeConnection();
            }
        }

        // Отправка ответа на отзыв
        public void CreateAnswer(object sender, RoutedEventArgs e, TextBox textBox, Grid AnswerGrid, TextBlock AnswerText, Comments  CurrentComment)
        {
            if(textBox.Text != String.Empty)
            {
                DB db = new DB();
                db.openConnection();

                using (MySqlConnection connection = db.getConnection())
                {
                    var buf = DateTime.Now;
                    string currentDate = buf.ToString("yyyy-MM-dd");
                    var name = this.FindName("UserName") as TextBlock;
                    textBox.Text = $"{name.Text}, {textBox.Text}";

                    string SqlCommandText = $"INSERT INTO `comments` (`id`, `materialId`, `userId`, `materialMark`, `dateOfWriting`, `commentText`, `QuantityLikeDislike`, `commentId`) VALUES (NULL, '{CatalogPage.CurrentMateriall.id}', '{SignInPage.CurrentUser.id}', NULL, '{currentDate}', '{textBox.Text}', '0', '{CurrentComment.id}');";
                    MySqlCommand command = new MySqlCommand(SqlCommandText, connection);

                    if (command.ExecuteNonQuery() >= 1)
                    {
                        textBox.Text = String.Empty;
                        var btn1 = this.FindName("SendAnswerButton") as Button;
                        var btn2 = this.FindName("CancelWriteButton") as Button;

                        HideOrOpenAnswerTextBoxAndButtons(sender, e, textBox, btn1, btn2);
                        FillAnswerGrid(CurrentComment.id, AnswerText, AnswerGrid);
                    }
                    else
                    {
                        var msg = new MessageDialog("Произошла ошибка, не удалось отправить комментарий");
                        var res = msg.ShowAsync();
                    }
                }


                db.closeConnection();
            }
        }

        // Менять статус кнопки на не рабочий если текстбокс без текста и когда появляется текст делать его кликабельным
        public void EditStatusSendAnswerButton()
        {

        }
    }
}
