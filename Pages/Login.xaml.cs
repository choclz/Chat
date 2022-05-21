using System;
using System.IO;
using System.Threading.Tasks;
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

        private async void AuthUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userLogin.Text.ToLower()) ){ MessageBox.Show("Логин пользователя не задан!", "Ошибка авторизации"); return; }
            if (string.IsNullOrWhiteSpace(userPassword.Password)) { MessageBox.Show("Пароль пользователя не задан!", "Ошибка авторизации"); return; }
            SelectedServer.Content = "Попытка подключения к серверу...";
            AuthUser.IsEnabled = false;
            await Task.Run(()=>{
                if (UserData.ServerAvailable == false)
                {
                    if (Connector.CheckServer() == 1) { UserData.ServerAvailable = true; }
                    else
                    {
                        if (MessageBox.Show("В работе главного сервера наблюдаются сбои. Использовать локальный сервер?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            string line;
                            try
                            {
                                StreamReader sr = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "SERVER.txt"));
                                line = sr.ReadLine();
                                sr.Close();
                            }
                            catch
                            {
                                line = @".\SQLEXPRESS";
                            }
                            if (Connector.ChangeServer(line) == 1)
                            {
                                MessageBox.Show("Подключение установлено");
                                UserData.ServerAvailable = true;

                            }
                            else { MessageBox.Show("Попробуйте позже"); UserData.ServerAvailable = false; }
                        }
                        else
                        {
                            MessageBox.Show("Попробуйте подключиться позже");
                            UserData.ServerAvailable = false;
                            return;
                        }
                    }
                }
            });
            AuthUser.IsEnabled = true;
            if (!UserData.ServerAvailable) return;
            SelectedServer.Content = "Cервер в сети";
            if (!Connector.IsUserExist(userLogin.Text)) { MessageBox.Show("Такого логина не существует в системе!", "Ошибка авторизации"); return; }
            if (!Connector.CheckPass(userPassword.Password, userLogin.Text)) { MessageBox.Show("Пароль пользователя неверный!", "Ошибка авторизации"); return; }
            UserData.UserLogin = userLogin.Text.ToLower();
            UserData.UserId = Connector.GetUserId(UserData.UserLogin);
            Manager.MainFrame.Navigate(new MessagePage());
        }
        
        private async void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            CreateUser.IsEnabled = false;
            await Task.Run(() =>
            {
                if (UserData.ServerAvailable == false)
                {
                    if (Connector.CheckServer() == 1) { UserData.ServerAvailable = true; }
                    else
                    {
                        if (MessageBox.Show("В работе главного сервера наблюдаются сбои. Использовать локальный сервер?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            if (Connector.ChangeServer(@".\SQLEXPRESS") == 1)
                            {
                                MessageBox.Show("Подключение установлено");
                                UserData.ServerAvailable = true;

                            }
                            else { MessageBox.Show("Попробуйте позже"); UserData.ServerAvailable = false; }
                        }
                        else
                        {
                            MessageBox.Show("Попробуйте подключиться позже");
                            UserData.ServerAvailable = false;
                        }
                    }
                }
            });
            CreateUser.IsEnabled = true;
            if (!UserData.ServerAvailable) return;
            Manager.MainFrame.Navigate(new CreateNewUser());
        }
    }
}
