using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using s32.Sceh.Data;
using s32.Sceh.Interfaces;

namespace s32.Sceh.Code
{
    public class SteamUsers
    {
        public static ISteamUser Get(string me)
        {
            return new SteamUser()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}