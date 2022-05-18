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

namespace ClientChat
{
    /// <summary>
    /// Логика взаимодействия для CreateNewUser.xaml
    /// </summary>
    public partial class CreateNewUser : Page
    {
        public CreateNewUser()
        {
            InitializeComponent();
            Roles.ItemsSource = Connector.GetRoles().Where(p => p.id < 4 && p.id > 1).ToList();
            Roles.SelectedIndex = 0;
        }

        private void CreateUSer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserLogin.Text) || string.IsNullOrWhiteSpace(FName.Text) || string.IsNullOrWhiteSpace(SName.Text) || string.IsNullOrWhiteSpace(UserPassword.Text))
            {
                MessageBox.Show("Одно из полей не задано, проверьте правильность введённых данных!", "Ошибка регистрации");
                return;
            }
            if (Connector.IsUserExist(UserLogin.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует, пожалуйста, проверьте входные данные!");
                return;
            }
            string Errors;
            if (Connector.AddUser(UserLogin.Text, FName.Text, SName.Text, ThName.Text, UserPassword.Text, (Roles.SelectedItem as Roles).id, out Errors) == -1)
            {
                MessageBox.Show(Errors, "Ошибка регистрации нового пользователя!");
                return;
            }
            MessageBox.Show("Пользователь успешно создан!", "Успешная регистрация в системе");
            Manager.MainFrame.Navigate(new Login());
        }
    }
}
