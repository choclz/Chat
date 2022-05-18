using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditFile.xaml
    /// </summary>
    public partial class AddEditFile : Page
    {
        byte[] file;
        int id;
        Messages message = new Messages();
        string Error;
        public AddEditFile(int id, Messages msg)
        {
            InitializeComponent();
            this.id = id;
            if (msg != null) message = msg;
            DataContext = message;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (message.id == 0)
            {
                if (file == null) return;
                if (Connector.AddFile(file, FileName.Content.ToString(), out int a) == -1) { MessageBox.Show("Ошибка создания файла"); return; }

                if (Connector.SendMessage(id, UserData.UserLogin, Comment.Text + "\n*Вложенный файл", out Error, out _, a) == -1)
                {
                    MessageBox.Show(Error);
                    return;
                }
            }
            Connector.Save(out Error);
            MessageBox.Show(Error);
            Manager.MessagePartBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog OPF = new System.Windows.Forms.OpenFileDialog())
            {
                OPF.Filter = "Документы .doc|*.doc|Документы .docx|*.docx|Файлы pdf|*.pdf";
                if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(OPF.FileName, FileMode.Open))
                    {
                        file = new byte[fs.Length];
                        fs.Read(file, 0, file.Length);
                        FileName.Content = OPF.SafeFileName;
                    }
                }
            }
        }
    }
}
