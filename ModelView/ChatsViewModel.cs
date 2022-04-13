using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ClientChat.ModelView
{
    class ChatsViewModel : ObservableObject
    {
        public ObservableCollection<Messages> Messages { get; set; }
        public ObservableCollection<Chats> Chats { get; set; }

        /* Commands */
        public RelayCommand SendCommand { set; get; }
        private Chats _selectedChat;

        public Chats SelectedChat
        {
            get { return _selectedChat; }
            set { _selectedChat = value; OnPropertyChanged(); }
        }

        private string _message;

        public string Message
        {       
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        public ChatsViewModel(Chats chats, Messages msg)
        {

        }

    }
}
