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
    
    public partial class UserTask
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public int status { get; set; }
        public Nullable<int> FileId { get; set; }
        public string Comment { get; set; }
    
        public virtual TaskFiles TaskFiles { get; set; }
        public virtual Tasks Tasks { get; set; }
        public virtual TaskStatus TaskStatus { get; set; }
        public virtual Users Users { get; set; }
    }
}
