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
                SetName(steamApp.Name);
            }
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string BuildTag()
        {
            return String.Concat("[app=", Id, ']', Name, "[/app]");
        }

        public void SetName(string steamAppName)
        {
            if (String.IsNullOrEmpty(steamAppName))
            {
                Name = String.Empty;
                return;
            }

            var pos = steamAppName.LastIndexOf(" - ");
            if (pos > 0)
                Name = steamAppName.Remove(pos);
            else
                Name = steamAppName;
        }

        public override string ToString()
        {
            return BuildTag();
        }
    }
}