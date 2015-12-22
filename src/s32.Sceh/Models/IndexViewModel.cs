using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using s32.Sceh.Code;

namespace s32.Sceh.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(IndexModel model)
        {
            Input = model;
        }

        public IndexModel Input { get; set; }

        public Inventory MyInv { get; set; }

        public Inventory OtherInv { get; set; }

        public List<SteamApp> SteamApps { get; set; }

        public int OriginalsUsed { get; set; }

        public int ThumbnailsUsed { get; set; }
    }
}