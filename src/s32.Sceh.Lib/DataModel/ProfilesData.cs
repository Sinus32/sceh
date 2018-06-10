using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.DataModel
{
    public class ProfilesData
    {
        public ProfilesData()
        {
            SteamProfiles = new List<SteamProfile>();
        }

        public bool AutoLogIn { get; set; }
        public long LastSteamProfileId { get; set; }
        public List<SteamProfile> SteamProfiles { get; set; }
    }
}
