using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Data.Entity;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AnswerPage.xaml
    /// </summary>
    public partial class AnswerPage : Page
    {
        int TaskId;
        UserTask _current;
        public AnswerPage(UserTask current)
        {
            InitializeComponent();
            _current = current;
            if (current.Tasks.NeedFile)
            {
                PlaceForFile.Visibility = Visibility.Visible;
            }
            else
            {
                TaskReady.Visibility = Visibility.Visible;
                TaskReady.IsChecked = _current.status == 1 ? false : true;
            }
            DataContext = _current.Tasks;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            int id; string Err;
            if (TaskReady.Visibility == Visibility)
            {
                if (TaskReady.IsChecked == true)
                {
                    _current.status = 2;
                    Connector.Save();
                    Connector.SendMessage(_current.Tasks.Requests.RequestFrom, UserData.UserLogin, $"У задачи {_current.Tasks.TaskName} обновлён статус: выполнено.", false, out Err, out id);
                }
                else
                {
                    _current.status = 1;
                    Connector.Save();
                    Connector.SendMessage(_current.Tasks.Requests.RequestFrom, UserData.UserLogin, $"У задачи {_current.Tasks.TaskName} обновлён статус: не выполнено.", false, out Err, out id);
                }
                if (id == -1)
                {
                    MessageBox.Show(Err.ToString(), "Ошибка отправки, попробуйте позже!");
                }
                else
                {
                    MessageBox.Show("Задача обработана!");
                    Manager.MessagePartBack();
                }
            }
        }
    }
}