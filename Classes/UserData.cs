using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientChat
{
    class UserData
    {
        public static int UserId { get; set; }
        public static string UserLogin { get; set; }
        public static bool ServerAvailable { get; set; } = false;
    }
}
