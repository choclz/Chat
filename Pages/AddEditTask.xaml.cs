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
        string Error;
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
                if (Connector.AddTask(task, out Error) == -1) { MessageBox.Show(Error); return; }
                List<int> UsersID = Connector._context.UsersChats.Where(p => p.ChatId == task.Requests.RequestFrom && p.UserId != UserData.UserId).Select(p => p.UserId).ToList();
                foreach (int user in UsersID)
                {
                    Connector._context.UserTask.Add(new UserTask(){ UserId = user, TaskId = task.id, status = 1, Comment = "Ответ отсутствует" });
                }
                if (Connector.SendMessage(task.Requests.RequestFrom, UserData.UserLogin, "В данном диалоге создана новая задача!", out Err, out a) == -1)
                {
                    MessageBox.Show(Error);
                    return;
                }
                Connector.Save(out Error); MessageBox.Show(Error);
                Manager.MessagePart.GoBack();
                return;
            }
            if (task.StartTime < DateTime.Now.AddDays(-1)) { MessageBox.Show("Дата начала должна быть больше текущей даты!"); return; }
            if (task.StartTime < task.Requests.StartTime) { MessageBox.Show("Дата начала выполнения задачи не может быть меньше даты начала выполнения заявки!"); return; }
            if (task.EndTime == null) { MessageBox.Show("Не задана дата окончания выполнения!"); return; }
            if (task.EndTime > task.Requests.EndTime) { MessageBox.Show("Дата окончания выполнения задачи не может быть больше даты окончания заявки!"); return; }
            if (task.EndTime < task.StartTime) { MessageBox.Show("Дата окончания выполнения должна быть больше даты старта!"); return; }
            if (string.IsNullOrWhiteSpace(task.TaskName)) { MessageBox.Show(Error = "Имя задачи не задано!"); return; }
            if (string.IsNullOrWhiteSpace(task.Description)) { MessageBox.Show(Error = "Описание задачи не задано!"); return; }
            Connector.SendMessage(task.Requests.RequestFrom, UserData.UserLogin, $"В данном диалоге обновлена задача - \"{task.TaskName}\"! ", out Err, out a);
            Connector.Save(out Error); MessageBox.Show(Error);
            Manager.MessagePart.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.GoBack();
        }
    }
}
