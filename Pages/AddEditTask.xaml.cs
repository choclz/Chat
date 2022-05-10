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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditTask.xaml
    /// </summary>
    public partial class AddEditTask : Page
    {
        int ReqId;
        List<Tasks> tasks = Connector._context.Tasks.ToList();
        Tasks task = new Tasks();

        public AddEditTask(Tasks tasks, int ReqId)
        {
            InitializeComponent();
            if (tasks != null) task = tasks;
            DataContext = task;
            if (this.task.id == 0)
            {
                task.ReqId = ReqId;
                task.NeedFile = false;
                return;
            }
        }

        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            string Err;
            int a;
            if (task.id == 0)
            {
                Connector._context.Tasks.Add(task);
                List<int> UsersID = Connector._context.UsersChats.Where(p => p.ChatId == task.Requests.RequestFrom && p.UserId != UserData.UserId).Select(p => p.UserId).ToList();
                foreach (int user in UsersID)
                {
                    Connector._context.UserTask.Add(new UserTask(){ UserId = user, TaskId = task.id, status = 1, Comment = "Ответ отсутствует" });
                }
                Connector.SendMessage(task.Requests.RequestFrom, UserData.UserLogin, "В данном диалоге создана новая задача!", false, out Err, out a);
                MessengerEntities.GetContext().SaveChanges();
                Manager.MessagePart.GoBack();
                return;
            }
            Connector.SendMessage(task.Requests.RequestFrom, UserData.UserLogin, $"В данном диалоге обновлена задача - \"{task.TaskName}\"! ", false, out Err, out a);
            MessengerEntities.GetContext().SaveChanges();
            Manager.MessagePart.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.GoBack();
        }
    }
}
