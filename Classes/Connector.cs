using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClientChat
{
    static class Connector
    {
        static public MessengerEntities _context = MessengerEntities.GetContext();
        /// <summary>
        /// Метод шифрации пароля с использованием SHA256
        /// </summary>
        /// <param name="pass">Пароль для шифрации</param>
        /// <returns>Возвращает хэш пароля</returns>
        public static string PassHash(string pass)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(pass));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        /// <summary>
        /// Метод получения списка всех пользователей
        /// </summary>
        /// <returns>Возращает список всех пользователей</returns>
        public static List<Users> GetUsers()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static bool CheckPass(string pass, string Nick)
        {
            try
            {
                return UserPassword(GetUserId(Nick)) == PassHash(pass) ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Получение пользователя
        /// </summary>
        /// <param name="Login">Логин пользователя</param>
        /// <returns></returns>
        public static Users GetUser(string Login)
        {
            try { 
                return _context.Users.Where(p => p.nickname == Login).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Получает роль пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Статус пользователя числом</returns>
        public static int GetRole(int userId)
        {
            try
            {
                return Convert.ToInt32(MessengerEntities.GetContext().Users.FirstOrDefault(p => p.id == userId));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        /// <summary>
        /// Возвращает пароль пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Возвращает пароль в зашифрованном виде</returns>
        public static string UserPassword(int userId)
        {
            try
            {
                return _context.Auth.Where(p => p.id == userId).Select(p => p.Password).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Идентификаторы чатов пользователей
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        private static List<int> ChatsID(int userId)
        {
            try { 
                return _context.UsersChats.Where(p => p.UserId == userId).Select(p => p.ChatId).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Получает список диалогов пользователя
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Возвращает список чатов пользователя</returns>
        public static List<Chats> GetChats(int userId)
        {
            try { 
            List<Chats> all = new List<Chats>();
            List<int> ids = ChatsID(userId);
            foreach (int id in ids)
            {
                all.Add(_context.Chats.SingleOrDefault(p => p.id == id));
            }
            return all;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public static bool IsUserExist(string login)
        {
            try { 
                return _context.Users.Where(p => p.nickname == login).Count() > 0 ? true : false; }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static int GetUserId(string login)
        {
            try { return _context.Users.Where(p => p.nickname == login).Select(p => p.id).SingleOrDefault(); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        /// <summary>
            /// Добавление нового пользователя в систему
            /// </summary>
            /// <param name="Nick">Логин пользователя</param>
            /// <param name="FN">Имя пользователя</param>
            /// <param name="SN">Фамилия пользователя</param>
            /// <param name="Role">Статус пользователя</param>
            /// <returns>1- пользователь добавлен, -1 - пользователь не добавлен</returns>
        public static int AddUser(string Nick, string FN, string SN, string Pass, int Role, out string Errors)
        {
            if (IsUserExist(Nick)) { Errors = "Пользователь c таким логином уже существует"; return -1; }
            string PassSecurity = @"(?=.*[0-9])(?=.*[!@#$%^&*])[0-9a-zA-Zа-яА-ЯёЁ!@#$%^&*]{6,}";
            if (!Regex.IsMatch(Pass, PassSecurity, RegexOptions.IgnoreCase)) { Errors = "Пароль должен содержать: \n- Хотя бы одно число\n- Хотя бы один спецсимвол\n- не менее 6 символов"; return -1; }
            Users newUser = new Users();
            Auth auth = new Auth();
            newUser.nickname = Nick;
            newUser.FName = FN;
            newUser.SName = SN;
            newUser.Role = Role;
            auth.Password = PassHash(Pass);
            try
            {
                _context.Users.Add(newUser);
                auth.userId = newUser.id;
                _context.Auth.Add(auth);
                _context.SaveChanges();
                Errors = "Пользователь успешно добавлен!";
                return 1;
            }
            catch (Exception ex)
            {
                Errors = "Произошла внутренняя ошибка, повторите позже.";
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        /// <summary>
        /// Добавление нового пользователя в систему
        /// </summary>
        /// <param name="Nick">Логин пользователя</param>
        /// <param name="FN">Имя пользователя</param>
        /// <param name="SN">Фамилия пользователя</param>
        /// <param name="LN">Отчество пользователя</param>
        /// <param name="Role">Статус пользователя</param>
        /// <returns>1- пользователь добавлен, -1 - пользователь не добавлен</returns>
        public static int AddUser(string Nick, string FN, string SN, string LN, string Pass, int Role, out string Errors)
        {
            if (IsUserExist(Nick)) { Errors = "Пользователь c таким логином уже существует"; return -1; }
            string PassSecurity = @"(?=.*[0-9])(?=.*[!@#$%^&*])[0-9a-zA-Zа-яА-ЯёЁ!@#$%^&*]{6,}";
            if (!Regex.IsMatch(Pass, PassSecurity, RegexOptions.IgnoreCase)) { Errors = "Пароль должен содержать: \n- Хотя бы одно число\n- Хотя бы один спецсимвол\n- не менее 6 символов"; return -1; }
            if (Role < 1 || Role > 4) { Errors = "Роль пользователя указана с ошибкой, проверьте правильность введённых данных!"; return -1; }
            Users newUser = new Users();
            Auth auth = new Auth();
            newUser.nickname = Nick.ToLower();
            newUser.FName = FN;
            newUser.SName = SN;
            newUser.Role = Role;
            newUser.ThName = LN;
            auth.Password = PassHash(Pass);
            try
            {
                _context.Users.Add(newUser);
                auth.userId = newUser.id;
                _context.Auth.Add(auth);
                _context.SaveChanges();
                Errors = "Пользователь успешно добавлен!";
                return 1;
            }
            catch (Exception ex)
            {
                Errors = "Произошла внутренняя ошибка, повторите позже.";
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        /// <summary>
        /// Установка фотографии профиля
        /// </summary>
        /// <param name="Nick">Логин пользователя</param>
        /// <param name="img">Хэш картинки</param>
        /// <returns>Статус выполнения операции</returns>
        public static bool SetAvatar(string Nick, byte[] img)
        {
            Users user = GetUser(Nick);
            user.Photo = img;
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int EditUser(string Nick, string FN, string SN, string LN, string Status, int Role, out string Errors)
        {
            Users user = GetUser(Nick);
            if (Role < 1 || Role > 4) { Errors = "Роль пользователя указана с ошибкой, проверьте правильность введённых данных!"; return -1; }
            if (Status.Length > 200) { Errors = "Статус должен быть не длиннее 200 символов!"; return -1; }
            user.nickname = Nick;
            user.FName = FN;
            user.SName = SN;
            user.Role = Role;
            user.ThName = LN;
            user.Status = Status;
            try
            {
                _context.SaveChanges();
                Errors = "Данные пользователя успешно изменены!";
                return 1;
            }
            catch (Exception ex)
            {
                Errors = "Произошла внутренняя ошибка, повторите позже.";
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        public static int CreateChat(string nick, string[] nick2, string name, int type, out string Errors)
        {
            if (!IsUserExist(nick)) { Errors = "Данного администратора не существует в системе!"; return -1; }
            foreach (string nn in nick2)
            {
                if (!IsUserExist(nn)) { Errors = "Один из участников не существует в системе!"; return -1; }
            }
            if (String.IsNullOrWhiteSpace(name)) { Errors = "Название беседы должно быть заполнено!"; return -1; }
            if (type < 0 || type > 1) { Errors = "Тип беседы - число от 0 до 1, где 0 - личная беседа, 1 - общий чат"; return -1; }
            Chats chats = new Chats();
            List<UsersChats> uc = new List<UsersChats>();
            chats.admin = GetUserId(nick);
            chats.name = name;
            chats.type = type == 1;
            try
            {
                _context.Chats.Add(chats);
                uc.Add(new UsersChats { ChatId = chats.id, UserId = chats.admin });
                foreach (string nn in nick2)
                {
                    uc.Add(new UsersChats { ChatId = chats.id, UserId = GetUserId(nn) });
                }
                _context.UsersChats.AddRange(uc);
                _context.SaveChanges();
                Errors = "Ошибок не обнаружено, чат успешно создан!";

                return 1;
            }
            catch (Exception ex)
            {
                Errors = "Ошибка регистрации нового чата - " + ex.Message;
                return -1;
            }
        }
        public static int CreateChat(string nick, string nick2, string name, int type, out string Errors)
        {
            if (!IsUserExist(nick)) { Errors = "Данного администратора не существует в системе!"; return -1; }
            if (!IsUserExist(nick2)) { Errors = "Один из участников не существует в системе!"; return -1; }
            if (String.IsNullOrWhiteSpace(name)) { Errors = "Название беседы должно быть заполнено!"; return -1; }
            if (type < 0 || type > 1) { Errors = "Тип беседы - число от 0 до 1, где 0 - личная беседа, 1 - общий чат"; return -1; }
            Chats chats = new Chats();
            List<UsersChats> uc = new List<UsersChats>();
            chats.admin = GetUserId(nick);
            chats.name = name;
            chats.type = type == 1;
            try
            {
                _context.Chats.Add(chats);
                uc.Add(new UsersChats { ChatId = chats.id, UserId = chats.admin });
                uc.Add(new UsersChats { ChatId = chats.id, UserId = GetUserId(nick2) });
                _context.UsersChats.AddRange(uc);
                _context.SaveChanges();
                Errors = "Ошибок не обнаружено, чат успешно создан!";

                return _context.Chats.Last().id;
            }
            catch (Exception ex)
            {
                Errors = "Ошибка регистрации нового чата - " + ex.Message;
                return -1;
            }
        }
        public static bool ChatExist(int chatId) => _context.Chats.Where(p => p.id == chatId).Count() > 0 ? true : false;
        public static bool CanSendMessage(int userID, int ChatsId) => _context.UsersChats.Where(p => p.ChatId == ChatsId && p.UserId == userID).Count() > 0 ? true : false;
        public static int SendMessage(int chatId, string nickname, string text, bool hasFiles, out string Errors, out int messageId)
        {
            if (!ChatExist(chatId)) { Errors = "Чат не существует, проверьте исходные данные!"; messageId = -1; return -1; }
            if (!IsUserExist(nickname)) { Errors = "Пользователь - отправитель не существует!"; messageId = -1; return -1; }
            if (!CanSendMessage(GetUserId(nickname), chatId)) { Errors = "Пользователь не состоит в данном чате!"; messageId = -1; return -1; }
            Messages msg = new Messages();
            msg.ChatId = chatId;
            msg.from = GetUserId(nickname);
            msg.text = text;
            msg.Filles = hasFiles;
            msg.date = DateTime.UtcNow;
            try
            {
                _context.Messages.Add(msg);
                _context.SaveChanges();
                Errors = "Ошибок не обнаружено, сообщение отправлено!";
                messageId = msg.id;
                return 1;
            }
            catch (Exception ex)
            {
                Errors = "Ошибка регистрации нового сообщения - " + ex.Message;
                messageId = -1;
                return -1;
            }
        }
        public static List<Message> GetMessagesFromChat(int chat_id, int userId)
        {
            try
            {
                List<Message> Output = new List<Message>();
                var mes = MessengerEntities.GetContext().Messages.Where(p => p.ChatId == chat_id).ToList();
                foreach (Messages mess in mes)
                {
                    Output.Add(new Message(mess.id, mess.Users.nickname + $"({mess.Users.FName} {mess.Users.SName})", mess.text, mess.date, mess.from == userId, mess.Users.Photo));   
                }
                return Output;
            }
            catch
            {
                return null;
            }
        }

        public static List<Message> GetMessagesFromChat(int chat_id, int userId, int skip)
        {
            try
            {
                List<Message> Output = new List<Message>();
                var mes = MessengerEntities.GetContext().Messages.Where(p => p.ChatId == chat_id).Skip(skip).ToList();
                foreach (Messages mess in mes)
                {
                    Output.Add(new Message(mess.id, mess.Users.nickname, mess.text, mess.date, mess.from == userId, mess.Users.Photo));
                }
                return Output;
            }
            catch
            {
                return null;
            }
        }
    }
}
