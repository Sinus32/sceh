using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using s32.Sceh.DataModel;
using s32.Sceh.SteamApi;

namespace s32.Sceh.Classes
{
    public class SteamUser
    {
        public List<Card> Cards { get; set; }

        public SteamProfileResp Profile { get; set; }

        public long SteamId
        {
            get { return Profile != null ? Profile.SteamId : 0L; }
        }

        public string GetName()
        {
            return Profile != null && !String.IsNullOrEmpty(Profile.Name) ? Profile.Name : null;
        }
    }
}