using s32.Sceh.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.WinApp.UserNoteTags
{
    public class SteamAppTag : IUserNoteTag
    {
        public SteamAppTag()
        { }

        public SteamAppTag(SteamApp steamApp)
        {
            if (steamApp != null)
            {
                Id = steamApp.Id;
                Name = steamApp.Name;
            }
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string BuildTag()
        {
            return String.Concat("[app=", Id, ']', Name, "[/app]");
        }

        public override string ToString()
        {
            return BuildTag();
        }
    }
}