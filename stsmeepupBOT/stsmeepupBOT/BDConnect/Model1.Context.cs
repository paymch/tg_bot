﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class stsmeepupEntities : DbContext
    {
        private static stsmeepupEntities _context;
        public stsmeepupEntities()
            : base("name=stsmeepupEntities")
        {
        }
        public static stsmeepupEntities GetContext()
        {
            if (_context == null)
                _context = new stsmeepupEntities();
            return _context;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<connectChatAdmin> connectChatAdmin { get; set; }
        public virtual DbSet<typeStatusMeeting> typeStatusMeeting { get; set; }
        public virtual DbSet<meetings> meetings { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<admins> admins { get; set; }
        public virtual DbSet<chats> chats { get; set; }
        public virtual DbSet<connectUsersFinishedMeetings> connectUsersFinishedMeetings { get; set; }
        public virtual DbSet<finishedMeetings> finishedMeetings { get; set; }
        public virtual DbSet<users> users { get; set; }
    }
}
