using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace s32.Sceh.Models
{
    public class LoginModel
    {
        [Display(Name = "My profile")]
        [Required]
        [RegularExpression("^[0-9a-zA-Z_-]{3,20}$")]
        public string MyProfile { get; set; }
    }

    public class IndexModel
    {
        [Display(Name = "My profile")]
        public string MyProfile { get; set; }

        [Display(Name = "Other profile")]
        [Required]
        [RegularExpression("^[0-9a-zA-Z_-]{3,20}$")]
        public string OtherProfile { get; set; }
    }
}