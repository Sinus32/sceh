﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace s32.Sceh.DataStore
{
    [Serializable]
    [XmlRoot("SteamProfile", Namespace = ScehData.NS_SCEH)]
    public class SteamProfile : SteamProfileKey
    {
        [XmlAttribute]
        public DateTime LastUpdate { get; set; }

        [XmlElement(Order = 0)]
        public DateTime LastUse { get; set; }

        [XmlElement(Order = 1)]
        public string Name { get; set; }

        [XmlElement(Order = 2)]
        public string AvatarSmallUrl { get; set; }

        [XmlElement(Order = 3)]
        public string AvatarMediumUrl { get; set; }

        [XmlElement(Order = 4)]
        public string AvatarFullUrl { get; set; }

        [XmlElement(Order = 5)]
        public string Note { get; set; }
    }
}