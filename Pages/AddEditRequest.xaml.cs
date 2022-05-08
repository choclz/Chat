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
    /// Логика взаимодействия для AddEditRequest.xaml
    /// </summary>
    public partial class AddEditRequest : Page
    {
        Requests request = new Requests();
        int chatId;
        List<RequestStatus> requestStatuses = Connector._context.RequestStatus.ToList();

        public AddEditRequest(Requests requests, int chatId, bool admin)
        {
            InitializeComponent();
            request.RequestFrom = chatId;
            request.customer = UserData.UserId;
            this.chatId = chatId;
            if (requests != null) this.request = requests;
            DataContext = this.request;
            requestStatuses.Insert(0, new RequestStatus { name = "В процессе создания" });
            if (this.request.id == 0)
            {
                ReqStatus.IsEnabled = false;
                ReqStatus.ItemsSource = requestStatuses;
                return;
            }
            ReqStatus.ItemsSource = requestStatuses;
        }


        private void AddRequest_Click(object sender, RoutedEventArgs e)
        {
            string Err;
            int a;
            if (request.id == 0)
            {
                request.status = 1;
                MessengerEntities.GetContext().Requests.Add(request);
                Connector.SendMessage(chatId, UserData.UserLogin, "В данном диалоге создана новая заявка!", false, out Err, out a);
                MessengerEntities.GetContext().SaveChanges();
                Manager.MessagePart.GoBack();
                return;
            }
            Connector.SendMessage(chatId, UserData.UserLogin, $"В данном диалоге обновлена заявка - \"{request.name}\"! ", false, out Err, out a);
            MessengerEntities.GetContext().SaveChanges();
            Manager.MessagePart.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }
    }
}
