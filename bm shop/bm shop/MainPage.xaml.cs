﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace bm_shop
{
    /// <summary>
      /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
      /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationViewControl.BackRequested += NavigationView_BackRequested;
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(MainPage));
            }
        }



        private NavigationViewItem _currentButton;
        public static string CategoryName;

        private void Level1Button_Click(object sender, RoutedEventArgs e)
        {
            _currentButton = sender as NavigationViewItem;
            ShowMenuFlyout();
        }

        private void ShowMenuFlyout()
        {
            MenuFlyout menuFlyout = new MenuFlyout();

            // Добавление элементов второго уровня для каждого раздела
            AddSubMenuItems(menuFlyout, "Лако-красочные материалы", new string[] { "Грунтовки", "Краски", "Лаки", "Растворители", "Грунт-эмаль 3в1", "Пропитки" });
            AddSubMenuItems(menuFlyout, "Строительные материалы", new string[] { "Общестроительные материалы", "Стеновые и фасадные материалы", "Тепло-изоляционные материалы" });
            AddSubMenuItems(menuFlyout, "Крепежные изделия", new string[] { "Саморезы по металлу", "Дюбель-гвозди", "Саморезы по дереву", "Мебельный крепеж" });
            AddSubMenuItems(menuFlyout, "Металлопрокат", new string[] { "Арматура", "Профильная труба" });

            menuFlyout.ShowAt(_currentButton);
        }

        private void AddSubMenuItems(MenuFlyout menuFlyout, string header, string[] items)
        {
            MenuFlyoutSubItem subItem = new MenuFlyoutSubItem { Text = header };

            foreach (string itemText in items)
            {
                MenuFlyoutItem item = new MenuFlyoutItem { Text = itemText };
                item.Click += Item_Click;
                subItem.Items.Add(item);
            }

            menuFlyout.Items.Add(subItem);
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem clickedItem = sender as MenuFlyoutItem;
            _currentButton.Content = clickedItem.Text;

            // Добавьте здесь другие действия, которые вам нужно выполнить после изменения названия
            CategoryName = clickedItem.Text;
            ContentFrame.Navigate(typeof(CatalogPage));
        }

    }
}
