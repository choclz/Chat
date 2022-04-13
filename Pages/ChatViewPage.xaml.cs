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
            var messageList = Connector.GetMessagesFromChat(chat.id);
            _currentChat = chat;
            foreach (var t in messageList)
            {
                messages.Add(new Message(
                    t.Users.nickname,
                    t.text,
                    t.date,
                    t.from == 1003
                    )
                );
            }
            Chat_id.ItemsSource = messages;
            MessageUpdater(chat.id, Chat_id);
        }

        private static async Task MessageUpdater(int chatID, ListView _currentLV)
        {
            while (true)
            {
                int nowCount = messages.Count();
                var messageList = Connector.GetMessagesFromChat(chatID).Skip(nowCount);
                if (messageList.Count() != 0)
                {
                    foreach (var t in messageList)
                    {
                        messages.Add(new Message(
                            t.Users.nickname,
                            t.text,
                            t.date,
                            t.from == 1003
                            )
                        );
                    }
                    _currentLV.Items.Refresh();
                }
                await Task.Delay(3000);
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            messages.Add(new Message(UserData.UserLogin, NewMessage.Text, DateTime.Now, true));
            string Err;
            UserData.UserLogin = "Choclz4";
            Connector.SendMessage(_currentChat.id, UserData.UserLogin, NewMessage.Text, false, out Err);
            Connector._context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            Chat_id.Items.Refresh();
        }
    }
}
