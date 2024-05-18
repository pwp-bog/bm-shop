//using bm_shop;
//using System;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Media;

//namespace YourNamespace.Converters
//{
//    public class ThemeColorConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, string language)
//        {
//            string darkColor = "#191919";
//            string lightColor = "#E9EFFD";

//            var isDarkTheme = ((App)Windows.UI.Xaml.Application.Current).RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Dark;
//            var colorString = isDarkTheme ? darkColor : lightColor;

//            return new SolidColorBrush((Windows.UI.Color)Windows.UI.Xaml.Media.ColorConverter.ConvertFromString(colorString));
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, string language)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}