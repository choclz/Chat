//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClientChat
{
    using System;
    using System.Collections.Generic;
    
    public partial class MessageFiles
    {
        public int id { get; set; }
        public int messageId { get; set; }
        public byte[] File { get; set; }
    
        public virtual Messages Messages { get; set; }
    }
}
