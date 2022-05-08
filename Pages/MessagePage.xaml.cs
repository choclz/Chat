using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.Entity;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace ClientChat
{
    /// <summary>
    /// Логика взаимодействия для MessagePage.xaml
    /// </summary>
    public partial class MessagePage : Page
    {
        List<Chats> MessageGroup = Connector.GetChats(UserData.UserId);
        public MessagePage()
        {
            InitializeComponent();
            UsersList.ItemsSource = MessageGroup;
            ChatView.Navigate(new Pages.VoidPage());
            Manager.MessagePart = ChatView;
        }

        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Chats currentChat = (sender as ListView).SelectedItem as Chats;
            if (currentChat == null) return;
            ChatView.Navigate(new Pages.ChatViewPage(currentChat));
        }

        private void AddChatBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.AddNewChat());
        }
    }
}
