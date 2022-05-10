using System;
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
        public TasksList(int ReqId)
        {
            InitializeComponent();
            this.ReqId = ReqId;
            Tasks = Connector._context.Tasks.Where(p => p.ReqId == ReqId).Include(p=>p.Requests).ToList();
            TaskDG.ItemsSource = Tasks;
        }

        private void AddEditTask_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new AddEditTask((sender as Button).DataContext as Tasks, ReqId));
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
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
            if (currentTask.Requests.customer == UserData.UserId) return;
            UserTask _current = currentTask.UserTask.Where(p => p.UserId == UserData.UserId).First();
            Manager.MessagePart.Navigate(new Pages.AnswerPage(_current));
        }
    }
}
