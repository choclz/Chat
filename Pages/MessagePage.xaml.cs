﻿using System;
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
        public MessagePage()
        {
            InitializeComponent();
            List<Chats> MessageGroup = Connector.GetChats(UserData.UserId);
            UsersList.ItemsSource = Connector.GetChats(1003);
        }

        private void UsersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Chats currentChat = (sender as ListView).SelectedItem as Chats;
            if (currentChat == null) return;
            ChatView.Navigate(new Pages.ChatViewPage(currentChat));
        }
       
    }
}
