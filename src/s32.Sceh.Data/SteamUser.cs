using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace s32.Sceh.Data
{
    public class SteamUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Index("SteamUser_Login_UQ", IsUnique= true)]
        [MaxLength(250)]
        public string Login { get; set; }

        public long SteamId { get; set; }

        public string UserUrl { get; set; }
    }
}