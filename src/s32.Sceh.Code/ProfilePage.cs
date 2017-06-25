using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class ProfilePage
    {
        public static readonly ProfilePage API_GET_INVENTORY = new ProfilePage("/inventory/json/753/6");
        public static readonly ProfilePage API_GET_PROFILE = new ProfilePage("?xml=1");
        public static readonly ProfilePage BADGES = new ProfilePage("/badges");
        public static readonly ProfilePage INVENTORY = new ProfilePage("/inventory");
        private string _pageUrl;

        public ProfilePage(string pageUrl)
        {
            _pageUrl = pageUrl;
        }

        public string PageUrl
        {
            get { return _pageUrl; }
        }

        public override string ToString()
        {
            return _pageUrl;
        }
    }
}