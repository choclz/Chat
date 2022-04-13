using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientChat
{
    class Message
    {
        private int id { get; }
        public byte[] avatar { get; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime date { get; set; }
        public bool IsAuthor { get; set; }


        public Message(int id, string author, string text, DateTime date, bool isAuthor, byte[] avatar = null)
        {
            this.id = id;
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            this.date = date;
            IsAuthor = isAuthor;
        }
    }
}
