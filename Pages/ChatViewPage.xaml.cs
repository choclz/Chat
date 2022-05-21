using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChatViewPage.xaml
    /// </summary>
    public partial class ChatViewPage : Page
    {
        SoundPlayer sp = new SoundPlayer();
        static List<Message> messages = new List<Message>();
        Chats _currentChat;
        int chatID;
        ListView _messages;
        string Error;
        Timer timer;
        public ChatViewPage(Chats chat)
        {
            InitializeComponent();
            sp.Stream = Properties.Resources.zvuk;
            _currentChat = chat;
        }

        private void UpdateMessages(object o)
        {
            try
            {
                int messagesCount = Chat_id.Items.Count;
                List<Message> Newmessages = Connector.GetMessagesFromChat(chatID, UserData.UserId, messagesCount).ToList();
                if (Newmessages.Count() != 0)
                {
                    if (messagesCount != 0) sp.Play();
                    foreach (Message msg in Newmessages)
                    {
                        messages.Add(msg);
                        Dispatcher.Invoke(() => _messages.Items.Refresh());
                        Console.WriteLine("Выдано: " + Chat_id.Items.Count + " Получено: " + Newmessages.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            int t = Chat_id.Items.Count;
            if (Connector.SendMessage(_currentChat.id, UserData.UserLogin, NewMessage.Text, out Error, out id) == -1) { MessageBox.Show(Error); return; }
            NewMessage.Clear();
            if (t != 0) Chat_id.ScrollIntoView(Chat_id.Items[t - 1]);
        }

        private void NewTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.RequestList(_currentChat.id));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                messages.Clear();
                Chat_id.Items.Refresh();
                chatID = _currentChat.id;
                messages = Connector.GetMessagesFromChat(_currentChat.id, UserData.UserId, 0);
                Chat_id.ItemsSource = messages;
                _messages = Chat_id;
                timer = new Timer(UpdateMessages, null, 0, 1000);
                int messCount = Chat_id.Items.Count;
                if (messCount != 0) Chat_id.ScrollIntoView(Chat_id.Items[--messCount]);
            }
            else
            {
                timer.Change(Timeout.Infinite, 1000);
                timer.Dispose();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void Files_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.DialogFiles(_currentChat.id));
        }
    }
}
