using System;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string FilesPath = @"DownloadedFiles.txt";
        public static string MessagePath = @"MessageHistoty.json";

        public MainWindow()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Взаимодействие с кнопкой "Help for the developer"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Help(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
         "Чтобы продолжить работу с программой, необходимо занести 2 банки сгущенки разработчикам. " +
         "Вы согласны продолжить работу (по факту согласия отправляется email-уведомление об этом в IT-отдел и в бухгалтерию)?",
         "Помощь разработчикам",
         MessageBoxButton.OK,
         MessageBoxImage.Question);
        }

        /// <summary>
        /// Взаимодействие с кнопкой "Exit"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Взаимодействие с кнопкой "What I can?"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_WhatICan(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вот что я умею:\n" +
                "- Взаимодействовать с телеграм-ботом 'Oleg',\n" +
                "- Сохранять историю сообщений на устройство,\n" +
                "- Показывать список скачанных файлов из телегам-бота.\n");
        }

        [Obsolete]
        private void ButtonStartBot(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        /// <summary>
        /// Метод обработки нажати кнопки "Message history"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MessageHistory(object sender, RoutedEventArgs e)
        {
            var list = TelegramMessageClient.MessageHistory();

            string result = "";

            foreach (var item in list)
            {
                result += ($"{item.Time} {item.Id} {item.Msg} {item.FirstName}\n");
            }

            MessageBox.Show($"История сообщений:\n{result}");

        }

        /// <summary>
        /// Метод обработки нажати кнопки "See the list of downloaded files"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_DownloadedFiles(object sender, RoutedEventArgs e)
        {
            TelegramMessageClient.AddDownloadsToFile();

            string SavedNamesFiles = File.ReadAllText(FilesPath);

            MessageBox.Show($"Загруженные файлы:\n{SavedNamesFiles}");
                
        }
    }
}
