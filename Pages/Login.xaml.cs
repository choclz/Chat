using System.Windows;
using System.Windows.Controls;

namespace ClientChat
{

    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void AuthUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userLogin.Text.ToLower()) ){ MessageBox.Show("Логин пользователя не задан!", "Ошибка авторизации"); return; }
            if (string.IsNullOrWhiteSpace(userPassword.Password)) { MessageBox.Show("Пароль пользователя не задан!", "Ошибка авторизации"); return; }
            if (!Connector.IsUserExist(userLogin.Text)) { MessageBox.Show("Такого логина не существует в системе!", "Ошибка авторизации"); return; }
            if (!Connector.CheckPass(userPassword.Password, userLogin.Text)) { MessageBox.Show("Пароль пользователя неверный!", "Ошибка авторизации"); return; }
            UserData.UserLogin = userLogin.Text.ToLower();
            UserData.UserId = Connector.GetUserId(UserData.UserLogin);
            Manager.MainFrame.Navigate(new MessagePage());
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CreateNewUser());
        }
    }
}
