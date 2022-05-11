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
        string Error;
        public MessagePage()
        {
            InitializeComponent();
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

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.ProfileInfo(-1));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Connector.Update(out Error);
                {
                    if (Error != null)
                    {
                        MessageBox.Show(Error);
                        return;
                    }
                    UsersList.ItemsSource = Connector.GetChats(UserData.UserId).ToList();
                    ProfInfo.DataContext = Connector._context.Users.Where(p => p.id == UserData.UserId).First();
                }
            }
        }

        private void UpdateInfo(object sender, RoutedEventArgs e)
        {
            Connector.Update(out Error);
            {
                if (Error != null)
                {
                    MessageBox.Show(Error);
                    return;
                }
                UsersList.ItemsSource = Connector.GetChats(UserData.UserId).ToList();
                ProfInfo.DataContext = Connector._context.Users.Where(p => p.id == UserData.UserId).First();
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            Chats currentChat = (sender as Button).DataContext as Chats;
            if (MessageBox.Show("Вы уверены, что хотите удалть данный диалог?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Connector._context.Chats.Remove(currentChat);
                if (Connector.Save(out Error) == 1)
                {
                    MessageBox.Show("Чат успешно удалён!");
                    UsersList.ItemsSource = Connector.GetChats(UserData.UserId).ToList();
                    Manager.MessagePart.Navigate(new Pages.VoidPage());
                    return;
                }
                MessageBox.Show(Error);
            }
        }
    }
}
