using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientChat.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddNewChat.xaml
    /// </summary>
    public partial class AddNewChat : Page
    {
        List<Users> users = new List<Users>();
        List<Users> AllUsers;
        string Errors;
        List<Users> Selected;
        public AddNewChat()
        {
            InitializeComponent();
            AllUsers = Connector.GetUsers(out Errors);
            Users user = Connector.GetUser(UserData.UserLogin);
            AllUsers.Remove(user);
            if (Errors != null)
            {
                MessageBox.Show(Errors);
            }
            Selected = AllUsers;
            UsersToAddLV.ItemsSource = Selected;
            UsersToDelLV.ItemsSource = users;
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            users.Add((sender as Button).DataContext as Users);
            AllUsers.Remove((sender as Button).DataContext as Users);
            Selected = AllUsers;
            ChatName.Clear();
            UsersToAddLV.ItemsSource = Selected;
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
            if (Connector.CreateChat(UserData.UserLogin, users.Select(p => p.nickname.ToString()).ToArray(), ChatName.Text, out string Errors) == 1)
            {
                Manager.MessagePartBack();
            }
            MessageBox.Show(Errors);
        }

        private void User_KeyDown(object sender, KeyEventArgs e)
        {
            Selected = AllUsers.Where(p => p.nickname.Contains(User.Text.ToLower())).ToList();
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

        private void UsersToAddLV_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
