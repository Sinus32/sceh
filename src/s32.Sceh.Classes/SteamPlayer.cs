﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.Classes
{
    [XmlRoot("profile")]
    public class SteamPlayer
    {
        [XmlElement("steamID64")]
        public long SteamId{get;set;}
        
        [XmlElement("steamID")]
        public string Name{get;set;}

        [XmlElement("onlineState")]
        public StemOnlineState OnlineState{get;set;}
        
        [XmlElement("stateMessage")]
        public string StateMessage{get;set;}
        
        [XmlElement("privacyState")]
        public string PrivacyState{get;set;}

        [XmlElement("visibilityState")]
        public int VisibilityState{get;set;}
        
        [XmlElement("avatarIcon")]
        public string AvatarIcon{get;set;}

        [XmlElement("avatarMedium")]
        public string AvatarIconMedium{get;set;}

        [XmlElement("avatarFull")]
        public string AvatarIconFull{get;set;}
        
        [XmlElement("vacBanned")]
        public int VacBanned{get;set;}
        
        [XmlElement("tradeBanState")]
        public string TradeBanState{get;set;}
        
        [XmlElement("isLimitedAccount")]
        public int IsLimitedAccount{get;set;}
        
        [XmlElement("customURL")]
        public string CustomURL{get;set;}
        
        [XmlElement("memberSince")]
        public string MemberSince{get;set;}
        
        [XmlElement("steamRating")]
        public string SteamRating{get;set;}
        
        [XmlElement("hoursPlayed2Wk")]
        public double HoursPlayed2Wk{get;set;}
        
        [XmlElement("headline")]
        public string Headline{get;set;}
        
        [XmlElement("location")]
        public string Location{get;set;}
        
        [XmlElement("realname")]
        public string Realname{get;set;}
        
        public enum StemOnlineState
        {
            [XmlEnum("offline")]
            Offline,
            
            [XmlEnum("online")]
            Online,

            [XmlEnum("in-game")]
            InGame,
        }
    }
}
