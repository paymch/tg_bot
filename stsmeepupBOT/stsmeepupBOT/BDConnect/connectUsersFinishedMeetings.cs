//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace stsmeepupBOT.BDConnect
{
    using System;
    using System.Collections.Generic;
    
    public partial class connectUsersFinishedMeetings
    {
        public int id_connect { get; set; }
        public int id_finishedMeeting { get; set; }
        public int id_user { get; set; }
    
        public virtual finishedMeetings finishedMeetings { get; set; }
        public virtual users users { get; set; }
    }
}
