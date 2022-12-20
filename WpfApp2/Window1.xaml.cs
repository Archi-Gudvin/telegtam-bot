using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WpfApp2;


namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        TelegramMessageClient client;

        [Obsolete]
        public Window1()
        {
            InitializeComponent();

            client = new TelegramMessageClient(this);

            logList.ItemsSource = client.BotMessageLog;
        }

        private void btnMsgSendClick(object sender, RoutedEventArgs e)
        {

            client.SendMessage(txtMsgSend.Text, TargetSend.Text);
        }
    }
}
