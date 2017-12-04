using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace s32.Sceh.DataStore
{
    public class ScehData
    {
        public const string NS_SCEH = "http://schema.sinus32.net.pl/sceh/";
        public const string NS_XML = "http://www.w3.org/XML/1998/namespace";
        public const string NS_XS = "http://www.w3.org/2001/XMLSchema";
        public const string NS_XSI = "http://www.w3.org/2001/XMLSchema-instance";

        public string AppDataPath { get; set; }
        public ImageDirectory AvatarsDirectory { get; set; }
        public string AvatarsDirectoryPath { get; set; }
        public ImageDirectory CardsDirectory { get; set; }
        public string CardsDirectoryPath { get; set; }
        public ScehDataFile DataFile { get; set; }
        public string DataFilePath { get; set; }
        public string LocalAppDataPath { get; set; }
    }
}