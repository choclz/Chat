using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfileInfo.xaml
    /// </summary>
    public partial class ProfileInfo : Page
    {
        List<Roles> UsersRoles = Connector.GetRoles();
        Users user = new Users();
        public ProfileInfo(int userId)
        {
            InitializeComponent();    
            if (userId == -1)
            {
                userId = UserData.UserId;
                CanEdit.Visibility = Visibility.Hidden;
            }
            user = Connector.GetUser(userId);
            Role.ItemsSource = UsersRoles;
            DataContext = user;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }

        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog OPF = new System.Windows.Forms.OpenFileDialog())
            {
                OPF.Filter = "Картинки PNG|*.png|Картинки jpg|*.jpg";
                if (OPF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    byte[] imageData;
                    using (FileStream fs = new FileStream(OPF.FileName, FileMode.Open))
                    {
                        imageData = new byte[fs.Length];
                        fs.Read(imageData, 0, imageData.Length);
                        user.Photo = imageData;
                    }
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Connector.Save(out string Error);
            MessageBox.Show(Error);
            Manager.MessagePartBack();
        }

        private void DelPhoto_Click(object sender, RoutedEventArgs e)
        {
            user.Photo = null;
        }
    }
}
