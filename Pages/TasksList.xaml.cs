﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.Entity;
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
    /// Логика взаимодействия для TasksList.xaml
    /// </summary>
    public partial class TasksList : Page
    {
        int ReqId;
        List<Tasks> Tasks;
        string Error;
        bool admin = false;
        public TasksList(int ReqId)
        {
            InitializeComponent();
            this.ReqId = ReqId;
            if (Connector._context.Requests.Where(p=>p.id == ReqId).First().customer == UserData.UserId)
            {
                admin = true;
            }
        }

        private void AddEditTask_Click(object sender, RoutedEventArgs e)
        {
            if (!admin) return;
            Manager.MessagePart.Navigate(new AddEditTask((sender as Button).DataContext as Tasks, ReqId));
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!admin) return;
            Manager.MessagePart.Navigate(new AddEditTask(null, ReqId));
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }

        private void TaskDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Tasks currentTask = (sender as DataGrid).SelectedItem as Tasks;
            if (currentTask == null || !currentTask.NeedFile) return;
            if (currentTask.Requests.customer == UserData.UserId)
            {
                Manager.MessagePart.Navigate(new Pages.AnswersList(currentTask.id));
                return;
            }
            UserTask _current = currentTask.UserTask.Where(p => p.UserId == UserData.UserId).First();
            Manager.MessagePart.Navigate(new Pages.AnswerPage(_current));
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!admin) return;
            var TaskForRemoving = TaskDG.SelectedItems.Cast<Tasks>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {TaskForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    Connector._context.Tasks.RemoveRange(TaskForRemoving);
                    Connector._context.SaveChanges();
                    MessageBox.Show("Данные удалены!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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
                    Tasks = Connector._context.Tasks.Where(p => p.ReqId == ReqId).Include(p => p.Requests).ToList();
                    TaskDG.ItemsSource = Tasks;
                }
            }
        }
    }
}
