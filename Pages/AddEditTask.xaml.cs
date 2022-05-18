using System;
using System.Windows;
using System.Windows.Controls;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditTask.xaml
    /// </summary>
    public partial class AddEditTask : Page
    {
        string Error;
        Tasks task = new Tasks();

        public AddEditTask(Tasks tasks, int ReqId)
        {
            InitializeComponent();
            if (tasks != null) task = tasks;
            DataContext = task;
            if (task.id == 0)
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
                task.StartTime = StartTime.SelectedDate.Value;
                if (Connector.AddTask(task, out Error) == -1) { MessageBox.Show(Error); return; }
                if (Connector.GenerateTasks(task, UserData.UserId) == -1) { MessageBox.Show("Ошибка распределения заявок между пользователями. Попробуйте позже!"); return; }
                if (Connector.SendMessage(task.Requests.RequestFrom, UserData.UserLogin, $"В данном диалоге создана новая задача! \nЗаявка:{task.Requests.name}\nЗадача:{task.TaskName}", out Err, out a) == -1)
                {
                    MessageBox.Show(Error);
                    return;
                }
                Connector.Save(out Error);
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
            Connector.Save(out Error); MessageBox.Show(Error);
            Connector.SendMessage(task.Requests.RequestFrom, UserData.UserLogin, $"В данном диалоге обновлена задача - \nЗаявка:{task.Requests.name} \nЗадача:{task.TaskName}", out Err, out a);
            Connector.Save(out Error);
            Manager.MessagePart.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.GoBack();
        }
    }
}
