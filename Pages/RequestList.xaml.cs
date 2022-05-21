using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Input;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tasks.xaml
    /// </summary>
    public partial class RequestList : Page
    {
        int chatId;
        List<Requests> req;
        string Error;
        public RequestList(int chatId)
        {
            InitializeComponent();
            this.chatId = chatId;
        }

        private void TasksFromYou_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Uid == "0")
            {
                RequestDG.ItemsSource = req.Where(p => p.customer == UserData.UserId).ToList();
                return;
            }
            RequestDG.ItemsSource = req.Where(p => p.customer != UserData.UserId);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new AddEditRequest(null, chatId, false));
        }

        private void AddRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new AddEditRequest((sender as Button).DataContext as Requests, chatId, TasksFromYou.IsChecked.Value));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RequestDG.ItemsSource = req.Where(p => p.customer == UserData.UserId);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TasksFromYou.IsChecked == false) return;
            var RequestsForRemoving = RequestDG.SelectedItems.Cast<Requests>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {RequestsForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    Connector._context.Requests.RemoveRange(RequestsForRemoving);
                    Connector._context.SaveChanges();
                    MessageBox.Show("Данные удалены! Обновите страницу.");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RequestDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Requests currentRequest = (sender as DataGrid).SelectedItem as Requests;
            if (currentRequest == null) return;
            Manager.MessagePart.Navigate(new Pages.TasksList(currentRequest.id));
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
                    req = Connector.GetRequestsWithParam(chatId);
                    TasksFromYou.IsChecked = true;
                    RequestDG.ItemsSource = req.Where(p => p.customer == UserData.UserId).ToList();
                }
            }
        }
    }
}
