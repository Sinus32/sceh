using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace s32.Sceh.Data
{
    public class ScehData
    {
        public const string XML_NAMESPACE = "http://sinus32.net.pl/sceh/";

        public string AppDataPath { get; set; }
        public ImageDirectory AvatarsDirectory { get; set; }
        public string AvatarsDirectoryPath { get; set; }
        public ImageDirectory CardsDirectory { get; set; }
        public string CardsDirectoryPath { get; set; }
        public ScehDataFile DataFile { get; set; }
        public string DataFilePath { get; set; }
    }
}