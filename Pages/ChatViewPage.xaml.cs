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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChatViewPage.xaml
    /// </summary>
    public partial class ChatViewPage : Page
    {
        static CancellationTokenSource source = new CancellationTokenSource();
        static CancellationToken token = source.Token;
        static List<Message> messages = new List<Message>();
        Chats _currentChat;
        int chatID;
        ListView _messages;
        Timer timer;
        public ChatViewPage(Chats chat)
        {
            InitializeComponent();
            _currentChat = chat;
        }

        private void UpdateMessages(object o)
        {
            int messagesCount = Chat_id.Items.Count;
            List<Message> Newmessages = Connector.GetMessagesFromChat(chatID, UserData.UserId).Skip(messagesCount).ToList();
            if (Newmessages.Count() != 0)
            {
                foreach (Message msg in Newmessages)
                {
                    messages.Add(msg);
                    Dispatcher.Invoke(() => _messages.Items.Refresh());
                    Chat_id.ScrollIntoView(Chat_id.Items[--messagesCount]);
                }
            }
            Console.WriteLine("Выдано: " + Chat_id.Items.Count + " Получено: " + Newmessages.Count);
        }


        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string Err;
            int id = -1;
            UserData.UserLogin = UserData.UserLogin;
            Connector.SendMessage(_currentChat.id, UserData.UserLogin, NewMessage.Text, false, out Err, out id);
            messages.Add(new Message(id,UserData.UserLogin, NewMessage.Text, DateTime.Now, true));
            Chat_id.Items.Refresh();
            NewMessage.Clear();
            int messCount = Chat_id.Items.Count;
            if (messCount != 0) Chat_id.ScrollIntoView(Chat_id.Items[--messCount]);
        }

        private void NewTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.RequestList(_currentChat.id));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                messages.Clear();
                Chat_id.Items.Refresh();
                chatID = _currentChat.id;
                messages = Connector.GetMessagesFromChat(_currentChat.id, UserData.UserId);
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
    }
}
