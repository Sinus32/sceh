using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace s32.Sceh.DataModel
{
    public class ScehData
    {
        public const string NS_SCEH = "http://schema.sinus32.net.pl/sceh/";
        public const string NS_XML = "http://www.w3.org/XML/1998/namespace";
        public const string NS_XS = "http://www.w3.org/2001/XMLSchema";
        public const string NS_XSI = "http://www.w3.org/2001/XMLSchema-instance";

        public ImageDirectory AvatarsDirectory { get; set; }
        public ImageDirectory CardsDirectory { get; set; }

        public IEnumerable<ImageDirectory> ImageDirectories
        {
            get { return new ImageDirectory[] { AvatarsDirectory, CardsDirectory }; }
        }

        public PathConfig Paths { get; set; }
        public ProfilesData Profiles { get; set; }
    }
}