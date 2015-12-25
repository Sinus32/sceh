using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace s32.Sceh.Data
{
    public class SteamAppInfo
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}