using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AnswersList.xaml
    /// </summary>
    public partial class AnswersList : Page
    {
        int taskId;
        string Error;
        public AnswersList(int id)
        {
            InitializeComponent();
            taskId = id;
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            var FilesForRemoving = Files.SelectedItems.Cast<UserTask>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {FilesForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    Connector._context.UserTask.RemoveRange(FilesForRemoving);
                    Connector._context.SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    var files = Connector._context.UserTask.Where(p => p.TaskId == taskId && p.FileId != null).ToList();
                    Files.ItemsSource = files;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            var path = ((sender as Button).DataContext as UserTask).TaskFiles;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllBytes(System.IO.Path.Combine(folderBrowser.SelectedPath, path.Name), path.File);
                MessageBox.Show("Файл сохранен!");
                return;
            }
            MessageBox.Show("Операция отменена!");
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
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
                    var files = Connector._context.UserTask.Where(p => p.TaskId == taskId && p.FileId != null).ToList();
                    Files.ItemsSource = files;
                }
            }
        }

        private void Files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void remakeBtn_Click(object sender, RoutedEventArgs e)
        {
            UserTask tsk = (sender as Button).DataContext as UserTask;
            if (tsk.status == 1)
            {
                MessageBox.Show("Задание ещё не выполнено или в процессе исправления!");
                return;
            }
            tsk.status = 1;
            Connector.SendMessage(tsk.Tasks.Requests.RequestFrom, UserData.UserLogin, $"{tsk.Users.nickname}, задача: {tsk.Tasks.TaskName} возвращена с комментарием - {tsk.Comment}.", out Error, out taskId);
            Connector.Save(out Error);
            MessageBox.Show(Error);
            var files = Connector._context.UserTask.Where(p => p.TaskId == taskId && p.FileId != null).ToList();
            Files.ItemsSource = files;
        }

        private void Comment_KeyUp(object sender, KeyEventArgs e)
        {
            (Files.SelectedItem as UserTask).Comment = (sender as TextBox).Text;
        }
    }
}
