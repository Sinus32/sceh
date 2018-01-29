using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;

namespace s32.Sceh.UserNoteTags
{
    public class SteamAppTag : IUserNoteTag
    {
        public const string TagName = "app";

        public SteamAppTag()
        { }

        public SteamAppTag(long appId, string appName)
        {
            AppId = appId;
            AppName = appName;
        }

        public SteamAppTag(SteamApp steamApp)
        {
            if (steamApp != null)
            {
                AppId = steamApp.Id;
                AppName = steamApp.Name;
            }
        }

        public long AppId { get; set; }

        public string AppName { get; set; }

        public string Name
        {
            get { return TagName; }
        }

        public string BuildSourceText()
        {
            return String.Concat("[app=", AppId, ']', AppName, "[/app]");
        }

        public string GetFormatedText()
        {
            return AppName;
        }

        public override string ToString()
        {
            return BuildSourceText();
        }
    }
}