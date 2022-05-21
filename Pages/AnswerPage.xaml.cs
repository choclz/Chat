using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AnswerPage.xaml
    /// </summary>
    public partial class AnswerPage : Page
    {
        UserTask _current;
        byte[] file;
        bool fileAdded = false;
        string Error;
        public AnswerPage(UserTask current)
        {
            InitializeComponent();
            _current = current;
            if (current.Tasks.NeedFile)
            {
                PlaceForFile.Visibility = Visibility.Visible;
                if (current.FileId != null)
                {
                    FileName.Content = "Ранее уже был загружен файл - " + _current.TaskFiles.Name;
                }
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
            int id;
            if (TaskReady.Visibility == Visibility)
            {
                if (TaskReady.IsChecked == true)
                {
                    _current.status = 2;
                    Connector.Save(out Error);
                    MessageBox.Show(Error);
                    Connector.SendMessage(_current.Tasks.Requests.RequestFrom, UserData.UserLogin, $"У задачи {_current.Tasks.TaskName} обновлён статус: выполнено.", out Error, out id);
                }
                else
                {
                    _current.status = 1;
                    Connector.Save(out Error);
                    MessageBox.Show(Error);
                    Connector.SendMessage(_current.Tasks.Requests.RequestFrom, UserData.UserLogin, $"У задачи {_current.Tasks.TaskName} обновлён статус: не выполнено.", out Error, out id);
                }
                if (Error == null) MessageBox.Show("Задача обработана!");
            }
            else
            {
                if (fileAdded)
                {
                    _current.TaskFiles = new TaskFiles() { File = file, Name = FileName.Content.ToString() };
                    _current.status = 2;
                    Connector.Save(out Error);
                    MessageBox.Show(Error);
                    Connector.SendMessage(_current.Tasks.Requests.RequestFrom, UserData.UserLogin, $"У задачи {_current.Tasks.TaskName} обновлён статус: добавлен файл.", out Error, out id);
                }
            }
            Manager.MessagePartBack();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog OPF = new System.Windows.Forms.OpenFileDialog())
            {
                OPF.Filter = "Документы .docx|*.docx|Старые документы ворд|*.doc|Файлы pdf|*.pdf";
                if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(OPF.FileName, FileMode.Open))
                    {
                        file = new byte[fs.Length];
                        fs.Read(file, 0, file.Length);
                        fileAdded = true;
                        FileName.Content = OPF.SafeFileName;
                    }
                }
            }
        }
    }
}