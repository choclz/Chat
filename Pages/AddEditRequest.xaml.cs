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
        string Error;
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
            int a;
            if (request.id == 0)
            {
                if (Connector.AddRequest(request, out Error) == -1) { MessageBox.Show(Error); return; }
                Connector.SendMessage(chatId, UserData.UserLogin, "В данном диалоге создана новая заявка!", out Error, out a);
                Connector.Save(out Error); MessageBox.Show(Error);
                Manager.MessagePart.GoBack();
                return;
            }
            if (request.StartTime < DateTime.Now.AddDays(-1)) { MessageBox.Show("Дата начала должна быть больше текущей даты!"); return; }
            if (request.EndTime == null) { MessageBox.Show("Не задана дата окончания выполнения!"); return; }
            if (request.EndTime < request.StartTime) { MessageBox.Show("Дата окончания выполнения должна быть больше даты старта!"); return; }
            if (string.IsNullOrWhiteSpace(request.name)) { MessageBox.Show("Имя заявки не задано!"); return; }
            Connector.SendMessage(chatId, UserData.UserLogin, $"В данном диалоге обновлена заявка - \"{request.name}\"! ", out Error, out a);
            Connector.Save(out Error); MessageBox.Show(Error);
            Manager.MessagePart.GoBack();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MessagePartBack();
        }
    }
}
