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
    
    public partial class Auth
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string Password { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
