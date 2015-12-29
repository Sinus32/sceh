using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using s32.Sceh.Interfaces;

namespace s32.Sceh.Data
{
    public class SteamUser : ISteamUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Index("SteamUser_Login_UQ", IsUnique = true)]
        [MaxLength(250)]
        public string Login { get; set; }

        public long SteamId { get; set; }

        public string UserUrl { get; set; }
    }
}