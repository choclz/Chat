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

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddNewChat.xaml
    /// </summary>
    public partial class AddNewChat : Page
    {
        List<Users> users = new List<Users>();
        List<Users> AllUsers = Connector.GetUsers();
        public AddNewChat()
        {
            InitializeComponent();
            Users user = AllUsers.Where(p => p.id == UserData.UserId).First();
            AllUsers.Remove(user);
            UsersToAddLV.ItemsSource = AllUsers;
            UsersToDelLV.ItemsSource = users;
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            users.Add((sender as Button).DataContext as Users);
            AllUsers.Remove((sender as Button).DataContext as Users);
            UsersToDelLV.Items.Refresh();
            UsersToAddLV.Items.Refresh();
        }

        private void DelFromList_Click(object sender, RoutedEventArgs e)
        {
            AllUsers.Add((sender as Button).DataContext as Users);
            users.Remove((sender as Button).DataContext as Users);
            UsersToDelLV.Items.Refresh();
            UsersToAddLV.Items.Refresh();
        }

        private void CreateChat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connector.CreateChat(UserData.UserLogin, users.Select(p => p.nickname.ToString()).ToArray(), ChatName.Text, out string Errors);
                Console.WriteLine(Errors);
                MessageBox.Show("Беседа успешно создана!");
                Manager.MessagePartBack();
            }
            catch
            {
                MessageBox.Show("Ошибка создания беседы!");
            }
        }

        private void User_KeyDown(object sender, KeyEventArgs e)
        {
            List<Users> Selected = AllUsers.Where(p => p.nickname.Contains(User.Text.ToLower())).ToList();
            UsersToAddLV.ItemsSource = Selected;
            UsersToAddLV.Items.Refresh();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }

        private void Check_Profile(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new Pages.ProfileInfo(((sender as Button).DataContext as Users).id));
        }
    }
}
