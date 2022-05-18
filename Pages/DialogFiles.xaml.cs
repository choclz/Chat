using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Input;
using System.IO;
using Path = System.IO.Path;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для DialogFiles.xaml
    /// </summary>
    public partial class DialogFiles : Page
    {
        string Error;
        int id;
        List<Messages> files;
        public DialogFiles(int chatId)
        {
            InitializeComponent();
            id = chatId;
        }

        private void Files_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new AddEditFile(id, (sender as Button).DataContext as Messages));
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            var FilesForRemoving = Files.SelectedItems.Cast<Messages>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {FilesForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                try
                {
                    Connector._context.Messages.RemoveRange(FilesForRemoving);
                    Connector.Save(out Error);
                    MessageBox.Show(Error);
                    files = Connector.GetChatFiles(id);
                    Files.ItemsSource = files;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.AddEditFile(id, null));
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            var path = ((sender as Button).DataContext as Messages).MessageFiles;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllBytes(Path.Combine(folderBrowser.SelectedPath, path.Name), path.File);
                MessageBox.Show("Файл сохранен!");
                return;
            }
            MessageBox.Show("Операция прервана пользователем.");
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
                    files = Connector.GetChatFiles(id);
                    Files.ItemsSource = files;
                }
            }
        }
    }
}
