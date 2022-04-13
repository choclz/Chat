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
        private static List<Message> messages = new List<Message>();
        private static Chats _currentChat;
        public ChatViewPage(Chats chat)
        {
            InitializeComponent();
            messages.Clear();
            Chat_id.Items.Refresh();
            messages = Connector.GetMessagesFromChat(chat.id, UserData.UserId);
            _currentChat = chat;
            Chat_id.ItemsSource = messages;
            MessageUpdater(chat.id, Chat_id);
        }

        private static async Task MessageUpdater(int chatID, ListView _currentLV)
        {
            while (true)
            {
                await Task.Delay(3000);
                int nowCount = messages.Count();
                List<Message> Newmessages = Connector.GetMessagesFromChat(chatID, UserData.UserId).Skip(nowCount).ToList();
                Console.WriteLine(nowCount + " " + Newmessages.Count());
                if (Newmessages.Count() != 0)
                {
                    Console.WriteLine("bib");
                }
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            string Err;
            int id = -1;
            UserData.UserLogin = "Choclz4";
            Connector.SendMessage(_currentChat.id, UserData.UserLogin, NewMessage.Text, false, out Err, out id);
            messages.Add(new Message(id,UserData.UserLogin, NewMessage.Text, DateTime.Now, true));
            Chat_id.Items.Refresh();
        }
    }
}
