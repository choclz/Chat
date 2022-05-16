using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Entity;
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
        public static List<Users> GetUsers(out string Errors)
        {
            try
            {
                Errors = null;
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Errors = "Ошибка выборки пользователей системы, операция не завершена. Код ошибки \n" + ex.Message;
                return null;
            }
        }
        /// <summary>
        /// Обновление данных в таблицах
        /// </summary>
        /// <param name="Error">Ошибки в работе программы</param>
        public static void Update(out string Error)
        {
            try
            {
                _context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                Error = null;
            }
            catch (Exception ex)
            {
                Error = "Ошибка обновления данных!\n" + ex.Message;
            }
        }
        /// <summary>
        /// Операция сохраниния данных в базе
        /// </summary>
        /// <param name="Error">Статус операции</param>
        /// <returns>Статус ошибки, где -1 - ошибка, и наоборот</returns>
        public static int Save(out string Error)
        {
            try
            {
                _context.SaveChanges();
                Error = "Данные успешно сохранены!";
                return 1;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return -1;
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
        public static int AddRequest(Requests request, out string Errors)
        {
            if (request.StartTime < DateTime.Now.AddDays(-1)) { Errors = "Дата начала должна быть больше текущей даты!"; return -1; }
            if (request.EndTime == null) { Errors = "Не задана дата окончания выполнения!"; return -1; }
            if (request.EndTime < request.StartTime) { Errors = "Дата окончания выполнения должна быть больше даты старта!"; return -1; }
            if (string.IsNullOrWhiteSpace(request.name)) { Errors = "Имя заявки не задано!"; return -1; }
            request.status = 1;
            try
            {
                MessengerEntities.GetContext().Requests.Add(request);
                return Save(out Errors);
            }
            catch
            {
                Errors = "Невозможно добавить заявку, попробуйте позже."; return -1;
            }
        }
        public static int AddTask(Tasks request, out string Errors)
        {
            if (request.StartTime < DateTime.Now.AddDays(-1)) { Errors = "Дата начала должна быть больше текущей даты!"; return -1; }
            if (request.EndTime == null) { Errors = "Не задана дата окончания выполнения!"; return -1; }
            if (request.EndTime < request.StartTime) { Errors = "Дата окончания выполнения должна быть больше даты старта!"; return -1; }
            if (string.IsNullOrWhiteSpace(request.TaskName)) { Errors = "Имя задачи не задано!"; return -1; }
            if (string.IsNullOrWhiteSpace(request.Description)) { Errors = "Описание задачи не задано!"; return -1; }
            try
            {
                MessengerEntities.GetContext().Tasks.Add(request);
                return Save(out Errors);
            }
            catch
            {
                Errors = "Невозможно добавить задачу, попробуйте позже."; return -1;
            }

        }
        public static int CreateChat(string nick, string[] nick2, string name, out string Errors)
        {
            int type = 0;
            if (!IsUserExist(nick)) { Errors = "Данного администратора не существует в системе!"; return -1; }
            foreach (string nn in nick2)
            {
                if (!IsUserExist(nn)) { Errors = "Один из участников не существует в системе!"; return -1; }
            }
            if (string.IsNullOrWhiteSpace(name)) { Errors = "Название беседы должно быть заполнено!"; return -1; }
            if (nick2.Length == 0) { Errors = "Отсутсвуют участники беседы!"; return -1; }
            if (nick2.Length > 1) type = 1;
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
        public static int AddFile(byte[] file, string name, out int FileID)
        {
            try
            {
                FileID = _context.MessageFiles.Add(new MessageFiles() { Name = name, File = file }).id;
                return 1;
            }
            catch
            {
                FileID = -1;
                return -1;
            }
        }
        public static string TasksReady(int ReqId)
        {
            int all_current = _context.UserTask.Where(p => p.Tasks.ReqId == ReqId).Count();
            int done = _context.UserTask.Where(p => p.Tasks.ReqId == ReqId && p.status == 2).Count();
            return done + "/" + all_current;
        }
        public static bool ChatExist(int chatId) => _context.Chats.Where(p => p.id == chatId).Count() > 0 ? true : false;
        public static bool CanSendMessage(int userID, int ChatsId) => _context.UsersChats.Where(p => p.ChatId == ChatsId && p.UserId == userID).Count() > 0 ? true : false;
        public static int SendMessage(int chatId, string nickname, string text, out string Errors, out int messageId, int? hasFiles = null)
        {
            if (!ChatExist(chatId)) { Errors = "Чат не существует, проверьте исходные данные!"; messageId = -1; return -1; }
            if (!IsUserExist(nickname)) { Errors = "Пользователь - отправитель не существует!"; messageId = -1; return -1; }
            if (!CanSendMessage(GetUserId(nickname), chatId)) { Errors = "Пользователь не состоит в данном чате!"; messageId = -1; return -1; }
            if (string.IsNullOrWhiteSpace(text)) { Errors = "Сообщение не может быть пустым!"; messageId = -1; return -1; }
            Messages msg = new Messages();
            msg.ChatId = chatId;
            msg.from = GetUserId(nickname);
            msg.text = text;
            msg.Files = hasFiles;
            msg.date = DateTime.Now;
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
            List<Message> Output = new List<Message>();
            try
            {
                var mes = MessengerEntities.GetContext().Messages.Where(p => p.ChatId == chat_id).ToList();
                foreach (Messages mess in mes)
                {
                    Output.Add(new Message(mess.id, mess.Users.nickname + $"({mess.Users.FName} {mess.Users.SName})", mess.text, mess.date, mess.from == userId, mess.Users.Photo));   
                }
                return Output;
            }
            catch
            {
                return Output;
            }
        }
        public static List<Message> GetMessagesFromChat(int chat_id, int userId, int skip)
        {
            List<Message> Output = new List<Message>();
            try
            {
                var mes = MessengerEntities.GetContext().Messages.Where(p => p.ChatId == chat_id).ToList().Skip(skip);
                foreach (Messages mess in mes)
                {
                    Output.Add(new Message(mess.id, mess.Users.nickname + $"({mess.Users.FName} {mess.Users.SName})", mess.text, mess.date, mess.from == userId, mess.Users.Photo));
                }
                return Output;
            }
            catch
            {
                return Output;
            }
        }
        public static List<Requests> GetUserRequests(int chatId, int userID, bool Author)
        {
            if (Author)
            {
                return MessengerEntities.GetContext().Requests.Where(p => p.customer == userID && p.RequestFrom == chatId).ToList();
            }
            return MessengerEntities.GetContext().Requests.Where(p => p.customer != userID && p.RequestFrom == chatId).ToList();
        }
    }
}
