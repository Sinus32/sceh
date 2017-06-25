using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Classes
{
    public class SteamUser
    {
        public long SteamId
        {
            get { return Profile != null ? Profile.SteamId : 0L; }
        }

        public string GetName()
        {
            return Profile != null && !String.IsNullOrEmpty(Profile.Name) ? Profile.Name : null;
        }

        public SteamProfile Profile { get; set; }

        public List<Card> Cards { get; set; }

        public string GetProfileUrl(string page = null)
        {
            if (Profile == null)
                return null;

            string result;
            if (!String.IsNullOrEmpty(Profile.CustomURL))
                result = String.Concat("http://steamcommunity.com/id/", Profile.CustomURL);
            else
                result = String.Concat("http://steamcommunity.com/profiles/", Profile.SteamId);

            return page == null ? result : String.Concat(result, '/', page);
        }
    }
}
