using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s32.Sceh.Code
{
    public class SteamUrlPattern
    {
        public const string SteamCommunityPageByCustomUrl = "http://steamcommunity.com/id/";
        public const string SteamCommunityPageBySteamId = "http://steamcommunity.com/profiles/";

        public static readonly SteamUrlPattern CommunityPage, ApiGetInventoryA,
            ApiGetProfile, ApiGetInventoryB, Badges, Inventory,
            TradeOffers, TradeTopics, PostHistory;

        static SteamUrlPattern()
        {
            CommunityPage = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}",
                SteamCommunityPageBySteamId + "{0}");
            ApiGetInventoryA = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}/inventory/json/753/6",
                SteamCommunityPageBySteamId + "{0}/inventory/json/753/6");
            ApiGetInventoryB = new SteamUrlPattern(
                null,
                "http://steamcommunity.com/inventory/{0}/753/6?l=polish&count=2000");
            ApiGetProfile = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}?xml=1",
                SteamCommunityPageBySteamId + "{0}?xml=1");
            Badges = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}/badges",
                SteamCommunityPageBySteamId + "{0}/badges");
            Inventory = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}/inventory",
                SteamCommunityPageBySteamId + "{0}/inventory");
            TradeOffers = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}/tradeoffers",
                SteamCommunityPageBySteamId + "{0}/tradeoffers");
            TradeTopics = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}/tradeoffers/tradetopics",
                SteamCommunityPageBySteamId + "{0}/tradeoffers/tradetopics");
            PostHistory = new SteamUrlPattern(
                SteamCommunityPageByCustomUrl + "{0}/posthistory",
                SteamCommunityPageBySteamId + "{0}/posthistory");
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