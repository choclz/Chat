using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Input;

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
            admin = Connector.RequestAdmin(ReqId, UserData.UserId);
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
            if (currentTask == null) return;
            if (currentTask.Requests.customer == UserData.UserId && currentTask.NeedFile == true)
            {
                Manager.MessagePart.Navigate(new Pages.AnswersList(currentTask.id));
                return;
            }
            if (currentTask.Requests.customer == UserData.UserId) return;
            UserTask _current = currentTask.UserTask.Where(p => p.UserId == UserData.UserId).First();
            Manager.MessagePart.Navigate(new Pages.AnswerPage(_current));
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!admin) return;
            var TaskForRemoving = TaskDG.SelectedItems.Cast<Tasks>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {TaskForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Connector.RemoveTasks(TaskForRemoving);
                Connector.Save(out string Error);
                MessageBox.Show(Error);
                Tasks = Connector.GetTasks(ReqId);
                TaskDG.ItemsSource = Tasks;
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
                    Tasks = Connector.GetTasks(ReqId);
                    TaskDG.ItemsSource = Tasks;
                }
            }
        }
    }
}
