using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WpfApp2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WpfApp2
{

    


    class TelegramMessageClient
    {
        public static string DirectoryPath = @"..\Debug";
        
        public static string FilesPath = @"DownloadedFiles.txt";

        public static string MessagePath = @"MessageHistoty.json";

        private static string token = System.IO.File.ReadAllText("token.txt");

        private static TelegramBotClient bot = new TelegramBotClient(token);
        public ObservableCollection<MessageLog> BotMessageLog { get; set; }

        public Window1 w;

        public static List<MessageLog> messageLogs_1 = new List<MessageLog>();

       [Obsolete]

        /// <summary>
        /// Метод начала работы бота
        /// </summary>
        public TelegramMessageClient(Window1 W)
        {
            this.BotMessageLog = new ObservableCollection<MessageLog>();

            this.w = W;

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        /// <summary>
        /// Метод обработки сообщений пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private async void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            ExsistDirectory();

            if (e.Message != null)
            {
                if (e.Message.Text == "Увидеть список скачанных файлов")
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Введена команда: '{e.Message.Text}'", replyMarkup: GetButtons());
                    await PrintDirectory(e);
                }

                else if (e.Message.Text == "/start")
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Привет, вот что я умею:\n" +
                        $"- скачивать произвольные файлы,\n" +
                        $"- показывать скачанные файлы.", replyMarkup: GetButtons());
                }

                else if (e.Message.Text == "Скачать файл" ||
                    e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document ||
                    e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo ||
                    e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Video ||
                    e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio)
                {
                    await ChekingTheFileType(e);
                }

                else
                {
                    await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Выбери команду:", replyMarkup: GetButtons());
                } 

                string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

                Debug.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

                if (e.Message.Text == null) return;

                var messageText = e.Message.Text;

                //выводит сообщения пользователя в текстблок окна Window1
                w.Dispatcher.Invoke(() =>
                {
                    BotMessageLog.Add(AddMessageToList(new MessageLog
                            (DateTime.Now.ToShortTimeString(),
                            messageText,
                            e.Message.Chat.FirstName,
                            e.Message.Chat.Id)));
                   
                });
            }
        }

        /// <summary>
        /// Метод с кнопками
        /// </summary>
        /// <returns></returns>
        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton {Text = "/start"} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Скачать файл"} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Увидеть список скачанных файлов"} }
                }
            };
        }

        /// <summary>
        /// Метод загрузки файла по его типу
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [Obsolete]
        private static async Task ChekingTheFileType(MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Введена команда: '{e.Message.Text}'", replyMarkup: GetButtons());

                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Пришли мне файл");

            }

            else if (e.Message != null)
            {
                Random random = new Random();

                //скачивание документа
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
                {
                    DownLoad(e.Message.Document.FileId, e.Message.Document.FileName, e);
                }

                //скачивание аудиофайла
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio)
                {
                    DownLoad(e.Message.Audio.FileId, e.Message.Audio.FileName, e);
                }

                //скачивание фотографии
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                {
                    DownLoad(e.Message.Photo[e.Message.Photo.Length - 1].FileId, "photo" + random.Next() + ".png", e);
                }

                //скачивание видеофайла
                if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Video)
                {
                    DownLoad(e.Message.Video.FileId, "video" + random.Next() + ".mp4", e);

                }
            }
        }

        /// <summary>
        /// Метод скачивания различных файлов
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        [Obsolete]
        private static async void DownLoad(string fileId, string FileName, Telegram.Bot.Args.MessageEventArgs e)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(FileName, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Файл скачан");
            AddDownloadsToFile();
            fs.Close();

            fs.Dispose();
        }

        /// <summary>
        /// Метод создания папки-загрузки
        /// </summary>
        public static void ExsistDirectory()
        {
            if (Directory.Exists(DirectoryPath))
            {
                return;
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(DirectoryPath);
            }
        }

        /// <summary>
        /// Метод создания файлов
        /// </summary>
        public static void ExsistFiles(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                return;
            }
            else
            {
                FileStream fileStream = new FileStream(filepath, FileMode.OpenOrCreate);   
            }
        }

        /// <summary>
        /// Метод добавления названий загрузок в файл
        /// </summary>
        public static void AddDownloadsToFile()
        {
            List<string> ReaderNameFiles = new List<string>();

            //проверка файла на существование
            if (!System.IO.File.Exists(FilesPath))
            {
                FileInfo fileInfo = new FileInfo(FilesPath);
                fileInfo.Create();
            }

            DirectoryInfo directory = new DirectoryInfo(DirectoryPath);

            //переменные, которые принимают файлы по типу

            FileInfo[] photo_png = directory.GetFiles("*.png");
            FileInfo[] photo_jpg = directory.GetFiles("*.jpg");
            FileInfo[] video = directory.GetFiles("*.mp4");
            FileInfo[] audio = directory.GetFiles("*.mp3");
            FileInfo[] document_docx = directory.GetFiles("*.docx");
            FileInfo[] document_doc = directory.GetFiles("*.doc");
            FileInfo[] document_pdf = directory.GetFiles("*.pdf");
            FileInfo[] document_a4 = directory.GetFiles("*.a4");
            FileInfo[] document_zip = directory.GetFiles("*.zip");
            FileInfo[] document_rar = directory.GetFiles("*.rar");
            FileInfo[] document_fb2 = directory.GetFiles("*.fb2");

            //добавление названий файлов во временный лист
            foreach (var jpg in photo_jpg)
            {
                ReaderNameFiles.Add(jpg.Name);
            }
            foreach (var png in photo_png)
            {
                ReaderNameFiles.Add(png.Name);
            }
            foreach (var mp4 in video)
            {
                ReaderNameFiles.Add(mp4.Name);
            }
            foreach (var mp3 in audio)
            {
                ReaderNameFiles.Add(mp3.Name);
            }
            foreach (var docx in document_docx)
            {
                ReaderNameFiles.Add(docx.Name);
            }
            foreach (var doc in document_doc)
            {
                ReaderNameFiles.Add(doc.Name);
            }
            foreach (var pdf in document_pdf)
            {
                ReaderNameFiles.Add(pdf.Name);
            }
            foreach (var a4 in document_a4)
            {
                ReaderNameFiles.Add(a4.Name);
            }
            foreach (var zip in document_zip)
            {
                ReaderNameFiles.Add(zip.Name); 
            }
            foreach (var rar in document_rar)
            {
                ReaderNameFiles.Add(rar.Name);
            }
            foreach (var fb2 in document_fb2)
            {
                ReaderNameFiles.Add(fb2.Name);
            }

            //загрузка листа в файл
            System.IO.File.WriteAllLines(FilesPath, ReaderNameFiles);
        }

        /// <summary>
        /// Метод вывода названий заргуженных файлов в телеграм-чат
        /// </summary>
        /// <param name="path"></param>
        /// <param name="e"></param>
        [Obsolete]
        public static async Task PrintDirectory(Telegram.Bot.Args.MessageEventArgs e)
        {           
            DirectoryInfo directory = new DirectoryInfo(DirectoryPath);

            //переменные, которые принимают файлы по типу

            FileInfo[] photo_png = directory.GetFiles("*.png");
            FileInfo[] photo_jpg = directory.GetFiles("*.jpg");
            FileInfo[] video = directory.GetFiles("*.mp4");
            FileInfo[] audio = directory.GetFiles("*.mp3");
            FileInfo[] document_docx = directory.GetFiles("*.docx");
            FileInfo[] document_doc = directory.GetFiles("*.doc");
            FileInfo[] document_pdf = directory.GetFiles("*.pdf");
            FileInfo[] document_a4 = directory.GetFiles("*.a4");
            FileInfo[] document_zip = directory.GetFiles("*.zip");
            FileInfo[] document_rar = directory.GetFiles("*.rar");
            FileInfo[] document_fb2 = directory.GetFiles("*.fb2");

            //вывод фото
            await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Фото:");

            foreach (var png in photo_png)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{png.Name}");
            }

            foreach (var jpg in photo_jpg)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{jpg.Name}");
            }

            //вывод видео
            await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Видео:");

            foreach (var mp4 in video)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{mp4.Name}");
            }

            //вывод аудио
            await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Аудио:");

            foreach (var mp3 in audio)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{mp3.Name}");
            }

            //вывод документов разных типов
            await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Документы:");

            foreach (var docx in document_docx)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{docx.Name}");
            }

            foreach (var doc in document_doc)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{doc.Name}");
            }

            foreach (var pdf in document_pdf)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{pdf.Name}");
            }

            foreach (var a4 in document_a4)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{a4.Name}");
            }

            foreach (var zip in document_zip)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{zip.Name}");
            }

            foreach (var rar in document_rar)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{rar.Name}");
            }

            foreach (var fb2 in document_fb2)
            {
                await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{fb2.Name}");
            }

            AddDownloadsToFile();
        }

        /// <summary>
        /// Отправка сообщение пользователю из приложения
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Id"></param>
        public void SendMessage(string Text, string Id)
        {
            long id = Convert.ToInt64(Id);
            bot.SendTextMessageAsync(id, Text);
        }

        /// <summary>
        /// Метод сериализации JSON-файла
        /// </summary>
        private MessageLog AddMessageToList(MessageLog messageLog)
        {
            ExsistFiles(MessagePath);

            messageLogs_1.Add(messageLog);

            string json = JsonConvert.SerializeObject(messageLogs_1);

            System.IO.File.WriteAllText(MessagePath, json);

            return messageLog;
        }

        /// <summary>
        /// Метод десериализации JSON-файла
        /// </summary>
        /// <param name="messageLog"></param>
        /// <returns></returns>
        public static List<MessageLog> MessageHistory()
        {   
            ExsistFiles(MessagePath);

            List<MessageLog> list = new List<MessageLog>();
            string json = System.IO.File.ReadAllText(MessagePath);
            list = JsonConvert.DeserializeObject<List<MessageLog>>(json);
            return list;
        }   
    }
}
