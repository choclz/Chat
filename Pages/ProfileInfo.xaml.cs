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
    /// Логика взаимодействия для ProfileInfo.xaml
    /// </summary>
    public partial class ProfileInfo : Page
    {
        List<Roles> UsersRoles = Connector._context.Roles.ToList();
        Users user = new Users();
        public ProfileInfo(int userId)
        {
            InitializeComponent();    
            if (userId == -1)
            {
                userId = UserData.UserId;
                CanEdit.Visibility = Visibility.Hidden;
            }
            user = Connector._context.Users.Where(p => p.id == userId).First();
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
            Connector._context.SaveChanges();
            Manager.MessagePartBack();
        }

        private void DelPhoto_Click(object sender, RoutedEventArgs e)
        {
            user.Photo = null;
        }
    }
}
