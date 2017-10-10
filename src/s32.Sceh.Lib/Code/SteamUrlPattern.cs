using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class SteamUrlPattern
    {
        public static readonly SteamUrlPattern CommunityPage, ApiGetInventory,
            ApiGetProfile, Api2GetInventory, Badges, Inventory;

        static SteamUrlPattern()
        {
            CommunityPage = new SteamUrlPattern(
                SteamDataDownloader.SteamCommunityPageByCustomUrl + "{0}",
                SteamDataDownloader.SteamCommunityPageBySteamId + "{0}");
            ApiGetInventory = new SteamUrlPattern(
                SteamDataDownloader.SteamCommunityPageByCustomUrl + "{0}/inventory/json/753/6",
                SteamDataDownloader.SteamCommunityPageBySteamId + "{0}/inventory/json/753/6");
            ApiGetProfile = new SteamUrlPattern(
                SteamDataDownloader.SteamCommunityPageByCustomUrl + "{0}?xml=1",
                SteamDataDownloader.SteamCommunityPageBySteamId + "{0}?xml=1");
            Api2GetInventory = new SteamUrlPattern(
                null,
                "http://steamcommunity.com/inventory/{0}/753/6?l=polish&count=2000");
            Badges = new SteamUrlPattern(
                SteamDataDownloader.SteamCommunityPageByCustomUrl + "{0}/badges",
                SteamDataDownloader.SteamCommunityPageBySteamId + "{0}/badges");
            Inventory = new SteamUrlPattern(
                SteamDataDownloader.SteamCommunityPageByCustomUrl + "{0}/inventory",
                SteamDataDownloader.SteamCommunityPageBySteamId + "{0}/inventory");
        }

        private string _patternByCustomUrl, _patternBySteamId;

        public SteamUrlPattern(string patternByCustomUrl, string patternBySteamId)
        {
            _patternByCustomUrl = patternByCustomUrl;
            _patternBySteamId = patternBySteamId;
        }

        public Uri GetUrlByCustomUrl(string customUrl)
        {
            if (_patternByCustomUrl != null)
                return new Uri(String.Format(_patternByCustomUrl, customUrl));
            throw new NotSupportedException();
        }

        public Uri GetUrlBySteamId(long steamId)
        {
            if (_patternBySteamId != null)
                return new Uri(String.Format(_patternBySteamId, steamId));
            throw new NotSupportedException();
        }

        public bool HasSupportForCustomUrl
        {
            get { return _patternByCustomUrl != null; }
        }

        public bool HasSupportForSteamId
        {
            get { return _patternBySteamId != null; }
        }
    }
}