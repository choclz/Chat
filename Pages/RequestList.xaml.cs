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
    /// Логика взаимодействия для Tasks.xaml
    /// </summary>
    public partial class RequestList : Page
    {
        int chatId;
        List<Requests> req;
        public RequestList(int chatId)
        {
            InitializeComponent();
            this.chatId = chatId;
            req = MessengerEntities.GetContext().Requests.Where(p => p.RequestFrom == chatId).ToList();
            RequestDG.ItemsSource = req;
        }

        private void TasksFromYou_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Uid == "0")
            {
                RequestDG.ItemsSource = req.Where(p => p.customer == UserData.UserId);
                return;
            }
            RequestDG.ItemsSource = req.Where(p => p.customer != UserData.UserId);
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new AddEditRequest(null, chatId, false));
        }

        private void AddRequestBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePart.Navigate(new AddEditRequest((sender as Button).DataContext as Requests, chatId, TasksFromYou.IsChecked.Value));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RequestDG.ItemsSource = req.Where(p => p.customer == UserData.UserId);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }
    }
}
