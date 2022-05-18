using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientChat
{
    class Manager
    {
        public static Frame MainFrame { get; set; }
        public static Frame MessagePart { get; set; }

        public static void MessagePartBack()
        {
            MessagePart.GoBack();
        }
    }
}
